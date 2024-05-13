using Microsoft.Extensions.Configuration;
using UPB.BussinessLogic.Models;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using UPB.BussinessLogic.Managers.Exceptions;
using Serilog;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;


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
            try
            {
                using (var reader = new StreamReader(fileLocation))
                using (var csv_reader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return csv_reader.GetRecords<PatientModel>().ToList();
                }
            }
            catch (Exception ex) 
            {
                CSVFileNotFoundException csv_exception = new CSVFileNotFoundException(ex.Message);
                Log.Error(csv_exception.LogMessage("GETALL"));
                Log.Error("StackTrace: " + ex.StackTrace);

                throw csv_exception;
            }

        }

        public PatientModel GetByCI(string ci) 
        {

            PatientModel? p  = GetAll().FirstOrDefault( x => x.CI == ci);
            if (p != null)
                return p;
            else
            {
                NonFoundPatientException nonFoundEx = new NonFoundPatientException();
                Log.Error($"The Patient with the CI: {ci} was not found");
                throw nonFoundEx;

            }

               
        }

        public void CreatePatient(PatientModel patient)
        {
            try 
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };

                using (var stream = File.Open(fileLocation, FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecord(patient);
                }
            }
            catch(IOException ex) 
            {
                CSVFileNotFoundException csv_exception = new CSVFileNotFoundException(ex.Message);
                Log.Error(csv_exception.LogMessage("CREATEPATIENT"));
                Log.Error("StackTrace: " + ex.StackTrace);

                throw csv_exception;
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
                NonFoundPatientException nonFoundEx = new NonFoundPatientException();
                Log.Error($"The Patient with the CI: {ci} was not found");
                throw nonFoundEx;
            }
        }

        public void UpdatePatient(string ci, PatientUpdateModel patient)
        {

            var patients = GetAll();
            var personToUpdate = patients.FirstOrDefault(x => x.CI == ci);
            if (personToUpdate != null)
            {

                personToUpdate.Name = patient.Name;
                personToUpdate.LastName = patient.LastName;
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
            try
            {
                using (var writer = new StreamWriter(fileLocation))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.WriteRecords(patients);
                }
            }
            catch (IOException ex) 
            {
                CSVFileNotFoundException csv_exception = new CSVFileNotFoundException(ex.Message);
                Log.Error(csv_exception.LogMessage("WRITECSV"));
                Log.Error("StackTrace: " + ex.StackTrace);

                throw csv_exception;
            }
            

        }


        public async void GetPatientsCodes()
        {


            var patients = GetAll();
            HttpClient client = new HttpClient();
            foreach (var patient in patients)
            {
                try
                {
                    
                    HttpResponseMessage response = await client.GetAsync($"{_configuration.GetConnectionString("PatientsCodeAPI")}/{patient.CI}");
                    if (response.IsSuccessStatusCode)
                    {
                        string patientCode = await response.Content.ReadAsStringAsync();
                        Log.Information($"{patientCode}");
                        patient.Code = patientCode;
                    }
                    else
                    {
                        NonFoundPatientException nonFoundEx = new NonFoundPatientException();
                        Log.Error($"The Patient with the CI: {patient.CI} was not found");
                        throw nonFoundEx;
                    }
                }
                catch (Exception ex) 
                {
                    FailedToGetDataException fgde = new FailedToGetDataException(ex.Message);
                    Log.Error("Failed to get code for the patient");
                    throw fgde;
                }
            }
            WriteCSV(patients);
        }

    }
}
