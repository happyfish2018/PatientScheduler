using System.Net;
using System.Net.Http;
using System.Web.Http;
using PatientScheduler.Service.DAL;
using PatientScheduler.Service.Models;

namespace PatientScheduler.Service.Controller
{
    public class PatientController : ApiController
    {

        private readonly SchedulerManager _manager = new SchedulerManager();

        
        // GET: api/Patient
        public HttpResponseMessage GetRegisteredPatients()
        {
            var patientList = _manager.GetRegisteredPatients();
            return patientList != null ? Request.CreateResponse(HttpStatusCode.OK, patientList) :
                                                     Request.CreateResponse(HttpStatusCode.NoContent);
        }

        // POST: api/Patient
        public HttpResponseMessage RegisterPatient(Patient patient)
        {
            var isPatientAdded = _manager.RegisterPatient(patient);

            return isPatientAdded
                ? Request.CreateResponse(HttpStatusCode.Created)
                : Request.CreateResponse(HttpStatusCode.BadRequest);

            
        }       
    }
}
