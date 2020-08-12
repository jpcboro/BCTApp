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

        public async Task AddHive(string userId, Hive hive)
        {
            await firebase.Child($"BeeHives/{userId}")
                .PostAsync(new Hive()
                {
                    HiveName = hive.HiveName,
                    HiveLocation = hive.HiveLocation
                });
        }

        public async Task<List<Item>> GetAllUserItems(string userId)
        {
            var objectItems = 
                await firebase.Child($"items/{userId}")
                    .OnceAsync<Item>();
            
            return objectItems.Select(x=> new Item()
            {
                Name = x.Object.Name
            }).ToList();
        }
        
        public async Task<List<Hive>> GetAllUserHives(string userId)
        {
            var hives = 
                await firebase.Child($"BeeHives/{userId}")
                    .OnceAsync<Hive>();
            
            return hives.Select(x=> new Hive()
            {
                HiveName = x.Object.HiveName,
                HiveLocation = x.Object.HiveLocation
            }).ToList();
        }

        public async Task UpdateBeeHive(string userId, Hive hive)
        {
            var toUpdateHive = (await firebase
                .Child($"BeeHives/{userId}")
                .OnceAsync<Hive>())
                .FirstOrDefault(x => x.Object.HiveName == hive.HiveName);

            await firebase.Child($"BeeHives/{userId}")
                .Child(toUpdateHive.Key)
                .PutAsync(new Hive()
                {
                    HiveName = hive.HiveName,
                    HiveLocation = hive.HiveLocation
                });
        }
    }
}