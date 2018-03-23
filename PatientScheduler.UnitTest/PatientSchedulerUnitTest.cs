using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientScheduler.Service.Controller;
using PatientScheduler.Service.Data;
using PatientScheduler.Service.DAL;
using PatientScheduler.Service.Models;


namespace PatientScheduler.UnitTest
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PatientSchedulerUnitTest
    {
        private readonly SchedulerManager _manager = new SchedulerManager();

        [TestInitialize]
        public void InitializeTests()
        {
            PatientSchedulerMockData.CreateMockData();            
        }

        [TestMethod]
        public void RegisterPatient_VerifyReturnFalseWhenFailed()
        {
            Patient newPatient = new Patient
            {
                FirstName = "Paul",
                PatientCondition = new PatientCondition
                {
                    ConditionName = "Flu",
                    TopologyName = "Neck"
                }
            };

            bool isPaitentAdded = _manager.RegisterPatient(newPatient);
            Assert.IsFalse(isPaitentAdded);      
        }

        [TestMethod]
        public void RegisterFluPatient_VerifyReturnTrueWhenSucceeded()
        {
           
            Patient newPatient = new Patient
            {
                FirstName = "Paul",
                PatientCondition = new PatientCondition
                {
                    ConditionName = "Flu"
                }
            };

            bool isPaitentAdded = _manager.RegisterPatient(newPatient);
            Assert.IsTrue(isPaitentAdded);

            //Clear registered patients so that individual unit test has control over patient and consultation data. 
            PatientSchedulerContext.Patients.Clear();
            PatientSchedulerContext.Consultations.Clear();
        }

        [TestMethod]
        public void RegisterCancerPatient_VerifyReturnFalseWhenFailed()
        {
         
            Patient newPatient = new Patient
            {
                FirstName = "Mary",
                PatientCondition = new PatientCondition
                {
                    ConditionName = "Cancer"
                }
            };

            bool isPaitentAdded = _manager.RegisterPatient(newPatient);
            Assert.IsFalse(isPaitentAdded);
            
        }

        [TestMethod]
        public void RegisterCancerPatient_VerifyReturnTrueWhenSucceeded()
        {       
            Patient newPatient = new Patient
            {
                FirstName = "Mary",
                PatientCondition = new PatientCondition
                {
                    ConditionName = "Cancer",
                    TopologyName = "Breast"

                }
            };

            bool isPaitentAdded = _manager.RegisterPatient(newPatient);
            Assert.IsTrue(isPaitentAdded);

            //Clear registered patients so that individual unit test has control over patient and consultation data. 
            PatientSchedulerContext.Patients.Clear();
            PatientSchedulerContext.Consultations.Clear();
          
        }

        [TestMethod]
        public void GetRegisteredPatient_ReturnNonEmptyPatientList()
        {           
            Patient newPatient = new Patient
            {
                FirstName = "Mary",
                PatientCondition = new PatientCondition
                {
                    ConditionName = "Cancer",
                    TopologyName = "Breast"

                }
            };

            bool isPaitentAdded = _manager.RegisterPatient(newPatient);
            Assert.IsTrue(isPaitentAdded);

            var patients = _manager.GetRegisteredPatients().ToList();
            Assert.IsNotNull(patients);            
            Assert.AreEqual(patients.Count, 1);
            Assert.AreEqual(patients[0].FirstName, "Mary");
            Assert.IsNotNull(patients[0].PatientCondition);
            Assert.AreEqual(patients[0].PatientCondition.ConditionName, "Cancer");
            Assert.AreEqual(patients[0].PatientCondition.TopologyName, "Breast");

            //Clear registered patients so that individual unit test has control over patient and consultation data. 
            PatientSchedulerContext.Patients.Clear();
            PatientSchedulerContext.Consultations.Clear();

            
        }

        [TestMethod]
        public void GetConsultations_VerifyConsultationDateCalculation()
        {

            List<Patient> newPatients = CreatTestPatients();
            foreach (bool isPaitentAdded in newPatients.Select(patient => _manager.RegisterPatient(patient)))
            {
                Assert.IsTrue(isPaitentAdded);
            }
                                          
            //At this point, we have three cancer patients registered, two of them needs Advanced treatment machine, and one needs either Advance or Simple 
            //treatment machine. Therefore patient Daisy will have to be scheduled a couple of days from the
            //registered date. 

            var patients = _manager.GetRegisteredPatients().ToList();
            Assert.IsNotNull(patients);
            Assert.AreEqual(patients.Count, 3);

            var consultations = _manager.GetConsultations().ToList();
            Assert.IsNotNull(consultations);
            Assert.AreEqual(consultations.Count, 3);

            Consultation consultation = consultations.First(c => c.Patient.FirstName == "Lily");
            Assert.IsNotNull(consultation);
            Assert.AreEqual((consultation.ConsultationDate - consultation.RegistrationDate).TotalDays, 1);
            
            consultation = consultations.First(c => c.Patient.FirstName == "Poppy");
            Assert.IsNotNull(consultation);
            Assert.AreEqual((consultation.ConsultationDate - consultation.RegistrationDate).TotalDays, 1);

            consultation = consultations.First(c => c.Patient.FirstName == "Daisy");
            Assert.IsNotNull(consultation);
            Assert.AreEqual((consultation.ConsultationDate - consultation.RegistrationDate).TotalDays, 2);
            
            //Clear registered patients so that individual unit test has control over patient and consultation data. 
            PatientSchedulerContext.Patients.Clear();
            PatientSchedulerContext.Consultations.Clear();
        }

        private List<Patient> CreatTestPatients()
        {
            List<Patient> newPatients = new List<Patient>
            {
                 new Patient
                {
                    FirstName = "Lily",
                    PatientCondition = new PatientCondition
                    {
                        ConditionName = "Cancer",
                        TopologyName = "Head&Neck"

                    }
                },
                new Patient
                {
                    FirstName = "Daisy",
                    PatientCondition = new PatientCondition
                    {
                        ConditionName = "Cancer",
                        TopologyName = "Head&Neck"

                    }
                },
                new Patient
                {
                    FirstName = "Poppy",
                    PatientCondition = new PatientCondition
                    {
                        ConditionName = "Cancer",
                        TopologyName = "Breast"

                    }
                }
            };
            return newPatients;
        }
        #region Controller Tests

        [TestMethod]
        public void PatientController_RegisterPatient_PatientRegistrationFailed()
        {
            Patient newPatient = new Patient
            {
                FirstName = "Paul",
                PatientCondition = new PatientCondition
                {
                    ConditionName = "Flu",
                    TopologyName = "Neck"
                }
            };

            var controller = new PatientController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var response = controller.RegisterPatient(newPatient);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void PatientController_GetRegisteredPatients_PatientCreatedSuccessful_PatientsRetrievalOk()
        {
            List<Patient> newPatients = CreatTestPatients();

            var controller = new PatientController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            foreach (var patient in newPatients)
            {
                var response = controller.RegisterPatient(patient);
                Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            }

            var result = controller.GetRegisteredPatients();
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

            //Clear registered patients so that individual unit test has control over patient and consultation data. 
            PatientSchedulerContext.Patients.Clear();
            PatientSchedulerContext.Consultations.Clear();
        }

        [TestMethod]
        public void ConsultationsController_GetConsultations_ConsultationsRetrievalOk()
        {
            List<Patient> newPatients = CreatTestPatients();

            var controller = new PatientController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            foreach (var patient in newPatients)
            {
                var response = controller.RegisterPatient(patient);
                Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            }

            var consulationController = new ConsultationsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var result = consulationController.GetConsultations();
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

            //Clear registered patients so that individual unit test has control over patient and consultation data. 
            PatientSchedulerContext.Patients.Clear();
            PatientSchedulerContext.Consultations.Clear();
        }
        #endregion
    }
}
