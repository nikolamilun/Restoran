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
    [ServiceContract]
    public interface IRestoran
    {
        // Salje dataSet sa podacima klijentu
        [OperationContract]
        [FaultContract(typeof(Izuzetak))]
        DataSet PreuzmiDataSet();
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
        // Vraca listu svih jela koja sadrze odredjeni sastojak
        [OperationContract]
        List<Jelo> jelaPremaSastojku(int idSastojka);
    }
}
