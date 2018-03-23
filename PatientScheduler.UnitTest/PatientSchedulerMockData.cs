using System.IO;
using PatientScheduler.Service.Data;

namespace PatientScheduler.UnitTest
{
    public static class PatientSchedulerMockData
    {
        public static void CreateMockData()
        {
            //Get current directory's parent path.
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;

            if (directoryInfo == null)
                return;

            var filePath = Path.Combine(directoryInfo.FullName, @"MockData.json");
            
            if (File.Exists(filePath))
                PatientSchedulerContext.SeedFromResourceJson(filePath);
                
        }                    
    }
}
