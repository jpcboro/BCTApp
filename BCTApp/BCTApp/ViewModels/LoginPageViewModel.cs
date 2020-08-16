using System;
using System.Threading.Tasks;
using BCTApp.Contants;
using BCTApp.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace BCTApp
{
    public class LoginPageViewModel: BindableBase
    {
        private readonly INavigationService _navigationService;

        private IFirebaseAuthentication _firebaseAuth;
        
        public  DelegateCommand LoginCommand => new DelegateCommand( ()=>  ExecuteLoginCommandAsync());

        private string _email;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public LoginPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();
            
        }
        
        private  async Task ExecuteLoginCommandAsync()
        {
            string uid = await _firebaseAuth.LoginWithEmailAndPassword(Email, Password);
            if (uid != String.Empty)
            {
                Settings.UserEmail = Email;
                Settings.UID = uid;
                var navParams = new NavigationParameters();
                navParams.Add(ParameterConstants.UID, uid);
                await _navigationService.NavigateAsync(PageConstants.MapPage, navParams);

            }

        }

       
    }
}