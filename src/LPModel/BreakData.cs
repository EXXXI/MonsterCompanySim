using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModel
{
    public class BreakData
    {
        public int Id { get; set; }
        public int Break { get; set; }

        public BreakData()
        {
        }

        public BreakData(int id, int breakCount)
        {
            Id = id;
            Break = breakCount;
        }


    }
}
