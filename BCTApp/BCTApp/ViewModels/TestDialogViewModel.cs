using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BCTApp
{
    public class TestDialogViewModel : BindableBase, IDialogAware
    {
        private string _question;

        public string Question
        {
            get { return _question; }
            set { SetProperty(ref _question, value); }
        }

        private bool _canClose;

        public bool CanClose
        {
            get { return _canClose; }
            set { SetProperty(ref _canClose, value); }
        }

        private bool _closeOnTap;

        public bool CloseOnTap
        {
            get { return _closeOnTap; }
            set { SetProperty(ref _closeOnTap, value); }
        }
        
        public  DelegateCommand CloseCommand { get; }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => CanClose;
        
        public TestDialogViewModel()
        {
                
        }
      
 
        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("CloseOnTap"))
            {
                CloseOnTap = CanClose = parameters.GetValue<bool>("CloseOnTap");
            }
        }

        
    }
}