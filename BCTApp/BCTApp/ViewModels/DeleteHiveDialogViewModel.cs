using System;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace BCTApp
{
    public class DeleteHiveDialogViewModel : ViewModelBase, IDialogAware
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
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
            IEventAggregator eventAggregator,
            IPageDialogService pageDialogService)
        {
            _firebaseHelper = firebaseHelper;
            _eventAggregator = eventAggregator;
            _pageDialogService = pageDialogService;
            CloseDialogCommand = new DelegateCommand(()=> RequestClose(null));
            DeleteHiveCommand = new DelegateCommand(async () => await DeleteHive());
           
        }
        
        private async Task DeleteHive()
        {
            
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                return;
            }

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

            Message = $"Hive clicked is {Hive.HiveName}";

        }

        public event Action<IDialogParameters> RequestClose;
    }
}