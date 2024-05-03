using Microsoft.AspNetCore.Mvc;
using UPB.BussinessLogic.Models;
using UPB.BussinessLogic.Managers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PacientesWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly PatientManager _patientManager;
        public PatientController(PatientManager _patientManager) 
        { 
            this._patientManager = _patientManager;
        }


        // GET: api/<PatientController>
        [HttpGet]
        public IEnumerable<PatientModel> Get()
        {
            return _patientManager.GetAll();
        }

        // GET api/<PatientController>/5
        [HttpGet("{ci}")]
        public PatientModel Get(string ci)
        {
            return _patientManager.GetByCI(ci);
        }

        // POST api/<PatientController>
        [HttpPost]
        public void Post([FromBody] PatientModel value)
        {
            _patientManager.CreatePatient(value);
        }

        // PUT api/<PatientController>/5
        [HttpPut("{ci}")]
        public void Put(string ci, [FromBody] PatientModel value)
        {
            _patientManager.UpdatePatient(ci, value);
        }

        // DELETE api/<PatientController>/5
        [HttpDelete("{ci}")]
        public void Delete(string ci)
        {
            _patientManager.DeletePatient(ci);
        }
    }
}
