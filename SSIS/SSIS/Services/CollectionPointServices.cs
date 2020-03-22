using SSIS.Models;
using System.Collections.Generic;
using System.Linq;

namespace SSIS.Services
{
    public class CollectionPointServices
    {
        private SSISDbContext dbContext;

        public CollectionPointServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<CollectionPoint> GetCollectionPoint()
        {
            return dbContext.CollectionPoints.ToList();
        }
        public CollectionPoint GetOneCollectionPoint(int collectionpointid)
        {
            return dbContext.CollectionPoints.SingleOrDefault(m => m.CollectionPointId == collectionpointid);
        }
        public CollectionPoint GetCollectionPointbyName(string name)
        {
            return dbContext.CollectionPoints.SingleOrDefault(m => m.Place == name);
        }
    }
}
