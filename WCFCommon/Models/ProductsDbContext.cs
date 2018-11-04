using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System.Data.Entity;

namespace WCFCommon
{
    public class DbInitializer : CreateDatabaseIfNotExists<ProductsDbContext>
    {
        protected override void Seed(ProductsDbContext context)
        {
            for (int i = 0; i < 1000; i++)
            {
                context.Products.Add(new Product
                {
                    Name = $"Products {i}",
                    Barcode = $"Barcode {i}",
                    Category = new ProductCategory { Name = $"Category {i}", ParentCategory = new ProductCategory { Name = $"Parent Category {i}{i}" } },
                    Color = $"Color {i}",
                    Cost = i,
                    PurchasesUoM = new MeasureUnit { Name = $"P.UoM {i}", Category = new MeasureUnitCategory { Name = $"P.UoM Category" } },
                    SaleUoM = new MeasureUnit { Name = $"S.UoM {i}", Category = new MeasureUnitCategory { Name = $"S.UoM Category" } },
                    Weight = i,
                    Volum = i * 2,
                    MaxQuantity = i * i,
                    MinQuantity = i,
                });
            }

            context.SaveChanges();
        }
    }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext() : base(new MySqlConnection("server=localhost;port=3306;database=royn_profiling;uid=root;password=gamadev"), true)
        {
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new DbInitializer());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<MeasureUnit> MeasureUnits { get; set; }

        public DbSet<MeasureUnitCategory> GetMeasureUnitCategories { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }
    }
}