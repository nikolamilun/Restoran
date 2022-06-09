using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Data;

namespace Common
{
    // Interfejs koji definise serverske metode i da li mogu da bace izuzetak
    [ServiceContract]
    public interface IRestoran
    {
        // Salje dataSet sa podacima klijentu
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        DataSet PosaljiDataSet();
        // Preuzima dataSet sa izmenjenim podacima od klijenta
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        bool vratiDataSet(DataSet podaciDataSet);
        // Trazi sva jela na menijima koji su aktivni i imaju cenu vecu od navedene, vraca klijentu listu jela
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        List<Jelo> jelaNaMenijimaKojiSuAktivniSaCenom(int cena);
        // Vraca listu jela sa brojem sastojaka jednakim ili vecim od unesenog
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        List<Jelo> jelaPremaBrojuSastojaka(int brSastojaka);
        // Vraca listu svih jela za odredjeni meni
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        List<Jelo> jelaPremaMeniju(int idMenija);
        // Insertuje red u tabelu meni
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        int InsertIntoMeni(string naziv, bool aktivan);
        // Insertuje red u tabelu jelo
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        int InsertIntoJelo(string naziv, string opis);
        // Insertuje red u tabelu sastojak
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        int InsertIntoSastojak(string naziv, string opis);
        // Vraca listu svih jela koja sadrze odredjeni sastojak
        [OperationContract]
        List<Jelo> jelaPremaSastojku(int idSastojka);
    }
}
