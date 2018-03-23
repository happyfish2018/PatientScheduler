using System.Collections.Generic;
using PatientScheduler.Service.BusinessRules;
using PatientScheduler.Service.Data;
using PatientScheduler.Service.Models;

namespace PatientScheduler.Service.DAL
{
    public class SchedulerManager : ISchedulerManager
    {

        
        /// <summary>
        /// Return a list of registered patients
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Patient> GetRegisteredPatients()
        {
            return PatientSchedulerContext.Patients;
        }

        /// <summary>
        /// Return a list of consultations.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Consultation> GetConsultations()
        {
            return PatientSchedulerContext.Consultations;
        }

        /// <summary>
        /// New patient registration
        /// </summary>
        /// <param name="patient"></param>
        /// <returns>true if patient is registered successfully; otherwise false</returns>
        public bool RegisterPatient(Patient patient)
        {
            ValidationRules rule = new ValidationRules();

            if (!rule.PatientValidation(patient))
                return false;            

            if (PatientSchedulerContext.Patients == null)           
                PatientSchedulerContext.Patients = new List<Patient>();

            PatientSchedulerContext.Patients.Add(patient);

            ConsultationScheduler scheduler = new ConsultationScheduler();
            Consultation consultation = scheduler.SchedulePatientConsultation(patient);

            if (consultation == null)
                return false;

            if (PatientSchedulerContext.Consultations == null)
                PatientSchedulerContext.Consultations = new List<Consultation>();
            
                PatientSchedulerContext.Consultations.Add(consultation);
            
            return true;
        }
    }
}
