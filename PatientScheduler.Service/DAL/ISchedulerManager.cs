using System.Collections.Generic;
using PatientScheduler.Service.Models;

namespace PatientScheduler.Service.DAL
{
    public interface ISchedulerManager
    {
        /// <summary>
        /// Get a list of all registered patients.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Patient> GetRegisteredPatients();

        /// <summary>
        /// Get a list of all scheduled consultations
        /// </summary>
        /// <returns></returns>
        IEnumerable<Consultation> GetConsultations();

        /// <summary>
        /// Register a patient and then create a consultation
        /// </summary>
        /// <returns>true when registration is successful; false when registration failed</returns>
        bool RegisterPatient(Patient patient);


    }
}
