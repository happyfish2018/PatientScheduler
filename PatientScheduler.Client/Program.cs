using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using PatientSchedulerClient.Model;
using PatientSchedulerClient.Properties;

namespace PatientSchedulerClient
{
    public class Program
    {
        static void Main(string[] args)
        {                                             

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Resource.BaseAddress);
                  
                    //Flu patient registered correctly. 
                    Patient newPatient1 = new Patient
                    {
                        FirstName = "Mary",
                        PatientCondition = new PatientCondition
                        {
                            ConditionName = "Flu"
                        }
                    };

                  
                    var httpContent = new StringContent(JsonConvert.SerializeObject(newPatient1), Encoding.UTF8, "application/json");

                    var postResponse = client.PostAsync("api/Patient", httpContent).Result;
                   
                    Console.WriteLine(Resource.ClientReceived, postResponse);

                    // Cancer patient registered successfully. 
                    Patient newPatient2 = new Patient
                    {
                        FirstName = "Joe",
                        PatientCondition = new PatientCondition
                        {
                            ConditionName = "Cancer",
                            TopologyName = "Breast"
                        }
                    };

                    httpContent = new StringContent(JsonConvert.SerializeObject(newPatient2), Encoding.UTF8, "application/json");

                    postResponse = client.PostAsync("api/Patient", httpContent).Result;

                    Console.WriteLine(Resource.ClientReceived, postResponse);

                    // Patient registration failed.
                    Patient newPatient3 = new Patient
                    {
                        FirstName = "Jimmy",
                        PatientCondition = new PatientCondition
                        {
                            ConditionName = "Flu",
                            TopologyName = "Breast"
                        }
                    };

                    httpContent = new StringContent(JsonConvert.SerializeObject(newPatient3), Encoding.UTF8, "application/json");
                    postResponse = client.PostAsync("api/Patient", httpContent).Result;
                    Console.WriteLine(Resource.ClientReceived, postResponse);

                    //GetRegisteredPatients
                    var getPatientsResponse = client.GetAsync("api/Patient").Result;                                       
                    Console.WriteLine(Resource.ClientReceived, getPatientsResponse);

                    //GetConsultations
                    var getConsultationsResponse = client.GetAsync("api/Consultations").Result;
                    Console.WriteLine(Resource.ClientReceived, getConsultationsResponse);
                    Console.WriteLine("Press Enter to quit");
                    Console.ReadLine();


                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Exception occurred: " + ex.InnerException);
            }
        }
    }
}
