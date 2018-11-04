using System;
using WCFClient;
using WCFCommon;
using WCFCommon.Services;

namespace WCFHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var host = new ServiceHost<ProductsService, IProductsService>(new DefaultBasicHttpBinding(), new Uri("http://localhost:80/royn")))
            {
                host.Open();

                Console.Write("Service started");
                Console.ReadKey();
                host.Close();
            }
        }
    }
}