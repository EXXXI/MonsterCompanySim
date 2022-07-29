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

                    // // ログ遅延評価確認用
                    // System.Diagnostics.Debug.Assert(log == TestLog);

                    return log;
                }
                return string.Empty;
            }
        }
        
        // /// <summary>
        // /// ログ遅延評価確認用
        // /// </summary>
        // private string TestLog { get; set; } = "";

        /// <summary>
        /// ログ計算用
        /// 本攻撃での社員情報
        /// </summary>
        private string LocalLog { get; set; } = string.Empty;

        /// <summary>
        /// ログ計算用
        /// 本攻撃以前のダメージデータ
        /// </summary>
        private Damage? OldDamage { get; set; } = null;

        /// <summary>
        /// ログ計算用
        /// 本攻撃単体のダメージデータ
        /// </summary>
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
            foreach (Damage oldDamage in oldDamages)
            {
                foreach (Damage newDamage in newDamages)
                {
                    Damage dmg = new();
                    if (oldDamage.Probability > 0 && newDamage.Probability > 0)
                    {
                        dmg.Probability = oldDamage.Probability * newDamage.Probability;
                        dmg.Value = oldDamage.Value + newDamage.Value;

                        // ログ関連は遅延評価にするため、プロパティに必要な情報を保存
                        dmg.OldDamage = oldDamage;
                        dmg.NewDamage = newDamage;
                        dmg.LocalLog = log;

                        // // ログ遅延評価確認用
                        // double crit = newDamage.IsCrit ? newDamage.Probability : 1 - newDamage.Probability;
                        // dmg.TestLog = oldDamage.Log + log + $":{newDamage.Value:#,0} {(newDamage.IsCrit ? "Critical " : "            ")}(CT率{crit * 100}%)\n";

                        result.Add(dmg);
                    }
                }
            }
            return result;
        }
    }
}
