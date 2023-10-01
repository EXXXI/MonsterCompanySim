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
        /// 味方への戦闘倍率
        /// </summary>
        public double AllyAtkModifier { get; set; } = 1.0;

        /// <summary>
        /// 敵への戦闘倍率
        /// </summary>
        public double EnemyAtkModifier { get; set; } = 1.0;

        /// <summary>
        /// 味方への器用倍率
        /// </summary>
        public double AllyDexModifier { get; set; } = 1.0;

        /// <summary>
        /// 敵への器用倍率
        /// </summary>
        public double EnemyDexModifier { get; set; } = 1.0;

        /// <summary>
        /// 表示用：ToStringオーバーライド
        /// </summary>
        public override string ToString()
        {
            return Description;
        }
    }
}
