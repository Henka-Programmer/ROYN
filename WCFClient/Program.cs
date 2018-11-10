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
    internal class Program
    {
        private static long getDataSize(object obj)
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
            for (int i = 0; i <= 100; i += 10)
            {
                RunIteration(service, i);
            }

            Console.ReadKey();
        }

        private static void RunIteration(IProductsService service, int size)
        {
            var stopWatch = Stopwatch.StartNew();
            var products = service.GetProducts(size);
            stopWatch.Stop();

            System.Console.WriteLine($"Getting '{products.Count}' product by traditional way takes: {stopWatch.ElapsedMilliseconds} - data size:{getDataSize(products)} bytes");

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
                .OrderBy(x => x.Id)
                .Skip(0)
                .Take(size);

            var roynresult = service.Execute(query);

            var products2 = roynresult.GetResult<List<Product>>();
            stopWatch2.Stop();
            System.Console.WriteLine($"Getting '{products2.Count}' product by royn way takes: {stopWatch2.ElapsedMilliseconds} - data size:{getDataSize(roynresult)} bytes");
        }
    }
}