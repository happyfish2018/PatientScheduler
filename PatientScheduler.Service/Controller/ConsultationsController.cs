using System.Net;
using System.Net.Http;
using System.Web.Http;
using PatientScheduler.Service.DAL;

namespace PatientScheduler.Service.Controller
{
    public class ConsultationsController : ApiController
    {
        private readonly SchedulerManager _manager = new SchedulerManager();


        // GET: api/Consultations
        public HttpResponseMessage GetConsultations()
        {
            var consultationList = _manager.GetConsultations();
            return consultationList != null ? Request.CreateResponse(HttpStatusCode.OK, consultationList) :
                                                     Request.CreateResponse(HttpStatusCode.NoContent);
        }      
    }
}