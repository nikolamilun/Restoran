using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Meni
    {
        int id;
        string naziv;
        bool aktivan;

        public Meni(int id, string naziv, bool aktivan)
        {
            Id = id;
            Naziv = naziv;
            Aktivan = aktivan;
        }

        public Meni() { }
        public int Id { get => id; set => id = value; }
        public string Naziv { get => naziv; set => naziv = value; }
        public bool Aktivan { get => aktivan; set => aktivan = value; }
    }
}
