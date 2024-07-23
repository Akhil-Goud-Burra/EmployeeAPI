using System;

namespace Employee_API.Logging
{
    public class Logging: ILogging
    {
        public void Log(string message, String type)
        {
            if (type == "error") { Console.WriteLine("Error: " + message); }

            else { Console.WriteLine(message); }
        }
    }
}
