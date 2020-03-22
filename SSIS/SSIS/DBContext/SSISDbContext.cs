using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace SSIS.Models
{

    public class SSISDbContext : IdentityDbContext<ApplicationUser>
    {
        public SSISDbContext()
            : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }


        public DbSet<AdjustmentVoucher> AdjustmentVouchers { get; set; }
        public DbSet<CollectionPoint> CollectionPoints { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DisburseItem> DisburseItems { get; set; }
        public DbSet<DepDisbursementList> DisbursementLists { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemSupplier> ItemSuppliers { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<RequestItem> RequestItems { get; set; }
        public DbSet<RequisationForm> RequisationForms { get; set; }
        public DbSet<StockCard> StockCards { get; set; }
        public DbSet<StorePerson> StorePersons { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Delegation> Delegations { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public static SSISDbContext Create()
        {
            return new SSISDbContext();
        }
    }
}
