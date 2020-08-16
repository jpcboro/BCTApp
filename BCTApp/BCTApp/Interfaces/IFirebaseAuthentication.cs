using System.Threading.Tasks;

namespace BCTApp
{
    public interface IFirebaseAuthentication
    {
        Task<string> LoginWithEmailAndPassword(string email, string password);

        bool SignOut();
    }
}