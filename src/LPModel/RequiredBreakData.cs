using MonsterCompanySimModel.Models;

namespace LPModel
{
    /// <summary>
    /// ステージ情報とその解法一覧
    /// </summary>
    public class RequiredBreakData
    {
        /// <summary>
        /// 部
        /// </summary>
        public int Part { get; set; }

        /// <summary>
        /// グレード
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// ステージ
        /// </summary>
        public int Stage { get; set; }

        /// <summary>
        /// ステージの文字列表現
        /// </summary>
        public string StageStr { get => $"{Part}-{Grade}-{Stage}"; }

        /// <summary>
        /// 解法一覧
        /// </summary>
        public List<BreakPattrn> Pattrns { get; set; } = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RequiredBreakData()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stage"></param>
        public RequiredBreakData(StageData stage)
        {
            Part = stage.Part;
            Grade = stage.Grade;
            Stage = stage.Stage;
        }

        /// <summary>
        /// パターン追加
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <param name="id3"></param>
        /// <param name="break1"></param>
        /// <param name="break2"></param>
        /// <param name="break3"></param>
        public void AddPattern(int id1, int id2, int id3, int break1, int break2, int break3)
        {
            BreakPattrn newPattern = new BreakPattrn();
            newPattern.AddAlly(id1, break1);
            newPattern.AddAlly(id2, break2);
            newPattern.AddAlly(id3, break3);

            // 既により少ない凸数で解法が用意されている場合無視
            var noUse = Pattrns.Where(p => BreakPattrn.ComparePattern(p, newPattern) == 1).Any();
            if (noUse)
            {
                return;
            }

            // 新しい解法より多い凸数の解法があった場合削除
            List<BreakPattrn> deleteList = new();
            var toDelete = Pattrns.Where(p => BreakPattrn.ComparePattern(p, newPattern) == -1).ToList();
            foreach (var item in toDelete) 
            {
                Pattrns.Remove(item);
            }
            Pattrns.Add(newPattern);
        }
    }
}
