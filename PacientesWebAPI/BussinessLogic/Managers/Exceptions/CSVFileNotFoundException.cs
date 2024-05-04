namespace UPB.BussinessLogic.Managers.Exceptions
{
    public class CSVFileNotFoundException : Exception
    {
        public CSVFileNotFoundException() { }

        public CSVFileNotFoundException(string message) : base(message) { }

        public string LogMessage(string method)
        {
            return "Method: " + method + " Message: " + base.Message;
        }
    }
}
