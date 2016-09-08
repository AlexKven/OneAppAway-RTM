using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public class RetrievedData<T>
    {
        public RetrievedData()
        {
            HasData = false;
        }

        public RetrievedData(string errorMessage)
            :this()
        {
            ErrorMessage = errorMessage;
        }

        public RetrievedData(string errorMessage, Exception caughtException)
            : this(errorMessage)
        {
            CaughtException = caughtException;
        }

        public RetrievedData(Exception caughtException)
            : this(caughtException.Message, caughtException) { }

        public RetrievedData(T data)
        {
            HasData = true;
            Data = data;
        }

        public RetrievedData(T data, string errorMessage)
            : this(data)
        {
            ErrorMessage = errorMessage;
        }

        public RetrievedData(T data, string errorMessage, Exception caughtException)
            : this(data, errorMessage)
        {
            CaughtException = caughtException;
        }

        public RetrievedData(T data, Exception caughtException)
            : this(data, caughtException.Message, caughtException) { }

        public T Data { get; }
        public bool HasData { get; }
        public string ErrorMessage { get; }
        public Exception CaughtException { get; }
    }
}
