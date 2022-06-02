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
        DataSet PreuzmiDataSet();
        // Preuzima dataSet sa izmenjenim podacima od klijenta
        [OperationContract]
        bool vratiDataSet(DataSet podaciDataSet);
        // Trazi sva jela na menijima koji su aktivni i imaju cenu vecu od navedene, vraca klijentu listu jela
        [OperationContract]
        List<Jelo> jelaNaMenijimaKojiSuAktivniSaCenom(int cena);
        // Vraca listu jela sa brojem sastojaka jednakim ili vecim od unesenog
        [OperationContract]
        List<Jelo> jelaPremaBrojuSastojaka(int brSastojaka);
        // Vraca listu svih jela za odredjeni meni
        [OperationContract]
        List<Jelo> jelaPremaMeniju(int idMenija);
        // Vraca listu svih jela koja sadrze odredjeni sastojak
        [OperationContract]
        List<Jelo> jelaPremaSastojku(int idSastojka);
    }
}
