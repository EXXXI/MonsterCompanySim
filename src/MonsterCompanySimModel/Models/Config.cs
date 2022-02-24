using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 設定ファイルデータ保持クラス
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 社員無しの名称
        /// </summary>
        public string NoEmproyeeName { get; set; } = "社員無し";

        /// <summary>
        /// 最大レベル
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// 編成検索時の、要求レベル検索を起動する検索結果数の閾値
        /// </summary>
        public int RequireThreshold { get; set; }
    }
}
