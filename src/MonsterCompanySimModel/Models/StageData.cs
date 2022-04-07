using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// ステージデータ用クラス
    /// </summary>
    public class StageData
    {
        /// <summary>
        /// 部
        /// </summary>
        public int Part { get; set; }

        /// <summary>
        /// グレード(2部の場合話数、幕間は13,14...)
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// ステージ(1～10)
        /// </summary>
        public int Stage { get; set; }

        /// <summary>
        /// クリア時入手資金
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// クリア時入手ガチャチケ(2部は金チケ)
        /// </summary>
        public int Ticket { get; set; }

        /// <summary>
        /// 敵社員1人目社員番号
        /// </summary>
        public int Enemy1Id { get; set; }

        /// <summary>
        /// 敵社員1人目進化状況
        /// </summary>
        public int Enemy1EvolState { get; set; }

        /// <summary>
        /// 敵社員1人目レベル
        /// </summary>
        public int Enemy1Level { get; set; }

        /// <summary>
        ///  敵社員2人目社員番号
        /// </summary>
        public int Enemy2Id { get; set; }

        /// <summary>
        /// 敵社員2人目進化状況
        /// </summary>
        public int Enemy2EvolState { get; set; }

        /// <summary>
        /// 敵社員2人目レベル
        /// </summary>
        public int Enemy2Level { get; set; }

        /// <summary>
        ///  敵社員3人目社員番号
        /// </summary>
        public int Enemy3Id { get; set; }

        /// <summary>
        /// 敵社員3人目進化状況
        /// </summary>
        public int Enemy3EvolState { get; set; }

        /// <summary>
        /// 敵社員3人目レベル
        /// </summary>
        public int Enemy3Level { get; set; }

        /// <summary>
        /// 表示用：ToStringオーバーライド
        /// </summary>
        /// <returns>部-グレード-ステージ</returns>
        public override string ToString()
        {
            if (Part == 0)
            {
                // TODO: 定数化
                return "ステージを選択する場合、ここから選択する";
            }
            return $"{Part}-{Grade}-{Stage}\t資金:{Gold:#,0}   \tチケット:{Ticket:#,0}";
        }
    }
}
