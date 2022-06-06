using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Data.SqlClient;
using System.ServiceModel;

namespace Server
{
    class RestoranServer : IRestoran
    {
        public static string connString = @"data source=LIMUNPC\NIKOLASRVSQL;initial catalog=Restoran;trusted_connection=true";
        
        // Sluzi za vracanje lista jela po SQL komandi
        public List<Jelo> jelaPoUnesenojKomandi(string cmdText)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    List<Jelo> izlaz = new List<Jelo>();
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        izlaz.Add(new Jelo(sdr.GetInt32(0), sdr.GetString(1), sdr.GetString(2)));
                    }
                    return izlaz;
                }
            }
            catch (Exception exc)
            {
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }

        public List<Jelo> jelaNaMenijimaKojiSuAktivniSaCenom(int cena)
        {
            return jelaPoUnesenojKomandi("SELECT j.* FROM Jelo AS j, Meni AS m, NaMeniju AS nm WHERE m.Aktivan = 1 AND m.id_menija = nm.id_menija AND nm.id_jela = j.id_jela AND nm.cena <= " + cena + ";");
        }

        public List<Jelo> jelaPremaBrojuSastojaka(int brSastojaka)
        {
            return jelaPoUnesenojKomandi("SELECT j.*, COUNT(ss.id_sastojka) FROM Jelo AS j, SeSastoji AS ss, Sastojci AS s WHERE j.id_jela = ss.id_jela AND s.id_sastojka = ss.id_sastojka GROUP BY j.* HAVING COUNT(ss.id_sastojka) >= " + brSastojaka + ";") ;
        }

        public List<Jelo> jelaPremaMeniju(int idMenija)
        {
            return jelaPoUnesenojKomandi("SELECT j.* FROM jelo AS j, meni AS m, naMeniju AS nm WHERE j.id_jela = nm.id_jela AND nm.id_menija = " + idMenija + ";");
        }

        public List<Jelo> jelaPremaSastojku(int idSastojka)
        {
            return jelaPoUnesenojKomandi("SELECT j.* FROM jelo AS J, sastojci AS s, seSastoji AS ss WHERE j.id_jela = ss.id_jela AND ss.id_sastojka = " + idSastojka + ";");
        }

        public DataSet PreuzmiDataSet()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // SQLDataAdapteri za svaku tabelu posebno
                    SqlDataAdapter sdaJelo = new SqlDataAdapter("SELECT * FROM jelo", conn);
                    SqlDataAdapter sdaMeni = new SqlDataAdapter("SELECT * FROM meni", conn);
                    SqlDataAdapter sdaSastojci = new SqlDataAdapter("SELECT * FROM sastojci", conn);
                    SqlDataAdapter sdaNaMeniju = new SqlDataAdapter("SELECT * FROM naMeniju", conn);
                    SqlDataAdapter sdaSeSastoji = new SqlDataAdapter("SELECT * FROM seSastoji", conn);

                    DataSet ds = new DataSet();

                    // Popunjavanje DataSeta podacima
                    sdaJelo.Fill(ds, "jelo");
                    sdaMeni.Fill(ds, "meni");
                    sdaSastojci.Fill(ds, "sastojci");
                    sdaNaMeniju.Fill(ds, "naMeniju");
                    sdaSeSastoji.Fill(ds, "seSastoji");
                    return ds;
                }
            }
            catch (Exception exc)
            {
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }

        public bool vratiDataSet(DataSet podaciDataSet)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // SQLDataAdapteri za svaku tabelu posebno
                    // Takodje i njihove funkcije za update-ovanje podataka
                    #region SQLAdapteri sa komandama
                    SqlDataAdapter sdaJelo = new SqlDataAdapter("SELECT * FROM jelo", conn);
                    sdaJelo.UpdateCommand = new SqlCommand("UPDATE * FROM Jelo SET id_jela = @id_jela, naziv_jela = @naziv_jela, opis_jela = @opis_jela WHERE id_jela = @id_jela", conn);
                    sdaJelo.UpdateCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaJelo.UpdateCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaJelo.UpdateCommand.Parameters.Add("@naziv_jela", SqlDbType.NVarChar, 50, "naziv_jela");
                    sdaJelo.UpdateCommand.Parameters.Add("@opis_jela", SqlDbType.Text);
                    sdaJelo.UpdateCommand.Parameters["@opis_jela"].SourceColumn = "opis_jela";
                    sdaJelo.DeleteCommand = new SqlCommand("DELETE FROM Jelo WHERE id_jela = @id_jela", conn);
                    sdaJelo.DeleteCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaJelo.DeleteCommand.Parameters["@id_jela"].SourceColumn = "id_jela";

                    SqlDataAdapter sdaMeni = new SqlDataAdapter("SELECT * FROM meni", conn);
                    sdaMeni.UpdateCommand = new SqlCommand("UPDATE Meni SET id_menija = @id_menija, naziv_menija = @naziv_menija, aktivan = @aktivan WHERE id_menija = @id_menija", conn);
                    sdaMeni.UpdateCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaMeni.UpdateCommand.Parameters["@id_menija"].SourceColumn = "id_menija";
                    sdaMeni.UpdateCommand.Parameters.Add("@naziv_menija", SqlDbType.NVarChar, 50, "naziv_menija");
                    sdaMeni.UpdateCommand.Parameters.Add("@aktivan", SqlDbType.Bit);
                    sdaMeni.UpdateCommand.Parameters["@aktivan"].SourceColumn = "aktivan";
                    sdaMeni.DeleteCommand = new SqlCommand("DELETE FROM Meni WHERE id_menija = @id_menija", conn);
                    sdaMeni.DeleteCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaMeni.DeleteCommand.Parameters["@id_menija"].SourceColumn = "id_menija";

                    SqlDataAdapter sdaSastojci = new SqlDataAdapter("SELECT * FROM sastojci", conn);
                    sdaSastojci.UpdateCommand = new SqlCommand("UPDATE Sastojci SET id_sastojka = @id_sastojka, naziv_sastojka = @naziv_sastojka, opis_sastojka = @opis_sastojka WHERE id_sastojka = @id_sastojka", conn);
                    sdaSastojci.UpdateCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaSastojci.UpdateCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";
                    sdaSastojci.UpdateCommand.Parameters.Add("@naziv_sastojka", SqlDbType.NVarChar, 50, "naziv_sastojka");
                    sdaSastojci.UpdateCommand.Parameters.Add("@opis_sastojka", SqlDbType.Text);
                    sdaSastojci.UpdateCommand.Parameters["@opis_sastojka"].SourceColumn = "opis_sastojka";
                    sdaSastojci.DeleteCommand = new SqlCommand("DELETE FROM Sastojci WHERE id_sastojka = @id_sastojka", conn);
                    sdaSastojci.DeleteCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaSastojci.DeleteCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";

                    SqlDataAdapter sdaNaMeniju = new SqlDataAdapter("SELECT * FROM naMeniju", conn);
                    sdaNaMeniju.UpdateCommand = new SqlCommand("UPDATE naMeniju SET id_jela = @id_jela, id_menija = @id_menija, cena = @cena WHERE id_jela = @id_jela AND id_menija = @id_menija", conn);
                    sdaNaMeniju.UpdateCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaNaMeniju.UpdateCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaNaMeniju.UpdateCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaNaMeniju.UpdateCommand.Parameters["@id_menija"].SourceColumn = "id_menija";
                    sdaNaMeniju.UpdateCommand.Parameters.Add("@cena", SqlDbType.BigInt);
                    sdaNaMeniju.UpdateCommand.Parameters["@cena"].SourceColumn = "cena";
                    sdaNaMeniju.DeleteCommand = new SqlCommand("DELETE FROM NaMeniju WHERE id_jela = @id_jela AND id_menija = @id_menija", conn);
                    sdaNaMeniju.DeleteCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaNaMeniju.DeleteCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaNaMeniju.DeleteCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaNaMeniju.DeleteCommand.Parameters["@id_menija"].SourceColumn = "id_menija";

                    SqlDataAdapter sdaSeSastoji = new SqlDataAdapter("SELECT * FROM seSastoji", conn);
                    sdaNaMeniju.UpdateCommand = new SqlCommand("UPDATE seSastoji SET id_jela = @id_jela, id_sastojka = @id_sastojka, kolicina = @kolicina WHERE id_jela = @id_jela AND id_sastojka = @id_sastojka", conn);
                    sdaNaMeniju.UpdateCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaNaMeniju.UpdateCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaNaMeniju.UpdateCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaNaMeniju.UpdateCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";
                    sdaNaMeniju.UpdateCommand.Parameters.Add("@kolicina", SqlDbType.Int);
                    sdaNaMeniju.UpdateCommand.Parameters["@kolicina"].SourceColumn = "kolicina";
                    sdaNaMeniju.DeleteCommand = new SqlCommand("DELETE FROM NaMeniju WHERE id_jela = @id_jela AND id_sastojka = @id_sastojka", conn);
                    sdaNaMeniju.DeleteCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaNaMeniju.DeleteCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaNaMeniju.DeleteCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaNaMeniju.DeleteCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";

                    #endregion

                    // Unosenje izmena u bazu preko SQLDataAdaptera
                    sdaJelo.Update(podaciDataSet.Tables["jelo"]);
                    sdaMeni.Update(podaciDataSet.Tables["meni"]);
                    sdaSastojci.Update(podaciDataSet.Tables["sastojci"]);
                    sdaNaMeniju.Update(podaciDataSet.Tables["naMeniju"]);
                    sdaSeSastoji.Update(podaciDataSet.Tables["seSastoji"]);

                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
            return true;
        }

        public int InsertIntoMeni(string naziv, bool aktivan)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Meni VALUES ('" + naziv + "', " + (aktivan?1:0).ToString() + "); select @@identity;", conn);
                    int ret = int.Parse(cmd.ExecuteScalar().ToString());
                    return ret;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }

        public int InsertIntoJelo(string naziv, string opis)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Jelo VALUES ('" + naziv + "', '" + opis + "'); SELECT @@identity;", conn);
                    int ret = (int)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }

        public int InsertIntoSastojak(string naziv, string opis)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Sastojak VALUES ('" + naziv + "', '" + opis + "');  SELECT @@identity;", conn);
                    int ret = (int)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception exc)
            {
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }
    }
}
