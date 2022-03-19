using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 編成検索結果
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// 味方1
        /// </summary>
        public Employee? Ally1 { get; set; }

        /// <summary>
        /// 味方2
        /// </summary>
        public Employee? Ally2 { get; set; }

        /// <summary>
        /// 味方3
        /// </summary>
        public Employee? Ally3 { get; set; }

        /// <summary>
        /// 要求レベル
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// 勝率(0～1の数値)
        /// </summary>
        public double WinRate { get; set; }

        /// <summary>
        /// 勝率(0～100の数値)
        /// </summary>
        public double WinPercentage
        {
            get
            {
                return WinRate * 100;
            }
        }

        /// <summary>
        /// 消費合計
        /// </summary>
        public double SumEng { get; set; }
    }
}
