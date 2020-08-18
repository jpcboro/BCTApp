using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace BCTApp
{
    public class MoveHivePageViewModel : ViewModelBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IPageDialogService _pageDialogService;

        public MoveHivePageViewModel(IFirebaseHelper firebaseHelper,
            IPageDialogService pageDialogService)
        {
            _firebaseHelper = firebaseHelper;
            _pageDialogService = pageDialogService;

            CloseDialogCommand = new DelegateCommand(()=> RequestClose(new DialogParameters()
            {
                {"moveCanceled", true}
            }));
            MoveHiveCommand = new DelegateCommand(() => UpdateHivePosition());
        }

        public DelegateCommand MoveHiveCommand { get; set; }

        private async Task UpdateHivePosition()
        {
            
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                return;
            }
            
            IsSaving = true;
            IsControlVisible = false;
            await _firebaseHelper.UpdateBeeHive(Settings.UID, MovedHive);
           
            
            RequestClose(new DialogParameters()
            {
                {"moveCanceled", false}
            });
            
            
            IsSaving = false;
            IsControlVisible = true;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
         
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

            MovedHive = parameters.GetValue<Hive>("movedHive");

            HiveName = MovedHive.HiveName;

        }

        public Hive MovedHive { get; set; }

        private string _hiveName;

        public string HiveName
        {
            get { return _hiveName; }
            set { SetProperty(ref _hiveName, value); }
        }

       
        public DelegateCommand CloseDialogCommand { get; set; }

        public event Action<IDialogParameters> RequestClose;
    }
}