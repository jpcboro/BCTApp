using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BCTApp.Contants;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;
using DependencyService = Xamarin.Forms.DependencyService;
using Location = BCTApp.Models.Location;

namespace BCTApp
{
    public class MapPageViewModel : ViewModelBase, IInitialize, INavigatedAware, IInitializeAsync, IActiveAware
    {
        private readonly INavigationService _navigationService;
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
        private string _uid;

        private ObservableCollection<Pin> _pins;

        public ObservableCollection<Pin> Pins
        {
            get { return _pins; }
            set { SetProperty(ref _pins, value); }
        }

        private Pin _selectedHive;

        public Pin SelectedHive
        {
            get { return _selectedHive; }
            set { SetProperty(ref _selectedHive, value); }
        }

        public DelegateCommand<MapClickedEventArgs> MapClickedCommand { get; set; }
           
        
        public DelegateCommand<PinClickedEventArgs> PinClickedCommand =>
        new DelegateCommand<PinClickedEventArgs>(async (args) => await PinClicked(args));
        
        public DelegateCommand<PinDragEventArgs> PinDragStartCommand { get; set; }
        
        public DelegateCommand<PinDragEventArgs> PinDragEndCommand { get; set; }
        

        private async Task PinClicked(PinClickedEventArgs args)
        {
            var hive = new Hive()
            {
                HiveName = args.Pin.Label,
                HiveLocation = new Location()
                {
                    Latitude = args.Pin.Position.Latitude,
                    Longitude = args.Pin.Position.Longitude
                }
            };
            
            _dialogService.ShowDialog("DeleteHiveDialog", new DialogParameters()
            {
                {"message",hive}
            }, OnDialogClosed);
        }

        private async void OnDialogClosed(IDialogResult result)
        {
            if (result.Parameters.ContainsKey("deleted"))
            {
                var deletedHive = result.Parameters.GetValue<Hive>("deleted");
                

                RemoveHiveFromMap(deletedHive);
                
            }

            if (result.Parameters.ContainsKey("savedHive"))
            {
                var newHive =  result.Parameters.GetValue<Hive>("savedHive");

                _eventAggregator.GetEvent<UpdateHiveListEvent>().Publish(true);
                
                AddHiveOnMap(newHive);
                Device.StartTimer(TimeSpan.FromMilliseconds(100),  () =>
                {
                    MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(newHive.HiveLocation.Latitude,
                        newHive.HiveLocation.Longitude), Distance.FromKilometers(8)));
                    return false;
                });
                
                
            
            }
            
        }
        
        private async void OnMoveDialogClosed(IDialogResult result)
        {
            
            if (result.Parameters.ContainsKey("moveCanceled"))
            {
                var moveParams = result.Parameters.GetValue<bool>("moveCanceled");
             
                if (moveParams)
                {
                    GetUserHives(Settings.UID);
                    
                }
               
            }
            
        }

        private void AddHiveOnMap(Hive newHive)
        {
            var pin = new Pin()
            {
                Label = newHive.HiveName,
                Position = new Position(newHive.HiveLocation.Latitude, newHive.HiveLocation.Longitude),
                Icon = BitmapDescriptorFactory.FromBundle("map_pin.png"),
                IsDraggable = true,
            };

            Pins.Add(pin);

        }

        private void RemoveHiveFromMap(Hive deletedHive)
        {
            foreach (var item in Pins.ToList())
            {
                if (item.Label == deletedHive.HiveName)
                {
                    Pins.Remove(item);
                }
            }
            
        }

        public MoveToRegionRequest MoveToRegionReq { get; } = new MoveToRegionRequest();

        public MoveCameraRequest MoveCameraReq => new MoveCameraRequest();

        public MapPageViewModel(INavigationService navigationService,
            IFirebaseHelper firebaseHelper,
            IDialogService dialogService,
            IEventAggregator eventAggregator,
            IPageDialogService pageDialogService
        )
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _pageDialogService = pageDialogService;
            _firebaseHelper = firebaseHelper;
            
            _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();
            
            PinDragStartCommand = new DelegateCommand<PinDragEventArgs>((args)=> PinDragStart(args));
            PinDragEndCommand = new DelegateCommand<PinDragEventArgs>((args) => PinDragEnd(args));
            PinDraggingCommand = new DelegateCommand<PinDragEventArgs>((args) => PinDragging(args));
            MapClickedCommand = new DelegateCommand<MapClickedEventArgs>((args) => MapTappedToCreatePin(args));
            LogOutCommand = new DelegateCommand(()=> LogOut());
            
            Pins = new ObservableCollection<Pin>();
            
        }

        public DelegateCommand LogOutCommand { get; set; }

        private async void LogOut()
        {
            
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                return;
            }
            
            var signOutOk = _firebaseAuth.SignOut();
            if (signOutOk)
            {
                _navigationService.NavigateAsync(PageConstants.LoginPage);
                Settings.UID = String.Empty;
                
            }
           
        }

        public DelegateCommand<PinDragEventArgs> PinDraggingCommand { get; set; }

        private void PinDragging(PinDragEventArgs args)
        {
            PinDragged = true;
        }

        private bool CanSubmit(MapClickedEventArgs arg)
        {
            return false;
        }

        private bool _isMapTapEnabled = true;
        private IFirebaseAuthentication _firebaseAuth;

        public bool IsMapTapEnabled
        {
            get { return _isMapTapEnabled; }
            set { SetProperty(ref _isMapTapEnabled, value); }
        }

      
        private void PinDragEnd(PinDragEventArgs args)
        {
            SelectedPinNewLoc = args.Pin;
            
            PinNewLocation = new Location()
            {
                Latitude = args.Pin.Position.Latitude,
                Longitude = args.Pin.Position.Longitude
            };

            var movedHive = new Hive()
            {
                HiveName = SelectedPinNewLoc.Label,
                HiveLocation = new Location()
                {
                    Latitude = SelectedPinNewLoc.Position.Latitude,
                    Longitude = SelectedPinNewLoc.Position.Longitude
                }
            };

            if (PinDragged)
            {
                _dialogService.ShowDialog("MoveHivePage", new DialogParameters()
                {
                    {"movedHive", movedHive }
                }, OnMoveDialogClosed);

                PinDragged = false;
                IsMapTapEnabled = true;
            }

           
        }

      

        public bool PinDragged { get; set; }

        public Location PinNewLocation { get; set; }

        private void PinDragStart(PinDragEventArgs args)
        {
            IsMapTapEnabled = false;
            
            // SelectedPinOrigLoc = args.Pin;
            
            OriginalPinLocation = new Location()
            {
                Latitude = args.Pin.Position.Latitude,
                Longitude = args.Pin.Position.Longitude,
               
            };
        }

        public Pin SelectedPinOrigLoc { get; set; }
        
       public Pin SelectedPinNewLoc { get; set; }


        public Location OriginalPinLocation { get; set; }

        public async Task MapTappedToCreatePin(MapClickedEventArgs args)
        {
           

            if (IsMapTapEnabled)
            {
                var newHiveLocation = new Location()
                {
                    Latitude = args.Point.Latitude,
                    Longitude = args.Point.Longitude
                };
            
                _dialogService.ShowDialog("CreateNewHivePage", new DialogParameters()
                {
                    {"map_clicked", newHiveLocation}
                }, OnDialogClosed);
            }
            else
            {
               
            }

        }
        

        public void Initialize(INavigationParameters parameters)
        {
           
        }

        private async Task GotoUserLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest()
                    {
                        DesiredAccuracy = GeolocationAccuracy.Low,
                        Timeout = TimeSpan.FromSeconds(20)
                    });
                }
                
                MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude,
                location.Longitude), Distance.FromKilometers(10)));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

            }

        }

        private async Task GetUserHives(string uid)
        {
            
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                return;
            }
            
            var userHives = await _firebaseHelper.GetAllUserHives(uid);
            if (Pins.Any())
            {
                Pins.Clear();
            }
            foreach (var hive in userHives)
            {
                Pins.Add(new Pin()
                {
                    Label = hive.HiveName,
                    Position = new Position(hive.HiveLocation.Latitude, hive.HiveLocation.Longitude),
                    Icon = BitmapDescriptorFactory.FromBundle("map_pin.png"),
                    IsDraggable = true
                    
                });
            
            }

           
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                IsControlVisible = false;
            }
            else
            {
                IsControlVisible = true;
                if (!string.IsNullOrEmpty(Settings.UID))
                {
                    _uid = Settings.UID;
                }
         
                Device.StartTimer(TimeSpan.FromMilliseconds(500),  () =>
                {
                    GotoUserLocation();
                    GetUserHives(_uid);
                
                    return false;
                });
            }
            
            
          
        }

        public async Task InitializeAsync(INavigationParameters parameters)
        {
            
            _eventAggregator.GetEvent<UpdateHiveListEvent>().Publish(true);

        }
        
        public event EventHandler IsActiveChanged;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);

        }
      
    }
}

