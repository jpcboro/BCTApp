using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BCTApp
{
    public class SampleDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private string _message;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private Hive _hive;

        public Hive Hive
        {
            get { return _hive; }
            set { SetProperty(ref _hive, value); }
        }

        public SampleDialogViewModel(IFirebaseHelper firebaseHelper)
        {
            _firebaseHelper = firebaseHelper;
            CloseDialogCommand = new DelegateCommand(()=> RequestClose(null));
            DeleteHiveCommand = new DelegateCommand(async () => await DeleteHive());
           
        }
        
        private async Task DeleteHive()
        {
            RequestClose(new DialogParameters{{"deleted", Hive}});

            await _firebaseHelper.DeleteBeeHive(Settings.UID, Hive);

        }

        public DelegateCommand DeleteHiveCommand { get; set; }
        public DelegateCommand CloseDialogCommand { get; set; }
        
        
        


        public bool CanCloseDialog() => true;
        
        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
            Hive = parameters.GetValue<Hive>("message");

            Message = $"Pin clicked is {Hive.HiveName}";

        }

        public event Action<IDialogParameters> RequestClose;
    }
}