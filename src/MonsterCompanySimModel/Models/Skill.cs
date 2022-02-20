using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    public class Skill
    {
        public SkillType SkillType { get; set; }
        public Range Range { get; set; }
        public double Modifier { get; set; }
        public EmployeeType Type { get; set; }
        public Element Element { get; set; }
        public List<int> Buddys { get; set; } = new List<int>();
        public int BuddyEvolState { get; set; }

    }
}
