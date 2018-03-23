using System.Collections.Generic;


namespace PatientSchedulerClient.Model
{
    public class Doctor
    {
        
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Doctor's roles 
        /// </summary>
        public List<string> Roles { get; set; } 


    }
}