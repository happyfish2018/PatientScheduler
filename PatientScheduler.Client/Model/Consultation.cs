using System;

namespace PatientSchedulerClient.Model
{
    /// <summary>
    /// Patient consultation
    /// </summary>
    public class Consultation
    {       

        /// <summary>
        /// Patient
        /// </summary>
        public Patient Patient { get; set; }
      
        /// <summary>
        /// Doctor
        /// </summary>
        public Doctor Doctor { get; set; }
     
        /// <summary>
        /// Treatment room
        /// </summary>
        public TreatmentRoom Room { get; set; }

        /// <summary>
        /// Patient registration date
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Consultation date
        /// </summary>
        public DateTime ConsultationDate { get; set; }                

    }
}