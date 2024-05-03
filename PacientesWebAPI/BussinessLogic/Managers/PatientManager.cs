using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPB.BussinessLogic.Models;
using CsvHelper;
using System.Globalization;
using System.Collections.Immutable;
using CsvHelper.Configuration;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using UPB.BussinessLogic.Managers.Exceptions;
using Serilog;


namespace UPB.BussinessLogic.Managers
{
    public class PatientManager
    {
        private readonly IConfiguration _configuration;
        private string fileLocation;
        

        public PatientManager(IConfiguration conf) 
        {
            _configuration = conf;
            fileLocation = _configuration.GetConnectionString("CSVFile");
        }

        public List<PatientModel> GetAll()
        {
            using(var reader = new StreamReader(fileLocation))
            using( var csv_reader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv_reader.GetRecords<PatientModel>().ToList();
            }
           
                
        }

        public PatientModel GetByCI(string ci) 
        {

            PatientModel? patient =  GetAll().FirstOrDefault( x => x.CI == ci);

            if (patient != null)
                return patient;
            else
            {
                Log.Error($"Patient with the CI: {ci} was not found");
                throw new NonFoundPatientException();
                
            }
               
        }

        public void CreatePatient(PatientModel patient)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false};

            using (var stream = File.Open(fileLocation, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecord(patient);
            }
        }

        public void DeletePatient(string ci) 
        {

            var patients = GetAll();
            var personToRemove = patients.FirstOrDefault(x => x.CI == ci);
            if (personToRemove != null)
            {
                patients.Remove(personToRemove);
                WriteCSV(patients);
            }

            else
            {
                Log.Error($"Patient with the CI: {ci} was not found");
                throw new NonFoundPatientException();
            }
        }

        public void UpdatePatient(string ci, PatientModel updatedPatient)
        {

            var patients = GetAll();
            var personToUpdate = patients.FirstOrDefault(x => x.CI == ci);
            if (personToUpdate != null)
            {

                personToUpdate.Name = updatedPatient.Name;
                personToUpdate.LastName = updatedPatient.LastName;
                WriteCSV(patients);
            }
            else
            {
                Log.Error($"Patient with the CI: {ci} was not found");
                throw new NonFoundPatientException();
            }
               
            
            
        }

        private void WriteCSV(IEnumerable<PatientModel> patients)
        {
            using (var writer = new StreamWriter(fileLocation))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(patients);
            }

        }
    }
}
