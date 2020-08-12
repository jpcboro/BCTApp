
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Xamarin.Forms;

[assembly:Dependency(typeof(BCTApp.iOS.FirebaseAuthentication))]
namespace BCTApp.iOS
{
    public class FirebaseAuthentication : IFirebaseAuthentication
    {
        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return user.User.Uid;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}