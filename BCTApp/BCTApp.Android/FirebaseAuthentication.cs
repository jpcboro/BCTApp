using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(BCTApp.Droid.FirebaseAuthentication))]
namespace BCTApp.Droid
{
    public class FirebaseAuthentication : IFirebaseAuthentication
    {
      
        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var userId =  user.User.Uid;
                return userId;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch(Exception e)
            {
                return string.Empty;
            }
        }

        public bool SignOut()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
