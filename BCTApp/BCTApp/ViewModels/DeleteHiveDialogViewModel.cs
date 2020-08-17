using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BCTApp
{
    public class DeleteHiveDialogViewModel : ViewModelBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IEventAggregator _eventAggregator;
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

        public DeleteHiveDialogViewModel(IFirebaseHelper firebaseHelper,
            IEventAggregator eventAggregator)
        {
            _firebaseHelper = firebaseHelper;
            _eventAggregator = eventAggregator;
            CloseDialogCommand = new DelegateCommand(()=> RequestClose(null));
            DeleteHiveCommand = new DelegateCommand(async () => await DeleteHive());
           
        }
        
        private async Task DeleteHive()
        {

            IsSaving = true;
            IsControlVisible = false;
            await _firebaseHelper.DeleteBeeHive(Settings.UID, Hive);
            
            _eventAggregator.GetEvent<UpdateHiveListEvent>().Publish(true);
            
            RequestClose(new DialogParameters{{"deleted", Hive}});
            
            IsSaving = false;
            IsControlVisible = true;

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