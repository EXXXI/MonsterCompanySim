using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// ダメージ情報
    /// </summary>
    public class Damage
    {
        /// <summary>
        /// 発生確率
        /// </summary>
        public double Probability { get; set; } = 1;

        /// <summary>
        /// ダメージ量
        /// </summary>
        public double Value { get; set; } = 0;

        /// <summary>
        /// クリティカルフラグ(ログ用)
        /// </summary>
        private bool IsCrit { get; set; } = false;

        /// <summary>
        /// 戦闘ログ
        /// </summary>
        public string Log
        {
            get
            {
                if (NewDamage != null && OldDamage != null)
                {
                    double crit = NewDamage.IsCrit ? NewDamage.Probability : 1 - NewDamage.Probability;
                    string log = OldDamage.Log + LocalLog + $":{NewDamage.Value:#,0} {(NewDamage.IsCrit ? "Critical " : "            ")}(CT率{crit * 100}%)\n";
                    // System.Diagnostics.Debug.Assert(log == LocalLog);
                    return log;
                }
                return string.Empty;
            }
        }

        // private string TestLog { get; set; } = "";

        private string LocalLog { get; set; } = string.Empty;

        private Damage? OldDamage { get; set; } = null;

        private Damage? NewDamage { get; set; } = null;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public Damage()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="probability">発生確率</param>
        /// <param name="value">ダメージ量</param>
        /// <param name="isCrit">クリティカルフラグ(ログ用)</param>
        public Damage(double probability, double value, bool isCrit)
        {
            Probability = probability;
            Value = value;
            IsCrit = isCrit;
        }

        /// <summary>
        /// 新しい攻撃でのダメージ情報をもとに、総ダメージを再計算
        /// </summary>
        /// <param name="oldDamages">今までのダメージ</param>
        /// <param name="newDamages">新しい攻撃でのダメージ</param>
        /// <param name="log">新しい攻撃の戦闘ログ</param>
        /// <returns>新しい攻撃を含めたダメージ情報</returns>
        static public List<Damage> CombineDamages(List<Damage> oldDamages, List<Damage> newDamages, string log)
        {
            var result = new List<Damage>(oldDamages.Count * newDamages.Count);
            var dmg = new Damage { };
            foreach (Damage oldDamage in oldDamages)
            {
                foreach (Damage newDamage in newDamages)
                {
                    if (oldDamage.Probability > 0 && newDamage.Probability > 0)
                    {
                        // double crit = newDamage.IsCrit ? newDamage.Probability : 1 - newDamage.Probability;
                        dmg.Probability = oldDamage.Probability * newDamage.Probability;
                        dmg.Value = oldDamage.Value + newDamage.Value;
                        // dmg.TestLog = oldDamage.Log + log + $":{newDamage.Value:#,0} {(newDamage.IsCrit ? "Critical " : "            ")}(CT率{crit * 100}%)\n";
                        dmg.OldDamage = oldDamage;
                        dmg.NewDamage = newDamage;
                        dmg.LocalLog = log;
                        result.Add(dmg);
                    }
                }
            }
            return result;
        }
    }
}
