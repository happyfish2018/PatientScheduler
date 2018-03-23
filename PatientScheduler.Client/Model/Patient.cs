

namespace PatientSchedulerClient.Model
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
        public string FirstName { get; set; }

        /// <summary>
        /// patient condition
        /// </summary>
        public PatientCondition PatientCondition { get; set; }
    }
}