using System.Globalization;
using System.Linq;
using PatientScheduler.Service.Data;
using PatientScheduler.Service.Models;

namespace PatientScheduler.Service.BusinessRules
{
    public class ValidationRules
    {
        /// <summary>
        /// Validate patient to make sure all required information is present. 
        /// Patient needs to have a name AND a condition         
        /// </summary>
        /// <param name="patient"></param>
        /// <returns>true = patient information is valid; false = patient information is invalid</returns>
        public bool PatientValidation(Patient patient)
        {            
            // patient information can not be null
            if (patient == null)
                return false;

            // patient must have first name
            if (string.IsNullOrEmpty(patient.FirstName))
                return false;

            // patient must have a condition
            if (patient.PatientCondition == null)
                return false;

            // When patient condition is Cancer, it must have a topology associated. 
            // When patient condition is Flu, it shouldn't have a topology associated. 
            switch(patient.PatientCondition.ConditionName)
            {
                case "Cancer":
                    if (string.IsNullOrEmpty(patient.PatientCondition.TopologyName))
                        return false;
                    break;
                case "Flu":
                    if (!string.IsNullOrEmpty(patient.PatientCondition.TopologyName))
                        return false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Validate Treatment machine to make sure all required information is present
        /// A treatment machine needs to have a unique name AND a capability
        /// </summary>
        /// <param name="machine"></param>
        /// <returns>true = treatment machine information is valid; false = treatment machine information is invalid</returns>
        public bool TreatmentMachineValidation(TreatmentMachine machine)
        {
            // treatment machine information can not be null
            if (machine == null)
                return false;

            // treatment machine must have a name.
            if (string.IsNullOrEmpty(machine.MachineName))
                return false;

            // treamtent machine must have a unique name. 
            if (PatientSchedulerContext.TreatmentMachines != null)
            {
                return
                    PatientSchedulerContext.TreatmentMachines.All(
                        treatmentMachine =>
                            CultureInfo.CurrentCulture.CompareInfo.Compare(treatmentMachine.MachineName,
                                machine.MachineName, CompareOptions.IgnoreCase) != 0);
            }

            // treatment machine must have a capablity
            if (string.IsNullOrEmpty(machine.MachineCapability))
                return false;
                            
            return true;
        }

        /// <summary>
        /// Validate Treatment room to make sure all required information is present
        /// A treatment room needs to have a unique name
        /// </summary>
        /// <param name="room"></param>
        /// <returns>true = treatment room information is valid; false = treatment room information is invalid</returns>
        public bool TreatmentRoomValidation(TreatmentRoom room)
        {
            // treatment room information can not be null
            if (room == null)
                return false;

            // treatment room must have a name.
            if (string.IsNullOrEmpty(room.RoomName))
                return false;

            // treamtent room must have a unique name. 
            if (PatientSchedulerContext.TreatmentRooms != null)
            {
                return
                    PatientSchedulerContext.TreatmentRooms.All(
                       treatmentRoom =>
                            CultureInfo.CurrentCulture.CompareInfo.Compare(treatmentRoom.RoomName,
                                room.RoomName, CompareOptions.IgnoreCase) != 0);
            }

            return true;
        }

        /// <summary>
        /// Validate doctor to make sure all required information is present
        /// A doctor needs to have a unique name and at lease one role
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns>true = doctor information is valid; false = doctor information is invalid</returns>
        public bool DoctorValidation(Doctor doctor)
        {
            // doctor information can not be null
            if (doctor == null)
                return false;

            // doctor must have a name.
            if (string.IsNullOrEmpty(doctor.FirstName))
                return false;

            // doctor must have at least one role.
            if (doctor.Roles == null || doctor.Roles.Count <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Validate consultation to make sure all required information is present
        /// Rules: 
        /// A consultation must happen with a doctor in a treatment room
        /// Cancer patients must see an Oncologist and Flu patients must see a general practitioner
        /// Cancer patient with Neck&Head topology must be seen in a treatment room with "Advanced" treatment machine
        /// Cancer patient with Breast topology must be seen in a treament romm with "Advance" or "Simple" treatment machine
        /// Consultation cannot be scheduled on the same day as the patient is registered 
        /// </summary>
        /// <param name="consultation"></param>
        /// <returns>true = consultation information is valid; false = consultation information is invalid</returns>
        public bool ConsultatonValidation(Consultation consultation)
        {
            // consultation can not be null
            if (consultation == null)
                return false;

            // Validate on the content of the consultation
            if (PatientValidation(consultation.Patient) == false ||
                DoctorValidation(consultation.Doctor) == false ||
                TreatmentRoomValidation(consultation.Room) == false ||
                TreatmentMachineValidation(consultation.Room.TreatmentMachine) == false)
                return false;
                
            // Flu patient must see a genernal practioner
            if (consultation.Patient.PatientCondition.ConditionName == Properties.Resource.Flu)
            {
                if (consultation.Doctor.Roles != null && !consultation.Doctor.Roles.Contains(Properties.Resource.GeneralPractitioner))
                    return false;
            }            
            else if (consultation.Patient.PatientCondition.ConditionName == Properties.Resource.Cancer)
            {
                // Cancer patient must see a oncologist
                if (consultation.Doctor.Roles != null && !consultation.Doctor.Roles.Contains(Properties.Resource.Oncologist))
                    return false;

                // Cancer patient with HeadNeck topology must be seen in a room with an Advanced treatment machine
                if (consultation.Patient.PatientCondition.TopologyName == Properties.Resource.HeadNeck && 
                    consultation.Room.TreatmentMachine.MachineCapability != Properties.Resource.Advanced)
                    return false;

                // Cancer patient with breast topology must be seen in a room with an Advanced or simple treatment machine
                if (consultation.Patient.PatientCondition.TopologyName == Properties.Resource.Breast && 
                    (string.IsNullOrEmpty(consultation.Room.TreatmentMachine.MachineCapability) ||
                     (consultation.Room.TreatmentMachine.MachineCapability != Properties.Resource.Advanced &&
                     consultation.Room.TreatmentMachine.MachineCapability != Properties.Resource.Simple)))
                    return false;
                
            }

            // Consultation date must be after the registration date
            if (consultation.RegistrationDate >= consultation.ConsultationDate)
                return false;

            return true;
        }
    }
}
