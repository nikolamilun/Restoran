using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    // Klasa za bacanje izuzetaka klijentu pomocu servera
    public class Izuzetak
    {
        // Poruka koja se ispisuje
        string poruka;

        public Izuzetak(string poruka)
        {
            Poruka = poruka;
        }

        public string Poruka { get => poruka; set => poruka = value; }
    }
}
