using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// スキル
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// スキル種類
        /// </summary>
        public SkillType SkillType { get; set; }

        /// <summary>
        /// スキル範囲
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// 倍率
        /// </summary>
        public double Modifier { get; set; }

        /// <summary>
        /// タイプ
        /// </summary>
        public EmployeeType Type { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Element Element { get; set; }

        /// <summary>
        /// 相方
        /// </summary>
        public List<int> Buddys { get; set; } = new List<int>();

        /// <summary>
        /// 相方の形態情報
        /// </summary>
        public int BuddyEvolState { get; set; }

    }
}
