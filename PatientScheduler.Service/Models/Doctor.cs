using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientScheduler.Service.Models
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
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Doctor's roles 
        /// </summary>
        public List<string> Roles { get; set; } 


    }
}