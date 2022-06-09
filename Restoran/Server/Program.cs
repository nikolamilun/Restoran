using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.ServiceModel;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Otvara se host za servis
                using (ServiceHost host = new ServiceHost(typeof(RestoranServer)))
                {
                    // Dodaje se endpoint na koji se povezuje klijent
                    host.AddServiceEndpoint(typeof(IRestoran), new BasicHttpBinding(), new Uri("http://localhost:8000"));
                    // Host se otvara
                    host.Open();
                    Console.WriteLine("Server je pokrenut!");
                    Console.ReadKey();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Pokretanje servera nije uspelo! Razlog: " + exc.Message);
            }
        }
    }
}
