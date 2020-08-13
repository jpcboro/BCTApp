using System.Threading.Tasks;
using BCTApp.Contants;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Forms.GoogleMaps;

namespace BCTApp
{
    public class MapPageViewModel : BindableBase, IInitialize
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IDialogService _dialogService;
        private string _uid;

        // public MapPageViewModel(IFirebaseHelper firebaseHelper, IDialogService dialogService)
        // {
        //     _firebaseHelper = firebaseHelper;
        //     _dialogService = dialogService;
        // }
        //
        //
        // public void Initialize(INavigationParameters parameters)
        // {
        //     _uid = parameters.GetValue<string>(ParameterConstants.UID);;
        // }
        //
        public async Task CreatePin(MapClickedEventArgs args)
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