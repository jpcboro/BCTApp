using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BCTApp.Helpers;
using BCTApp.Models;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;

namespace BCTApp
{
    public class HiveListPageViewModel : ViewModelBase, IActiveAware, INavigationAware, IInitialize
    {
        private readonly IFirebaseHelper _firebaseHelper;
        private readonly IPageDialogService _pageDialogService;


        private UpdateHiveListEvent _event;

        public HiveListPageViewModel(IFirebaseHelper firebaseHelper,
            IEventAggregator eventAggregator,
            IPageDialogService pageDialogService)
        {
            _firebaseHelper = firebaseHelper;
            _pageDialogService = pageDialogService;

            UserHives = new ObservableCollection<Hive>();

            UpdateHiveListEvent updateHiveListEvent = eventAggregator.GetEvent<UpdateHiveListEvent>();

            updateHiveListEvent.Subscribe(RefreshHiveList);
        }

        private ObservableCollection<Hive> _userHives;

        public ObservableCollection<Hive> UserHives
        {
            get { return _userHives; }
            set { SetProperty(ref _userHives, value); }
        }
        public event EventHandler IsActiveChanged;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
            
        }
        async void RefreshHiveList(bool shouldRefresh)
        {
            if (shouldRefresh)
            {
            
                    var userHivesList = await _firebaseHelper.GetAllUserHives(Settings.UID);
                
                    UserHives.Clear();
                
                    foreach (var hive in userHivesList)
                    {
                        UserHives.Add(hive);
                    }

            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public void Initialize(INavigationParameters parameters)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _pageDialogService.DisplayAlertAsync("No Internet", "Please check your internet connection and try again.", "Ok");
                IsControlVisible = false;
            }
            else
            {
                IsControlVisible = true;
            }
        }
    }
}