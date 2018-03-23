using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using PatientScheduler.Service.Models;

namespace PatientScheduler.Service.Data
{
    public static class PatientSchedulerContext
    {
        public static List<TreatmentMachine> TreatmentMachines { get; set; }
        public static List<TreatmentRoom> TreatmentRooms { get; set; }
        public static List<Doctor> Doctors { get; set; }
        public static List<Patient> Patients { get; set; }
        public static List<Consultation> Consultations { get; set; } 

        public static void SeedFromResourceJson(string dataFilePath)
        {
            if (string.IsNullOrEmpty(dataFilePath))
                return;

            var data = JObject.Parse(File.ReadAllText(dataFilePath));

            //Get Treatment machines
            var treatmentMachines = data.GetValue("TreatmentMachines").ToList();
            TreatmentMachines = new List<TreatmentMachine>();
            foreach(var machine in treatmentMachines)
            {                
                var name = ((JObject)machine).GetValue("Name").ToString();
                var capability = ((JObject)machine).GetValue("Capability").ToString();
                TreatmentMachines.Add(new TreatmentMachine
                {
                    MachineName = name,
                    MachineCapability = capability
                });
            }

            //Get Treatment rooms
            var treatmentRooms = data.GetValue("TreatmentRooms").ToList();
            TreatmentRooms = new List<TreatmentRoom>();
            foreach (var room in treatmentRooms)
            {
                var name = ((JObject)room).GetValue("Name").ToString();
                var machine = ((JObject)room).GetValue("TreatmentMachine");
                                      
                TreatmentRooms.Add(new TreatmentRoom
                {
                    RoomName = name,
                    TreatmentMachine = new TreatmentMachine
                    {
                        MachineName = machine == null ? string.Empty : machine.ToString()                     
                    }
                    
                });
            }

            //Get Doctors
            var doctors = data.GetValue("Doctors").ToList();
            Doctors = new List<Doctor>();
            foreach (var doc in doctors)
            {
                var name = ((JObject)doc).GetValue("Name").ToString();
                var roles = ((JObject)doc).GetValue("Roles").ToList().Select(r => r.ToString());
              
                Doctors.Add(new Doctor
                {
                    FirstName = name,
                    Roles = new List<string>(roles)
                });
            }


        }
            
    }
}
