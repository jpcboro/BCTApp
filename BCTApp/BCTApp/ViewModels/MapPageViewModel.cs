using System.Threading.Tasks;
using BCTApp.Contants;
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
            _dialogService.ShowDialog("TestDialog", new DialogParameters()
            {
                {"CloseOnTap", true}
            });
        }
        public void Initialize(INavigationParameters parameters)
        {
            _uid = parameters.GetValue<string>(ParameterConstants.UID);;
        }
    }
}    