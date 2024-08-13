using System;

namespace Corima.Services
{
    public class RepositoryService
    {
        public void Save(string data)
        {
            Console.WriteLine($"Data ({data}) was saved");
        }
    }
}