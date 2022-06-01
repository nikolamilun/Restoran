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
        [OperationContract]
        DataSet PreuzmiDataSet();
        [OperationContract]
        bool vratiDataSet(DataSet podaciDataSet);
    }
}
