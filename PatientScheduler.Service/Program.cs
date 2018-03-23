using System;
using System.IO;
using System.Web.Http;
using System.Web.Http.SelfHost;
using PatientScheduler.Service.Data;

namespace PatientScheduler.Service
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            
            const string baseAddress = "http://localhost:8002";
            var config = new HttpSelfHostConfiguration(baseAddress);
  
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            //Get current directory's parent path.
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;

            if (directoryInfo == null)
                return;

            var filePath = Path.Combine(directoryInfo.FullName, @"Data\DataResources.json");
            //If not such file found, abort
            if (!File.Exists(filePath))
                return;
            
            PatientSchedulerContext.SeedFromResourceJson(filePath);

            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Service started at " + baseAddress);
                Console.WriteLine("Press Enter to stop the service.");
                Console.ReadLine();
            }
        }
    }
}
