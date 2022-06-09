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
        // String koji odredjuje specifikacije veze sa tabelom
        public static string connString = @"data source=LIMUNPC\NIKOLASRVSQL;initial catalog=Restoran;trusted_connection=true; MultipleActiveResultSets=True";
        
        // Sluzi za vracanje lista jela po SQL komandi
        public List<Jelo> jelaPoUnesenojKomandi(string cmdText)
        {
            try
            {
                // Otvara se konekcija
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Lista jela koja se prosledjuje
                    List<Jelo> izlaz = new List<Jelo>();
                    // Komanda koja se salje bazi
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        // SDR cita podatke i ubacuje ih u listu
                        izlaz.Add(new Jelo(sdr.GetInt32(0), sdr.GetString(1), sdr.GetString(2)));
                    }
                    return izlaz;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }

        public List<Jelo> jelaNaMenijimaKojiSuAktivniSaCenom(int cena)
        {
            // sva jela koja su na meniju koji je aktivan i imaju cenu manju od one u parametru
            return jelaPoUnesenojKomandi("SELECT j.* FROM Jelo AS j, Meni AS m, NaMeniju AS nm WHERE m.Aktivan = 1 AND m.id_menija = nm.id_menija AND nm.id_jela = j.id_jela AND nm.cena <= " + cena + ";");
        }

        public List<Jelo> jelaPremaBrojuSastojaka(int brSastojaka)
        {
            try
            {
                // Svako jelo koje ima broj sastojaka veci od unesenog
                List<Jelo> izlaz = new List<Jelo>();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Otvara se konekcija
                    conn.Open();
                    // Preuzimaju se primarni kljucevi svih takvih jela
                    SqlCommand cmd = new SqlCommand("SELECT j.id_jela FROM Jelo AS j, SeSastoji AS ss, Sastojci AS s WHERE j.id_jela = ss.id_jela AND s.id_sastojka = ss.id_sastojka GROUP BY j.id_jela HAVING COUNT(ss.id_sastojka) > " + brSastojaka + ";", conn);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        // Za svaki primarni kljuc iz proslog reader-a se uzimaju svi podaci tog jela
                        int id = sdr.GetInt32(0);
                        SqlCommand cmd1 = new SqlCommand("SELECT * FROM Jelo WHERE id_jela = " + id + ";", conn);
                        SqlDataReader sdr1 = cmd1.ExecuteReader();
                        while (sdr1.Read())
                        {
                            // Svako jelo se ubacuje u listu
                            izlaz.Add(new Jelo(sdr1.GetInt32(0), sdr1.GetString(1), sdr1.GetString(2)));
                        }
                    }
                }
                return izlaz;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }

        public List<Jelo> jelaPremaMeniju(int idMenija)
        {
            // Sva jela sa odredjenog menija
            return jelaPoUnesenojKomandi("SELECT j.* FROM jelo AS j, naMeniju AS nm WHERE j.id_jela = nm.id_jela AND nm.id_menija = " + idMenija + ";");
        }

        public List<Jelo> jelaPremaSastojku(int idSastojka)
        {
            // Sva jela koja sadrze odredjeni sastojak
            return jelaPoUnesenojKomandi("SELECT j.* FROM jelo AS J, seSastoji AS ss WHERE j.id_jela = ss.id_jela AND ss.id_sastojka = " + idSastojka + ";");
        }

        public DataSet PosaljiDataSet()
        {
            // Popunjava DataSet podacima iz baze i salje ga klijentu
            try
            {
                // Konekcija
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
            // Uzima DataSet od klijenta i preko njega update-uje bazu podataka preko SDR-ova
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // SQLDataAdapteri za svaku tabelu posebno
                    // Takodje i njihove funkcije za update-ovanje podataka
                    #region SQLDataAdapteri sa komandama
                    // Pravi se novi SDA
                    SqlDataAdapter sdaJelo = new SqlDataAdapter("SELECT * FROM jelo", conn);
                    // UpdateCommand sluzi da odredi kako SDA osvezava podatke u bazi
                    sdaJelo.UpdateCommand = new SqlCommand("UPDATE * FROM Jelo SET naziv_jela = @naziv_jela, opis_jela = @opis_jela WHERE id_jela = @id_jela", conn);
                    sdaJelo.UpdateCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaJelo.UpdateCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaJelo.UpdateCommand.Parameters.Add("@naziv_jela", SqlDbType.NVarChar, 50, "naziv_jela");
                    sdaJelo.UpdateCommand.Parameters.Add("@opis_jela", SqlDbType.Text);
                    sdaJelo.UpdateCommand.Parameters["@opis_jela"].SourceColumn = "opis_jela";
                    // DeleteCommand sluzi da odredi kako SDA brise podatke u bazi
                    sdaJelo.DeleteCommand = new SqlCommand("DELETE FROM Jelo WHERE id_jela = @id_jela", conn);
                    sdaJelo.DeleteCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaJelo.DeleteCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    
                    // VAZNO
                    // KOMENTAR IZNAD VAZI ZA SVAKI SDA

                    SqlDataAdapter sdaMeni = new SqlDataAdapter("SELECT * FROM meni", conn);
                    sdaMeni.UpdateCommand = new SqlCommand("UPDATE Meni SET naziv_menija = @naziv_menija, aktivan = @aktivan WHERE id_menija = @id_menija", conn);
                    sdaMeni.UpdateCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaMeni.UpdateCommand.Parameters["@id_menija"].SourceColumn = "id_menija";
                    sdaMeni.UpdateCommand.Parameters.Add("@naziv_menija", SqlDbType.NVarChar, 50, "naziv_menija");
                    sdaMeni.UpdateCommand.Parameters.Add("@aktivan", SqlDbType.Bit);
                    sdaMeni.UpdateCommand.Parameters["@aktivan"].SourceColumn = "aktivan";
                    sdaMeni.DeleteCommand = new SqlCommand("DELETE FROM Meni WHERE id_menija = @id_menija", conn);
                    sdaMeni.DeleteCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaMeni.DeleteCommand.Parameters["@id_menija"].SourceColumn = "id_menija";

                    SqlDataAdapter sdaSastojci = new SqlDataAdapter("SELECT * FROM sastojci", conn);
                    sdaSastojci.UpdateCommand = new SqlCommand("UPDATE Sastojci SET naziv_sastojka = @naziv_sastojka, opis_sastojka = @opis_sastojka WHERE id_sastojka = @id_sastojka", conn);
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
                    sdaNaMeniju.InsertCommand = new SqlCommand("INSERT INTO NaMeniju VALUES (@id_menija, @id_jela, @cena)", conn);
                    sdaNaMeniju.InsertCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaNaMeniju.InsertCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaNaMeniju.InsertCommand.Parameters.Add("@id_menija", SqlDbType.Int);
                    sdaNaMeniju.InsertCommand.Parameters["@id_menija"].SourceColumn = "id_menija";
                    sdaNaMeniju.InsertCommand.Parameters.Add("@cena", SqlDbType.BigInt);
                    sdaNaMeniju.InsertCommand.Parameters["@cena"].SourceColumn = "cena";

                    SqlDataAdapter sdaSeSastoji = new SqlDataAdapter("SELECT * FROM seSastoji", conn);
                    sdaSeSastoji.UpdateCommand = new SqlCommand("UPDATE seSastoji SET id_jela = @id_jela, id_sastojka = @id_sastojka, kolicina = @kolicina WHERE id_jela = @id_jela AND id_sastojka = @id_sastojka", conn);
                    sdaSeSastoji.UpdateCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaSeSastoji.UpdateCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaSeSastoji.UpdateCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaSeSastoji.UpdateCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";
                    sdaSeSastoji.UpdateCommand.Parameters.Add("@kolicina", SqlDbType.Int);
                    sdaSeSastoji.UpdateCommand.Parameters["@kolicina"].SourceColumn = "kolicina";
                    sdaSeSastoji.DeleteCommand = new SqlCommand("DELETE FROM NaMeniju WHERE id_jela = @id_jela AND id_sastojka = @id_sastojka", conn);
                    sdaSeSastoji.DeleteCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaSeSastoji.DeleteCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaSeSastoji.DeleteCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaSeSastoji.DeleteCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";
                    sdaSeSastoji.InsertCommand = new SqlCommand("INSERT INTO SeSastoji VALUES (@id_sastojka, @id_jela, @kolicina)", conn);
                    sdaSeSastoji.InsertCommand.Parameters.Add("@id_jela", SqlDbType.Int);
                    sdaSeSastoji.InsertCommand.Parameters["@id_jela"].SourceColumn = "id_jela";
                    sdaSeSastoji.InsertCommand.Parameters.Add("@id_sastojka", SqlDbType.Int);
                    sdaSeSastoji.InsertCommand.Parameters["@id_sastojka"].SourceColumn = "id_sastojka";
                    sdaSeSastoji.InsertCommand.Parameters.Add("@kolicina", SqlDbType.BigInt);
                    sdaSeSastoji.InsertCommand.Parameters["@kolicina"].SourceColumn = "kolicina";
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
                // Konekcija
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // U bazu se unosi novi red sa datim vrednostima i vraca se query sa ID-jem koji je dodat
                    SqlCommand cmd = new SqlCommand("INSERT INTO Meni VALUES ('" + naziv + "', " + (aktivan?1:0).ToString() + "); select @@identity;", conn);
                    // ExecuteScalar preuzima taj ID
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
                    // -||- linija 251
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Jelo VALUES ('" + naziv + "', '" + opis + "'); SELECT @@identity;", conn);
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

        public int InsertIntoSastojak(string naziv, string opis)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // -||- linija 251
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Sastojci VALUES ('" + naziv + "', '" + opis + "');  SELECT @@identity;", conn);
                    int ret = int.Parse(cmd.ExecuteScalar().ToString());
                    return ret;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message) ;
                throw new FaultException<Izuzetak>(new Izuzetak(exc.Message));
            }
        }
    }
}
