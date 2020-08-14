using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BCTApp.Contants;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Forms.GoogleMaps;

namespace BCTApp
{
    public class MapPageViewModel : BindableBase, IInitialize
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
        
        public DelegateCommand<MapClickedEventArgs> MapClickedCommand => 
            new DelegateCommand<MapClickedEventArgs>(async(args)=> await MapTappedToCreatePin(args));

        public MapPageViewModel(INavigationService _navigationService,
            IFirebaseHelper firebaseHelper,
            IDialogService dialogService)
        {
            this._navigationService = _navigationService;
            _dialogService = dialogService;
            _firebaseHelper = firebaseHelper;
        }
   
        public async Task MapTappedToCreatePin(MapClickedEventArgs args)
        {
            var pinCount = Pins.Count + 1;
            var pinName = $"Hive {pinCount}";
            var pinLatitude = args.Point.Latitude;
            var pinLongitude = args.Point.Longitude;
            var pin = new Pin()
            {
                Label = pinName,
                Position = new Position(pinLatitude, pinLongitude)
            };
            
            Pins.Add(pin);

            var newHive = new Hive()
            {
                HiveName = pinName,
                HiveLocation = new Location()
                {
                    Latitude = pinLatitude,
                    Longitude = pinLongitude
                }
            };
            
            

            await SaveNewHive(newHive);

            // _dialogService.ShowDialog("TestDialog", new DialogParameters()
            // {
            //     {"CloseOnTap", true}
            // });
        }

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

        private async void GetUserHives(string uid)
        {
            var userHives = await _firebaseHelper.GetAllUserHives(uid);
            foreach (var hive in userHives)
            {
                Pins.Add( new Pin()
                {
                    Label = hive.HiveName,
                    Position = new Position(hive.HiveLocation.Latitude, hive.HiveLocation.Longitude)
                });
                
            }
        }
    }
}    