using System;
using BCTApp.Contants;
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
            containerRegistry.RegisterForNavigation<MainTabbedPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<HiveListPage, HiveListPageViewModel>();
            containerRegistry.RegisterDialog<DeleteHiveDialog, DeleteHiveDialogViewModel>();
            containerRegistry.RegisterDialog<CreateNewHivePage, CreateNewHivePageViewModel>();
            containerRegistry.RegisterDialog<MoveHivePage, MoveHivePageViewModel>();
            containerRegistry.Register<IFirebaseHelper, FirebaseHelper>();
            
        }

        protected override async void OnInitialized()
        {
            Device.SetFlags(new[] { "Brush_Experimental" });

            InitializeComponent();



            if (Settings.IsLoggedIn)
            {
                await NavigationService.NavigateAsync(PageConstants.MainTabbedPage);
            }
            else
            {
                await NavigationService.NavigateAsync(PageConstants.LoginPage);

            }
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
