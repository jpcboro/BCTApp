using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BCTApp
{
    public class MoveHivePageViewModel : BindableBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;

        public MoveHivePageViewModel(IFirebaseHelper firebaseHelper)
        {
            _firebaseHelper = firebaseHelper;
            
            CloseDialogCommand = new DelegateCommand(()=> RequestClose(null));
            MoveHiveCommand = new DelegateCommand(() => UpdateHivePosition());
        }

        public DelegateCommand MoveHiveCommand { get; set; }

        private async Task UpdateHivePosition()
        {
            await _firebaseHelper.UpdateBeeHive(Settings.UID, MovedHive);

            RequestClose(null);
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