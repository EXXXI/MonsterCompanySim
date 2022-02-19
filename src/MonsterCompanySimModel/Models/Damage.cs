using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    public class Damage
    {
        public double Probability { get; set; } = 1;
        public double Value { get; set; } = 0;
        public string Log { get; set; } = string.Empty;

        public Damage()
        {

        }

        public Damage(double probability, double value)
        {
            Probability = probability;
            Value = value;
        }

        static public List<Damage> CombineDamages(List<Damage> damages1, List<Damage> damages2, string log)
        {
            List<Damage> newDamages = new();
            foreach (Damage damage1 in damages1)
            {
                foreach (Damage damage2 in damages2)
                {
                    if (damage1.Probability > 0 && damage2.Probability > 0)
                    {
                        Damage newDamage = new Damage()
                        {
                            Probability = damage1.Probability * damage2.Probability,
                            Value = damage1.Value + damage2.Value,
                            Log = damage1.Log + log + ":" +damage2.Value + "\n"
                        };
                        newDamages.Add(newDamage);
                    }
                }
            }
            return newDamages;
        }
    }
}
