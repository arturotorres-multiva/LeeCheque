using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeCheque
{
    class Cheque
    {
        public int EstatusLectura { get; set; }
        public short EstatusDisp { get; set; }
        public string Banda { get; set; }
        public string Anverso { get; set; }
        public string Reverso { get; set; }
        public string Error { get; set; }
    }
}
