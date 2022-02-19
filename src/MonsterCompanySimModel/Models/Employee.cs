using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int EvolState { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public Element Element { get; set; }
        public EmployeeType Type { get; set; }
        public EmployeeRarity Rarity { get; set; }
        public int Part { get; set; }
        public double GuerrillaModifier { get; set; }
        public int Atk { get; set; }
        public int Dex { get; set; }
        public int Eng { get; set; }

        public Skill? HasSkill(SkillType type)
        {
            foreach (var skill in Skills)
            {
                if (type == skill.SkillType)
                {
                    return skill;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return Rarity + "\t" + Name ?? string.Empty;
        }
    }
}
