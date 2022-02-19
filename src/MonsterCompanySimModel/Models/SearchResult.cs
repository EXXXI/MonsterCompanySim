using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    public class SearchResult
    {
        public Employee? Ally1 { get; set; }
        public Employee? Ally2 { get; set; }
        public Employee? Ally3 { get; set; }

        public int MinLevel { get; set; }
        public double WinRate { get; set; }
        public double SumEng { get; set; }
    }
}
