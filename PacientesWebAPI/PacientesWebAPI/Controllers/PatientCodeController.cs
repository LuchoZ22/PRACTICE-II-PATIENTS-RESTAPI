using Microsoft.AspNetCore.Mvc;
using UPB.BussinessLogic.Managers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PacientesWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientCodeController : ControllerBase
    {
     
        private PatientManager _patientManager;

        public PatientCodeController(PatientManager patientManager)
        { 
            _patientManager = patientManager;
        }

        // PUT api/<PatientCodeController>/
        //Gives the patients their code
        [HttpPut]
        public void Put()
        {
            _patientManager.GetPatientsCodes();
        }

        
    }
}
