using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Prism;
using Prism.Mvvm;
using Prism.Navigation;

namespace BCTApp
{
    public class ViewModelBase : BindableBase, INavigationAware
    {
        private bool _isSaving = false;

        public bool IsSaving
        {
            get { return _isSaving; }
            set { SetProperty(ref _isSaving, value); }

        }

        private bool _isControlVisible = true;

        public bool IsControlVisible
        {
            get { return _isControlVisible; }
            set { SetProperty(ref _isControlVisible, value); }
        }
        
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }
        
       
    }
}