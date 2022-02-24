using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// 社員情報
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// 社員番号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 別形態を区別する番号(主に進化)
        /// </summary>
        public int EvolState { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// スキルリスト
        /// </summary>
        public List<Skill> Skills { get; set; } = new List<Skill>();

        /// <summary>
        /// 属性
        /// </summary>
        public Element Element { get; set; }

        /// <summary>
        /// タイプ
        /// </summary>
        public EmployeeType Type { get; set; }

        /// <summary>
        /// レアリティ
        /// </summary>
        public EmployeeRarity Rarity { get; set; }

        /// <summary>
        /// 所属する部
        /// </summary>
        public int Part { get; set; }

        /// <summary>
        /// ゲリラ係数
        /// </summary>
        public double GuerrillaModifier { get; set; }

        /// <summary>
        /// 戦闘(基礎値)
        /// </summary>
        public int Atk { get; set; }

        /// <summary>
        /// 器用(基礎値)
        /// </summary>
        public int Dex { get; set; }

        /// <summary>
        /// 消費(基礎値)
        /// </summary>
        public int Eng { get; set; }

        /// <summary>
        /// 指定スキルを所持しているか調べる
        /// </summary>
        /// <param name="type">スキル</param>
        /// <returns>指定スキルを所持していれば該当Skill、所持していない場合null</returns>
        public Skill? HasSkill(SkillType type)
        {
            foreach (var skill in Skills)
            {
                if (type == skill.SkillType)
                {
                    return skill;
                }
            }
            return null;
        }

        /// <summary>
        /// 表示用：ToStringオーバーライド
        /// </summary>
        /// <returns>レアリティ\t社員名</returns>
        public override string ToString()
        {
            return Rarity + "\t" + Name ?? string.Empty;
        }
    }
}
