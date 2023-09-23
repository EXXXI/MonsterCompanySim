using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 2.5部からの特殊条件
    /// </summary>
    public class StageCondition
    {
        /// <summary>
        /// 管理用名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 説明文
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 味方への倍率
        /// </summary>
        public double AllyAtkModifier { get; set; }

        /// <summary>
        /// 敵への倍率
        /// </summary>
        public double EnemyAtkModifier { get; set; }

        /// <summary>
        /// 表示用：ToStringオーバーライド
        /// </summary>
        public override string ToString()
        {
            return Description;
        }
    }
}
