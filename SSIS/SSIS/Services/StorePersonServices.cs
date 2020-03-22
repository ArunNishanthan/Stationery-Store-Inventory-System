using SSIS.Models;
using System.Collections.Generic;
using System.Linq;

namespace SSIS.Services
{
    public class StorePersonServices
    {
        private SSISDbContext dbContext;
        public StorePersonServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public StorePerson GetStorePerson(int userId)
        {
            return dbContext.StorePersons.SingleOrDefault(sp => sp.UserId == userId);
        }

        public List<StorePerson> GetStorePerson()
        {
            User user = new User
            {
                Role = Enums.Role.STORE_CLERK
            };
            return dbContext.StorePersons.Where(s => s.Role == user.Role).ToList();
        }

    }
}
