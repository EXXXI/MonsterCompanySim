using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 戦闘する社員
    /// </summary>
    public class Battler
    {
        /// <summary>
        /// 社員情報
        /// </summary>
        public Employee Employee { get; }

        /// <summary>
        /// 攻撃倍率
        /// </summary>
        public double Modifier { get; set; } = 1;

        /// <summary>
        /// 器用倍率
        /// </summary>
        public double DexModifier { get; set; } = 1;

        /// <summary>
        /// 攻撃時クリティカル指定
        /// </summary>
        public CriticalState AtkCritState { get; set; }

        /// <summary>
        /// 防御時クリティカル指定
        /// </summary>
        public CriticalState DefCritState { get; set; }

        /// <summary>
        /// 攻撃時クリティカル指定(1回のみ)
        /// </summary>
        public CriticalState OnceAtkCritState { get; set; }

        /// <summary>
        /// 防御時クリティカル指定(1回のみ)
        /// </summary>
        public CriticalState OnceDefCritState { get; set; }

        /// <summary>
        /// ターゲット固定状態(0:固定無し, 他:その番号で固定)
        /// </summary>
        public int FixedTarget { get; set; } = 0;

        /// <summary>
        /// レベル
        /// </summary>
        public long Level { get; set; }

        /// <summary>
        /// 「1回だけ軽減」使用フラグ(使用済みでtrue)
        /// </summary>
        public bool IsReduced { get; set; } = false;

        /// <summary>
        /// 「敵ミコ属性強化」使用フラグ(使用済みでtrue)
        /// </summary>
        public bool IsBuffedByMico { get; set; } = false;

        /// <summary>
        /// ブースト有無(ありでtrue)
        /// </summary>
        public bool IsBoost { get; set; } = false;

        /// <summary>
        /// スキル無効状態(無効時にtrue)
        /// </summary>
        public bool IsSkillDisabled { get; set; } = false;

        /// <summary>
        /// 戦闘
        /// </summary>
        public long Atk
        {
            get
            {
                return (long)(Employee.Atk * (1 + (Level - 1) * Employee.GuerrillaModifier));
            }
        }

        /// <summary>
        /// 器用
        /// </summary>
        public long Dex
        {
            get
            {
                return Math.Min(2147483647, (long)(Employee.Dex * (1 + (Level - 1) * Employee.GuerrillaModifier)));
            }
        }

        /// <summary>
        /// 消費
        /// </summary>
        public long Eng
        {
            get
            {
                return Math.Min(2000000000, (long)(Employee.Eng * (1 + (Level - 1) * Employee.GuerrillaModifier)));
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="employee">社員情報</param>
        public Battler(Employee employee)
        {
            Employee = employee;
        }

        /// <summary>
        /// 戦闘時の情報を初期化(ブーストなしで計算)
        /// </summary>
        public void ResetAttackProperty()
        {
            ResetAttackProperty(false);
        }

        /// <summary>
        /// 戦闘時の情報を初期化
        /// </summary>
        /// <param name="boost">ブースト有無(ブーストありでtrue)</param>
        public void ResetAttackProperty(bool boost)
        {
            IsBoost = boost;
            Modifier = 1;
            DexModifier = 1;
            AtkCritState = CriticalState.normal;
            DefCritState = CriticalState.normal;
            IsReduced = false;
            IsBuffedByMico = false;
        }

        /// <summary>
        /// コピー作成
        /// プロパティはシャローコピー
        /// </summary>
        /// <returns>コピーした戦闘社員</returns>
        public Battler ShallowCopy()
        {
            return (Battler)MemberwiseClone();
        }

        /// <summary>
        /// 表示用：ToStringオーバーライド
        /// </summary>
        /// <returns>EmployeeのToString結果</returns>
        public override string ToString()
        {
            return Employee?.ToString() ?? string.Empty;
        }
    }
}
