using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BCTApp
{
    public class CreateNewHivePageViewModel : BindableBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;

        private string _hiveName;

        public string HiveName
        {
            get { return _hiveName; }
            set { SetProperty(ref _hiveName, value); }
        }

        public CreateNewHivePageViewModel(IFirebaseHelper firebaseHelper)
        {
            _firebaseHelper = firebaseHelper;
            
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

             await _firebaseHelper.AddHive(Settings.UID, NewHive);

             RequestClose(new DialogParameters()
             {
                 {"savedHive", NewHive}
             });
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