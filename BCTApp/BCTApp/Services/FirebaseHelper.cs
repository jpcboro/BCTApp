using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BCTApp.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Polly;

namespace BCTApp
{
    public class FirebaseHelper : IFirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://bctappmap.firebaseio.com/");
        
        public async Task AddHive(string userId, Hive hive)
        {
             await Policy.Handle<HttpRequestException>(ex =>
                {
                    Debug.WriteLine($"{ex.Message}");
                    return true;
                })
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) =>
                    {
                        Console.WriteLine($"Something went wrong: {ex.Message}, retrying...");
                    }
                ).ExecuteAsync(async () =>  await firebase.Child($"BeeHives/{userId}")
                    .PostAsync(new Hive()
                    {
                        HiveName = hive.HiveName,
                        HiveLocation = hive.HiveLocation
                    }));

           
        }
        
        
        public async Task<List<Hive>> GetAllUserHives(string userId)
        {
            var hives = await Policy.Handle<HttpRequestException>(ex =>
                {
                    Debug.WriteLine($"{ex.Message}");
                    return true;
                })
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) =>
                    {
                        Console.WriteLine($"Something went wrong: {ex.Message}, retrying...");
                    }
                ).ExecuteAsync(async () => await firebase.Child($"BeeHives/{userId}")
                    .OnceAsync<Hive>());
          
            
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

        public async Task DeleteBeeHive(string userId,  Hive hive)
        {
            var hiveToDelete = (await firebase.Child($"BeeHives/{userId}")
                .OnceAsync<Hive>()).Where(a => a.Object.HiveName == hive.HiveName).FirstOrDefault();

            await firebase.Child($"BeeHives/{userId}").Child(hiveToDelete.Key).DeleteAsync();

        }
    }
}