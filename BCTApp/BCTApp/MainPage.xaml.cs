using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BCTApp
{
    public partial class MainPage : ContentPage
    {
        private IFirebaseAuthentication _firebaseAuth;
        public MainPage()
        {
            InitializeComponent();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            var hasUserSaved = Preferences.ContainsKey("email");
            if (hasUserSaved)
            {
                entryEmail.Text = Preferences.Get("email", String.Empty);
                entryPassword.Text = Preferences.Get("password", String.Empty);
            }
            
        }

        private  async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var userEmail = entryEmail.Text;
            var userPassword = entryPassword.Text;

            string uid = await _firebaseAuth.LoginWithEmailAndPassword(userEmail, userPassword);
            if (uid != string.Empty)
            {
                Preferences.Set("email", userEmail);
                Preferences.Set("password", userPassword);
                
                // await  Navigation.PushAsync(new MapPage(uid));

            }
        }
    }
}
