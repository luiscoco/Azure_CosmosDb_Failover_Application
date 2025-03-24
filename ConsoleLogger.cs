using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosFailoverApp
{
    public class ConsoleLogger
    {
        public void Info(string msg) => Console.WriteLine($"[INFO] {msg}");
        public void Error(string msg) => Console.WriteLine($"[ERROR] {msg}");
    }
}
