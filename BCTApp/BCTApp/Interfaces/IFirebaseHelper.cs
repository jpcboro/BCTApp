using System.Collections.Generic;
using System.Threading.Tasks;
using BCTApp.Models;

namespace BCTApp
{
    public interface IFirebaseHelper
    {
        Task AddHive(string userId, Hive hive);

        Task<List<Hive>> GetAllUserHives(string userId);

        Task UpdateBeeHive(string userId, Hive hive);
        
        Task DeleteBeeHive(string userId, Hive hive);
    }
}