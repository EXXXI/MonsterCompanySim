using System.Collections.Generic;
using System.Text;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 戦闘結果
    /// </summary>
    public class BattleResult
    {
        /// <summary>
        /// 味方の発生し得るダメージ
        /// </summary>
        public List<Damage> AllyDamages { get; set; } = new List<Damage> { new Damage() };

        /// <summary>
        /// 敵の発生し得るダメージ
        /// </summary>
        public List<Damage> EnemyDamages { get; set; } = new List<Damage> { new Damage() };

        /// <summary>
        /// 勝率
        /// </summary>
        public double WinRate
        { 
            get 
            {
                return CalcWinRate();
            }
        }

        /// <summary>
        /// 敵の最低ダメージ
        /// </summary>
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

        /// <summary>
        /// 味方の最大ダメージ
        /// </summary>
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

        /// <summary>
        /// 攻撃結果を元に、味方の発生し得るダメージを再計算
        /// </summary>
        /// <param name="damages">攻撃結果</param>
        /// <param name="log">攻撃ログ</param>
        public void CombineAllyDamages(List<Damage> damages, string log)
        {
            AllyDamages = Damage.CombineDamages(AllyDamages, damages, log);
        }

        /// <summary>
        /// 攻撃結果を元に、敵の発生し得るダメージを再計算
        /// </summary>
        /// <param name="damages">攻撃結果</param>
        /// <param name="log">攻撃ログ</param>
        public void CombineEnemyDamages(List<Damage> damages, string log)
        {
            EnemyDamages = Damage.CombineDamages(EnemyDamages, damages, log);
        }

        /// <summary>
        /// 勝率計算
        /// </summary>
        /// <returns>勝率</returns>
        private double CalcWinRate()
        {
            // TODO: 高速化

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