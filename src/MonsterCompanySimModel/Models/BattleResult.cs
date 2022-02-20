using System.Collections.Generic;
using System.Text;

namespace MonsterCompanySimModel.Models
{
    public class BattleResult
    {
        public List<Damage> AllyDamages { get; set; } = new List<Damage> { new Damage() };
        public List<Damage> EnemyDamages { get; set; } = new List<Damage> { new Damage() };
        public double WinRate
        { 
            get 
            {
                return CalcWinRate();
            }
        }

        public Damage? MinEnemyDamage
        {
            get
            {
                double minDamageValue = 42949672960000;
                Damage? minDamage = null;
                foreach (var damage in EnemyDamages)
                {
                    if (minDamageValue >= damage.Value)
                    {
                        minDamage = damage;
                        minDamageValue = minDamage.Value;
                    }
                }
                return minDamage;
            }
        }
        public Damage? MaxAllyDamage
        {
            get
            {
                double maxDamageValue = 0;
                Damage? maxDamage = null;
                foreach (var damage in AllyDamages)
                {
                    if (maxDamageValue <= damage.Value)
                    {
                        maxDamage = damage;
                        maxDamageValue = maxDamage.Value;
                    }
                }
                return maxDamage;
            }
        }

        public void CombineAllyDamages(List<Damage> damages, string log)
        {
            AllyDamages = Damage.CombineDamages(AllyDamages, damages, log);
        }

        public void CombineEnemyDamages(List<Damage> damages, string log)
        {
            EnemyDamages = Damage.CombineDamages(EnemyDamages, damages, log);
        }

        private double CalcWinRate()
        {
            // TODO:高速化

            double rate = 0;

            foreach (var allyDamage in AllyDamages)
            {
                foreach (var enemyDamage in EnemyDamages)
                {
                    if (enemyDamage.Value < allyDamage.Value)
                    {
                        rate += enemyDamage.Probability * allyDamage.Probability;
                    }
                }
            }

            return rate;
        }


    }
}