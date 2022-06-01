using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NaMeniju
    {
        int meniID;
        int jeloID;

        public NaMeniju(int meniID, int jeloID)
        {
            this.meniID = meniID;
            this.jeloID = jeloID;
        }

        public NaMeniju() { }
        public int MeniID { get => meniID; set => meniID = value; }
        public int JeloID { get => jeloID; set => jeloID = value; }
    }
}
