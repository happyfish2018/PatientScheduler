using System.ComponentModel.DataAnnotations;

namespace PatientScheduler.Service.Models
{
    public class Patient     
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
        /// patient condition
        /// </summary>
        public PatientCondition PatientCondition { get; set; }
    }
}