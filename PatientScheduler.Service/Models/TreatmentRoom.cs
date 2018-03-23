using System.ComponentModel.DataAnnotations;

namespace PatientScheduler.Service.Models
{
    public class TreatmentRoom
    {
        

        /// <summary>
        /// A treatment room's unique name
        /// </summary>
        [Required]      
        public string RoomName { get; set; }

       
        /// <summary>
        /// Navigation property
        /// </summary>
        public TreatmentMachine TreatmentMachine { get; set; }
    }
}