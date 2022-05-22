﻿using System;
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
        public string Log { get; set; } = string.Empty;

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
            List<Damage> result = new();
            foreach (Damage oldDamage in oldDamages)
            {
                foreach (Damage newDamage in newDamages)
                {
                    if (oldDamage.Probability > 0 && newDamage.Probability > 0)
                    {
                        double crit = newDamage.IsCrit ? newDamage.Probability : 1 - newDamage.Probability;
                        Damage damage = new()
                        {
                            Probability = oldDamage.Probability * newDamage.Probability,
                            Value = oldDamage.Value + newDamage.Value,
                            Log = $"{oldDamage.Log}{log}:{newDamage.Value:#,0} {(newDamage.IsCrit ? "Critical " : "            ")}(CT率{crit * 100}%)\n"
                        };
                        result.Add(damage);
                    }
                }
            }
            return result;
        }
    }
}
