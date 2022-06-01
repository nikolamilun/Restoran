using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SeSastoji
    {
        int jeloID;
        int sastojakID;

        public SeSastoji(int jeloID, int sastojakID)
        {
            JeloID = jeloID;
            SastojakID = sastojakID;
        }

        public SeSastoji() { }

        public int JeloID { get => jeloID; set => jeloID = value; }
        public int SastojakID { get => sastojakID; set => sastojakID = value; }
    }
}
