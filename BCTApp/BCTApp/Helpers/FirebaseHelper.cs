using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCTApp.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace BCTApp
{
    public class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://bctappmap.firebaseio.com/");
        
        public async Task AddItem(string name, string userId)
        {
            await firebase.Child($"items/{userId}")
                .PostAsync(new Item() {Name = name});
        }

        public async Task<List<Item>> GetAllUserItems(string userId)
        {
            var objectItems = await firebase.Child($"items/{userId}").OnceAsync<Item>();
            
            return objectItems.Select(x=> new Item()
            {
                Name = x.Object.Name
            }).ToList();
        }
    }
}