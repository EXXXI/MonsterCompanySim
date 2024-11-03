﻿namespace LPModel
{
    /// <summary>
    /// ステージの解法(凸のパターン)
    /// </summary>
    public class BreakPattrn
    { 
        /// <summary>
        /// 社員1の凸情報
        /// </summary>
        public BreakData? Ally1Break { get; set; }

        /// <summary>
        /// 社員2の凸情報
        /// </summary>
        public BreakData? Ally2Break { get; set; }

        /// <summary>
        /// 社員3の凸情報
        /// </summary>
        public BreakData? Ally3Break { get; set; }

        /// <summary>
        /// パターンの文字列表現
        /// </summary>
        public string PatternStr 
        {
            get => 
                $"{Ally1Break?.Id ?? 0}.{Ally1Break?.Break ?? 0}_{Ally2Break?.Id ?? 0}.{Ally2Break?.Break ?? 0}_{Ally3Break?.Id ?? 0}.{Ally3Break?.Break ?? 0}";
        }

        /// <summary>
        /// 社員追加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="breakCount"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddAlly(int id, int breakCount)
        {
            // 異常チェック
            if (id <= 0)
            {
                return;
            }
            if (Ally3Break != null)
            {
                throw new InvalidOperationException("4体以上の編成");
            }

            // 新しい社員情報を追加
            // 社員はID順で並べる
            BreakData newData = new(id, breakCount);
            if (Ally2Break != null)
            {
                if (Ally2Break.Id < id)
                {
                    Ally3Break = newData;
                    return;
                }
                else
                {
                    Ally3Break = Ally2Break;
                }
            }
            if (Ally1Break != null)
            {
                if (Ally1Break.Id < id)
                {
                    Ally2Break = newData;
                    return;
                }
                else
                {
                    Ally2Break = Ally1Break;
                }
            }
            Ally1Break = newData;
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>-1:左が不要 1:右が不要 0:両方必要</returns>
        static public int ComparePattern(BreakPattrn left, BreakPattrn right)
        {
            if ((left.Ally1Break?.Id != right.Ally1Break?.Id) ||
                (left.Ally2Break?.Id != right.Ally2Break?.Id) ||
                (left.Ally3Break?.Id != right.Ally3Break?.Id))
            {
                return 0;
            }

            if (((left.Ally1Break == null) || (right.Ally1Break == null) || (left.Ally1Break.Break >= right.Ally1Break.Break)) &&
                ((left.Ally2Break == null) || (right.Ally2Break == null) || (left.Ally2Break.Break >= right.Ally2Break.Break)) &&
                ((left.Ally3Break == null) || (right.Ally3Break == null) || (left.Ally3Break.Break >= right.Ally3Break.Break)))
            {
                return -1;
            }

            if (((left.Ally1Break == null) || (right.Ally1Break == null) || (left.Ally1Break.Break <= right.Ally1Break.Break)) &&
                ((left.Ally2Break == null) || (right.Ally2Break == null) || (left.Ally2Break.Break <= right.Ally2Break.Break)) &&
                ((left.Ally3Break == null) || (right.Ally3Break == null) || (left.Ally3Break.Break <= right.Ally3Break.Break)))
            {
                return 1;
            }

            return 0;
        }
    }
}
