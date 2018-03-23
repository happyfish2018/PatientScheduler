

namespace PatientSchedulerClient.Model
{
    public class TreatmentRoom
    {
        

        /// <summary>
        /// A treatment room's unique name
        /// </summary>     
        public string RoomName { get; set; }

       
        /// <summary>
        /// Navigation property
        /// </summary>
        public TreatmentMachine TreatmentMachine { get; set; }
    }
}