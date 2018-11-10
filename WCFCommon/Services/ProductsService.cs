using ROYN;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;

namespace WCFCommon.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class ProductsService : RoynService, IProductsService
    {
        static ProductsService()
        {
            using (var ctx = new ProductsDbContext())
            {
                if (!ctx.Database.Exists())
                {
                    ctx.Database.Initialize(true);
                }
            }
        }

        public List<Product> GetProducts(int size)
        {
            try
            {
                using (var ctx = new ProductsDbContext())
                {
                    var r = ctx.Products
                        .Include(x => x.PurchasesUoM.Category)
                        .Include(x => x.SaleUoM.Category)
                        .Include(x => x.Category.ParentCategory).OrderBy(x => x.Id).Skip(0).Take(size).ToList();
                    return r;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public override RoynResult Execute(RoynRequest request)
        {
            using (var ctx = new ProductsDbContext())
            {
                return ctx.Execute(request); ;
            }
        }
    }
}