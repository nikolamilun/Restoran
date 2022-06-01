using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Jelo
    {
        int id;
        string naziv;
        string opis;

        public Jelo(int id, string naziv, string opis)
        {
            Id = id;
            Naziv = naziv;
            Opis = opis;
        }

        public Jelo() { }

        public int Id { get => id; set => id = value; }
        public string Naziv { get => naziv; set => naziv = value; }
        public string Opis { get => opis; set => opis = value; }
    }
}
