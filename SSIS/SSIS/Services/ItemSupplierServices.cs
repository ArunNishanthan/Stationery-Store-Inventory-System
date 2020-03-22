using SSIS.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace SSIS.Services
{
    public class ItemSupplierServices
    {
        private SSISDbContext dbContext;

        public ItemSupplierServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #region Amy's code parth
        public List<ItemSupplier> GetLowStockItemSuppliers()
        {
            return dbContext.ItemSuppliers.Include(i => i.Supplier)
                                             .Include(w => w.Item)
                                             .Where(m => m.Item.CurrentQuantity <= m.Item.ReorderLevel).ToList();
        }

        public Supplier GetSupplier(Supplier supplier)
        {
            return dbContext.Suppliers.SingleOrDefault(sup => sup.SupplierCode == supplier.SupplierCode);
        }
        #endregion

        public List<ItemSupplier> GetItemSuppliersbyItem(string itemCode)
        {
            return dbContext.ItemSuppliers.Include(i => i.Supplier)
                .Include(w => w.Item)
                .Where(m => m.Item.ItemCode == itemCode).ToList();
        }

        public ItemSupplier GetSupplierbyId(int id)
        {
            return dbContext.ItemSuppliers.Include(m => m.Item).Include(m => m.Supplier).SingleOrDefault(m => m.ItemSupplierId == id);
        }

        public List<Supplier> GetAllSuppliers()
        {
            return dbContext.Suppliers.ToList();
        }

        public List<ItemSupplier> GetItemSuppliersBySupplierCode(string code)
        {
            return dbContext.ItemSuppliers.Include(i => i.Item).Where(i => i.Supplier.SupplierCode == code).ToList();
        }
    }

}
