using ROYN;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;
using WCFCommon;
using WCFCommon.Services;

namespace WCFClient
{
    public class ProductsServiceClient : ClientBase<IProductsService>
    {
        public ProductsServiceClient() : base(new BasicHttpBinding(), new EndpointAddress("http://localhost/productsService"))
        {
        }

        public List<Product> GetProducts()
        {
            return Channel.GetProducts();
        }

        public RoynResult Execute(RoynRequest roynRequest)
        {
            return Channel.Execute(roynRequest);
        }
    }

    internal class Program
    {
        private static long getSize(object obj)
        {
            DataContractSerializer ds = new DataContractSerializer(obj.GetType());

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ds.WriteObject(ms, obj);

                return ms.Length;
            }
        }

        private static void Main(string[] args)
        {
            BasicHttpBinding basicHttpBinding = new DefaultBasicHttpBinding();

            var serviceUrl = "http://localhost:80/royn";

            var endpoint = new EndpointAddress(serviceUrl);

            var channelFactory = new ChannelFactory<IProductsService>(basicHttpBinding, endpoint);

            IProductsService service = channelFactory.CreateChannel(endpoint);
            var stopWatch = Stopwatch.StartNew();
            var products = service.GetProducts();
            stopWatch.Stop();

            System.Console.WriteLine($"Getting '{products.Count}' product by traditional way takes: {stopWatch.ElapsedMilliseconds} - data size:{getSize(products)} bytes");

            var stopWatch2 = Stopwatch.StartNew();
            var query = new RoynRequest<Product>()
                .Add(x => x.Name)
                .Add(x => x.SaleUoM.Name)
                .Add(x => x.SaleUoM.Id)
                .Add(x => x.PurchasesUoM.Name)
                .Add(x => x.PurchasesUoM.Id)
                .Add(x => x.Category.Name)
                .Add(x => x.Category.Id)
                .Add(x => x.Category.ParentCategory.Name)
                .Add(x => x.Category.ParentCategory.Id)
                .Add(x => x.SaleUoM.Category.Name)
                .Add(x => x.SaleUoM.Category.Id)
                .Add(x => x.PurchasesUoM.Category.Name)
                .Add(x => x.PurchasesUoM.Category.Id)
                .Add(x => x.Weight)
                .Add(x => x.Volum)
                .Add(x => x.SoldInPOS)
                .Add(x => x.SaleUoMId)
                .Add(x => x.PurchasesUoMId)
                ;

            var roynresult = service.Execute(query);

            var products2 = roynresult.GetResult<List<Product>>();
            stopWatch2.Stop();
            System.Console.WriteLine($"Getting '{products2.Count}' product by royn way takes: {stopWatch2.ElapsedMilliseconds} - data size:{getSize(roynresult)} bytes");

            Console.ReadKey();
        }
    }
}