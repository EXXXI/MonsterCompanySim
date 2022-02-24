using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 社員(キー情報のみ)
    /// 入出力用
    /// </summary>
    public class SimpleEmployee
    {
        /// <summary>
        /// 社員番号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 別形態を区別する番号(主に進化)
        /// </summary>
        public int EvolState { get; set; }
    }
}
