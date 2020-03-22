using SSIS.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSIS.Services
{
    public class DelegationServices
    {
        private SSISDbContext dbContext;

        public DelegationServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Delegation SaveDelegation(Delegation delegation)
        {
            var delegationDb = dbContext.Delegations.Add(delegation);
            dbContext.SaveChanges();
            return delegationDb;
        }

        public List<Delegation> GetDelegationsbyDep(string depCode)
        {
            return dbContext.Delegations.Include(m => m.DelegatedTo).Where(m => m.DelegatedTo.DepartmentCode == depCode).OrderByDescending(m => m.FromDate).ToList();
        }
        public Employee GetDelegatedTo(int delId)
        {
            return dbContext.Delegations.Include(m => m.DelegatedTo).SingleOrDefault(m => m.Id == delId).DelegatedTo;
        }

        public void CancelDelegation(int id)
        {
            var delgation = dbContext.Delegations.SingleOrDefault(m => m.Id == id);
            dbContext.Delegations.Remove(delgation);
            dbContext.SaveChanges();
        }

    }
}
