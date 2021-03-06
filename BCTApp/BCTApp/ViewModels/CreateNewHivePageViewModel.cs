using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;
using Location = BCTApp.Models.Location;

namespace BCTApp
{
    public class CreateNewHivePageViewModel : ViewModelBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IPageDialogService _pageDialogService;

        private string _hiveName;

        public string HiveName
        {
            get { return _hiveName; }
            set { SetProperty(ref _hiveName, value); }
        }

        public CreateNewHivePageViewModel(IFirebaseHelper firebaseHelper,
                                           IPageDialogService pageDialogService)
        {
            _firebaseHelper = firebaseHelper;
            _pageDialogService = pageDialogService;

            CloseDialogCommand = new DelegateCommand(()=> RequestClose(null));
            SaveNewHiveCommand = new DelegateCommand( () => SaveNewHive());
        }

        private async Task SaveNewHive()
        {
             NewHive = new Hive()
            {
                HiveName = HiveName,
                HiveLocation = NewHiveLocation

            };

             if (Connectivity.NetworkAccess != NetworkAccess.Internet)
             {
                 await _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                 return;
             }
             
             IsSaving = true;
             IsControlVisible = false;
             
             await _firebaseHelper.AddHive(Settings.UID, NewHive);

             RequestClose(new DialogParameters()
             {
                 {"savedHive", NewHive}
             });
             
             IsSaving = false;
             IsControlVisible = true;
        }

        public Hive NewHive { get; set; }

        public DelegateCommand CloseDialogCommand { get; set; }
        public DelegateCommand SaveNewHiveCommand { get; set; }
        
        public Location NewHiveLocation { get; set; }
      
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            NewHiveLocation = parameters.GetValue<Location>("map_clicked");
        }

        public event Action<IDialogParameters> RequestClose;
    }
}