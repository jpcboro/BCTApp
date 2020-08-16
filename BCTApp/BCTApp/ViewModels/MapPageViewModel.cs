using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BCTApp.Contants;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;
using Location = BCTApp.Models.Location;

namespace BCTApp
{
    public class MapPageViewModel : BindableBase, IInitialize, INavigatedAware, IInitializeAsync
    {
        private readonly INavigationService _navigationService;
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IDialogService _dialogService;
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
            
            _dialogService.ShowDialog("SampleDialog", new DialogParameters()
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

                AddHiveOnMap(newHive);
            }
            
           

        }

        private void AddHiveOnMap(Hive newHive)
        {
            var pin = new Pin()
            {
                Label = newHive.HiveName,
                Position = new Position(newHive.HiveLocation.Latitude, newHive.HiveLocation.Longitude),
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

        public MapPageViewModel(INavigationService _navigationService,
            IFirebaseHelper firebaseHelper,
            IDialogService dialogService
            )
        {
            this._navigationService = _navigationService;
            _dialogService = dialogService;
            _firebaseHelper = firebaseHelper;
            
            PinDragStartCommand = new DelegateCommand<PinDragEventArgs>((args)=> PinDragStart(args));
            PinDragEndCommand = new DelegateCommand<PinDragEventArgs>((args) => PinDragEnd(args));
            PinDraggingCommand = new DelegateCommand<PinDragEventArgs>((args) => PinDragging(args));
            MapClickedCommand = new DelegateCommand<MapClickedEventArgs>((args) => MapTappedToCreatePin(args));
            
            
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

        private void OnMoveDialogClosed(IDialogResult obj)
        {
            
                foreach (var item in Pins)
                {
                    if (item.Label == SelectedPinOrigLoc.Label)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Pins.Clear();
                            GetUserHives(Settings.UID);
                           
                        });
                    
                    } 
                }
            
        }

        public bool PinDragged { get; set; }

        public Location PinNewLocation { get; set; }

        private void PinDragStart(PinDragEventArgs args)
        {
            IsMapTapEnabled = false;
            
            SelectedPinOrigLoc = args.Pin;
            
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
            // string mapCoordinates = $"Add marker at {args.Point.Latitude}, {args.Point.Longitude}";
            // _dialogService.ShowDialog("SampleDialog", new DialogParameters()
            // {
            //     {"message", mapCoordinates}
            // });

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

          
         
            // var pinLatitude = args.Point.Latitude;
            // var pinLongitude = args.Point.Longitude;
            //
            //    
            // await CreateMapPins(pinLatitude, pinLongitude);
        }

        // private async Task CreateMapPins(double pinLatitude, double pinLongitude)
        // {
        //     var pinCount = Pins.Count + 1;
        //     var pinName = $"Hive {pinCount}";
        //     var pin = new Pin()
        //     {
        //         Label = pinName,
        //         Position = new Position(pinLatitude, pinLongitude),
        //         IsDraggable = true,
        //     };
        //
        //     Pins.Add(pin);
        //
        //     var newHive = new Hive()
        //     {
        //         HiveName = pinName,
        //         HiveLocation = new Location()
        //         {
        //             Latitude = pinLatitude,
        //             Longitude = pinLongitude
        //         }
        //     };
        //
        //
        //     await SaveNewHive(newHive);
        // }

        private async Task SaveNewHive(Hive newHive)
        {
            try
            {
                await _firebaseHelper.AddHive(_uid, newHive);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }

        public void Initialize(INavigationParameters parameters)
        {
            if (!string.IsNullOrEmpty(Settings.UID))
            {
                _uid = Settings.UID;
            }
            else
            {
                _uid = parameters.GetValue<string>(ParameterConstants.UID);

            }
            
            GetUserHives(_uid);
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
            if (Pins.Any())
            {
                Pins.Clear();

            }
          
            var userHives = await _firebaseHelper.GetAllUserHives(uid);
            foreach (var hive in userHives)
            {
                Pins.Add(new Pin()
                {
                    Label = hive.HiveName,
                    Position = new Position(hive.HiveLocation.Latitude, hive.HiveLocation.Longitude),
                    IsDraggable = true
                    
                });
            
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(500),  () =>
            {
                GotoUserLocation();
                return false;
            });
        }

        public async Task InitializeAsync(INavigationParameters parameters)
        {
           
           
            
        
        }
    }
}

