using System;
using System.Threading.Tasks;
using BCTApp.Contants;
using BCTApp.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using DependencyService = Xamarin.Forms.DependencyService;

namespace BCTApp
{
    public class LoginPageViewModel : ViewModelBase, IInitialize
    {
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;

        private IFirebaseAuthentication _firebaseAuth;

        public DelegateCommand LoginCommand => new DelegateCommand(async () => await ExecuteLoginCommandAsync());


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
            set { SetProperty(ref _password, value);}
        }

        public bool IsEnabled => !string.IsNullOrEmpty(_email);

        public LoginPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

            _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();

            if (!string.IsNullOrEmpty(Settings.UserEmail))
            {
                Email = Settings.UserEmail;
            }
            
        }

    

        private  async Task ExecuteLoginCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Please fill up username or password", "Ok");

                return;

            }

            IsSaving = true;
            string uid = await _firebaseAuth.LoginWithEmailAndPassword(Email, Password);
            if (uid != String.Empty)
            {
                Settings.UserEmail = Email;
                Settings.UID = uid;
               
                await _navigationService.NavigateAsync(PageConstants.MainTabbedPage);
                
            }
            else
            {
                await _pageDialogService.DisplayAlertAsync("Alert", "Wrong email or password", "Ok");
            }

            IsSaving = false;
        }
        
        public void Initialize(INavigationParameters parameters)
        {

           
        }
    }
}