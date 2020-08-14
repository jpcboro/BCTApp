using System;
using Xamarin.Essentials;

namespace BCTApp.Helpers
{
    public static class Settings
    {
        private const string _userEmailKey = "email";
        private static readonly string _userEmailDefault = String.Empty;
        private const string _userIdKey = "uid";
        private static readonly string _userIdDefault = String.Empty;

        public static string UserEmail
        {
            get { return Preferences.Get(_userEmailKey, _userEmailDefault);}
            set { Preferences.Set(_userEmailKey, value);}
        }
        
        public static string UID
        {
            get { return Preferences.Get(_userIdKey, _userIdDefault);}
            set { Preferences.Set(_userIdKey, value);}
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(UID);

    }
}