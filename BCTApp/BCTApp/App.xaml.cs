using System;
using BCTApp.Helpers;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BCTApp
{
    public partial class App : PrismApplication
    {
      
        
        public App(IPlatformInitializer platformInitializer = null) : base(platformInitializer){}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterDialog<TestDialog, TestDialogViewModel>();
            containerRegistry.RegisterDialog<DeleteHiveDialog, DeleteHiveDialogViewModel>();
            containerRegistry.RegisterDialog<CreateNewHivePage, CreateNewHivePageViewModel>();
            containerRegistry.RegisterDialog<MoveHivePage, MoveHivePageViewModel>();
            containerRegistry.Register<IFirebaseHelper, FirebaseHelper>();


        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            if (Settings.IsLoggedIn)
                await NavigationService.NavigateAsync("MapPage");
            else
                await NavigationService.NavigateAsync("LoginPage");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
