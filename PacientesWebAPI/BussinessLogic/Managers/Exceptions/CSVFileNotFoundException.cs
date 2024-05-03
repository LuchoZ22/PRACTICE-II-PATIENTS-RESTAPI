using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPB.BussinessLogic.Managers.Exceptions
{
    internal class CSVFileNotFoundException : Exception
    {
        public CSVFileNotFoundException() { }

        public CSVFileNotFoundException(string message) : base(message) { }
    }
}
