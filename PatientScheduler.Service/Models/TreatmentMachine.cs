using System.ComponentModel.DataAnnotations;

namespace PatientScheduler.Service.Models
{
    /// <summary>
    /// Treatment machine object 
    /// </summary>
    public class TreatmentMachine
    {
        
        /// <summary>
        /// A treatment machine's unique name
        /// </summary>
        [Required]
        public string MachineName { get; set; }

        
        /// <summary>
        /// Machine capability
        /// </summary>
        public string MachineCapability { get; set; }
    }
}