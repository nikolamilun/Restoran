using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Data.SqlClient;

namespace Server
{
    class RestoranServer : IRestoran
    {
        static string connString = @"Data Source=LIMUNPC/NIKOLASRVSQL; Initial Catalog=Restoran; Integrated Security=true;";
        public List<Jelo> jelaNaMenijimaKojiSuAktivniSaCenom(int cena)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                List<Jelo> izlaz = new List<Jelo>();
                SqlCommand cmd = new SqlCommand("SELECT j.* FROM Jelo AS j, Meni AS m, NaMeniju AS nm WHERE m.Aktivan = 1 AND m.id_menija = nm.id_menija AND nm.id_jela = j.id_jela AND nm.cena <= " + cena + ";");
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    izlaz.Add(new Jelo(sdr.GetInt32(0), sdr.GetString(1), sdr.GetString(2)));
                }
                return izlaz;
            }
        }

        public List<Jelo> jelaPremaBrojuSastojaka(int brSastojaka)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {

            }
        }

        public List<Jelo> jelaPremaMeniju(int idMenija)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {

            }
        }

        public List<Jelo> jelaPremaSastojku(int idSastojka)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {

            }
        }

        public DataSet PreuzmiDataSet()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {

            }
        }

        public bool vratiDataSet(DataSet podaciDataSet)
        {
            throw new NotImplementedException();
        }
    }
}
