using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Sastojak
    {
        int id;
        string naziv;
        string opis;

        public Sastojak(int id, string naziv, string opis)
        {
            this.id = id;
            this.naziv = naziv;
            this.opis = opis;
        }

        public Sastojak() { }

        public int Id { get => id; set => id = value; }
        public string Naziv { get => naziv; set => naziv = value; }
        public string Opis { get => opis; set => opis = value; }
    }
}
