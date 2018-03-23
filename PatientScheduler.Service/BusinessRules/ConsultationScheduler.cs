using System;
using System.Collections.Generic;
using System.Linq;
using PatientScheduler.Service.Data;
using PatientScheduler.Service.Models;

namespace PatientScheduler.Service.BusinessRules
{
    public class ConsultationScheduler
    {
        public Consultation SchedulePatientConsultation(Patient patient)
        {

            var registrationDate = DateTime.Now.Date;
            // consulation date must be after registration date.                        
            var consultationStartDate = CalculateConsultationDate(registrationDate);
                       
            while (consultationStartDate.Day > 0)
            {
                //Get a list of consultations that are happening on the projected consultation day.
                List<Consultation> listOfConsultations = new List<Consultation>();
                if (PatientSchedulerContext.Consultations != null)               
                    listOfConsultations.AddRange(PatientSchedulerContext.Consultations.Where(consultation => consultation.ConsultationDate == consultationStartDate));
                                             
                //List of doctors that is available on the projected consultation day. 
                List<Doctor> avaliableDoctors =
                        new List<Doctor>(FindAvailableDoctors(patient.PatientCondition, listOfConsultations));

                //List of treatment rooms that is available on the projected consultation day.
                List<TreatmentRoom> avaliableTreatmentRooms =
                    new List<TreatmentRoom>(FindAvailableTreatmentRooms(patient.PatientCondition, listOfConsultations));

                //Create consultation use the first available docotor and treatment room. 
                if (avaliableDoctors.Any() && avaliableTreatmentRooms.Any())
                {
                    var consultation = new Consultation
                    {
                        ConsultationDate = consultationStartDate,
                        RegistrationDate = registrationDate,
                        Doctor = avaliableDoctors.First(),
                        Room = avaliableTreatmentRooms.First(),
                        Patient = patient
                    };
                   
                    return consultation;
                }
               
                //If we are here, it means that no doctor and/or treamtent room is available. Then move on to the next day.                 
                consultationStartDate = CalculateConsultationDate(consultationStartDate);
            }

            return null;
        }

        /// <summary>
        /// Calculate consultation date
        /// </summary>
        /// <param name="registrationDate"></param>
        /// <returns></returns>
        private DateTime CalculateConsultationDate(DateTime registrationDate)
        {
            // consulation date must be after registration date.             
            var daysInMonth = DateTime.DaysInMonth(registrationDate.Year, registrationDate.Month);
            var consultationStartDate = registrationDate;
            //If current day is not the last day of the month, safely add one more day. 
            if (registrationDate.Day < daysInMonth)
                consultationStartDate = registrationDate.AddDays(1);
            else
            {
                // if current month is not december, safely forward to next month's first day;
                // otherwise, forward to next year's first day. 
                int month = consultationStartDate.Month;
                consultationStartDate = month < 12 ? new DateTime(consultationStartDate.Year, consultationStartDate.Month + 1, 1) : new DateTime(consultationStartDate.Year + 1, 1, 1);
            }

            return consultationStartDate;            
        }

        private List<Doctor> FindAvailableDoctors(PatientCondition patientCondition, List<Consultation> scheduledConsultations)
        {
            if (patientCondition == null)
                return null;

            string doctorType = GetDoctorRoleBasedOnPatientCondition(patientCondition);
            
            List<Doctor> scheduledDoctors = new List<Doctor>();
            if (scheduledConsultations != null && scheduledConsultations.Count > 0)
                scheduledDoctors = new List<Doctor>(scheduledConsultations.Select(a => a.Doctor).ToList());

           return
                PatientSchedulerContext.Doctors.Except(scheduledDoctors)
                    .Where(doc => doc.Roles != null && doc.Roles.Contains(doctorType))
                    .ToList();
        

        }

        /// <summary>
        /// Get a list of available treatment rooms based on patient condition
        /// 
        /// </summary>
        /// <param name="patientCondition"></param>
        /// <param name="scheduledConsultations"></param>
        /// <returns></returns>
        private List<TreatmentRoom> FindAvailableTreatmentRooms(PatientCondition patientCondition,
            List<Consultation> scheduledConsultations)
        {
            if (patientCondition == null)
                return null;

            var scheduledTreatmentRooms = new List<TreatmentRoom>();

            if (scheduledConsultations != null)
                scheduledTreatmentRooms = new List<TreatmentRoom>(scheduledConsultations.Select(r => r.Room).ToList());

            List<TreatmentMachine> machines = GetTreatmentMachineBasedOnPatientCondition(patientCondition);

            // If patient has flu, a treatment without machine is preferred. 
            if (patientCondition.ConditionName.Equals(Properties.Resource.Flu, StringComparison.OrdinalIgnoreCase))
            {
                var availableRooms = new List<TreatmentRoom>(
                PatientSchedulerContext.TreatmentRooms.Except(scheduledTreatmentRooms)
                    .Where(t => t.TreatmentMachine == null || string.IsNullOrEmpty(t.TreatmentMachine.MachineName))
                    .ToList());
                // then any available treatment
                if (!availableRooms.Any())
                    availableRooms =
                        new List<TreatmentRoom>(
                            PatientSchedulerContext.TreatmentRooms.Except(scheduledTreatmentRooms).ToList());

                return availableRooms;
            }

            List<TreatmentRoom> returnRooms = new List<TreatmentRoom>();
            var rooms = PatientSchedulerContext.TreatmentRooms.Except(scheduledTreatmentRooms);
            foreach (var room in rooms)
            {
                returnRooms.AddRange(from m in machines
                                     where
                                         room.TreatmentMachine.MachineName.Equals(m.MachineName,
                                             StringComparison.OrdinalIgnoreCase)
                                     select room);
            }

            return returnRooms;
        }


        /// <summary>
        /// Get doctor role based on patient's condition
        /// </summary>
        /// <param name="patientCondition"></param>
        /// <returns></returns>
        private string GetDoctorRoleBasedOnPatientCondition(PatientCondition patientCondition)
        {

            string doctorRole = string.Empty;
            if (patientCondition.ConditionName.Equals(Properties.Resource.Flu, StringComparison.OrdinalIgnoreCase))
                doctorRole = Properties.Resource.GeneralPractitioner;
            else if (patientCondition.ConditionName.Equals(Properties.Resource.Cancer, StringComparison.OrdinalIgnoreCase))
                doctorRole = Properties.Resource.Oncologist;

            return doctorRole;
        }


        /// <summary>
        /// Get treatment machine capability based on patient condition.
        /// If a patient has cancer, and its topology is breast, machine can be either simple or advanced; therefore return value will just be empty string. 
        /// </summary>
        /// <param name="patientCondition"></param>
        /// <returns></returns>
        private List<TreatmentMachine> GetTreatmentMachineBasedOnPatientCondition(PatientCondition patientCondition)
        {
            if (patientCondition == null || PatientSchedulerContext.TreatmentMachines == null)
                return null;

            //Flu patient doesn't care about treatment machine. 
            if (patientCondition.ConditionName.Equals(Properties.Resource.Flu, StringComparison.OrdinalIgnoreCase))
                return PatientSchedulerContext.TreatmentMachines;
                        
            //Cancer patient has Head & Neck topology needs to have an Advanced machine.
            if (patientCondition.TopologyName.Equals(Properties.Resource.HeadNeck, StringComparison.OrdinalIgnoreCase))
            {
                return
                    PatientSchedulerContext.TreatmentMachines.Where(
                        t =>
                                !string.IsNullOrEmpty(t.MachineCapability) &&
                                t.MachineCapability.Equals(Properties.Resource.Advanced, StringComparison.OrdinalIgnoreCase)).ToList();
            }
                
            // All other cancer patients can have either Simple or Advanced machine.            
            return
                PatientSchedulerContext.TreatmentMachines.Where(
                    t =>
                        !string.IsNullOrEmpty(t.MachineCapability) && (
                        t.MachineCapability.Equals(Properties.Resource.Advanced, StringComparison.OrdinalIgnoreCase) || 
                        t.MachineCapability.Equals(Properties.Resource.Simple, StringComparison.OrdinalIgnoreCase))).ToList();                
                                        
        }        
    }
}
