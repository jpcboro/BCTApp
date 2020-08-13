using System;
using Xamarin.Essentials;

namespace BCTApp.Helpers
{
    public static class Settings
    {
        private const string _userEmailKey = "email";
        private static readonly string _userEmailDefault = String.Empty;
        private const string _userPassword = "password";
        private static readonly string _userPasswordDefault = String.Empty;

        public static string UserEmail
        {
            get { return Preferences.Get(_userEmailKey, _userEmailDefault);}
            set { Preferences.Set(_userEmailKey, value);}
        }
        
        public static string UserPassword
        {
            get { return Preferences.Get(_userPassword, _userPasswordDefault);}
            set { Preferences.Set(_userPassword, value);}
        }
        
    }
}