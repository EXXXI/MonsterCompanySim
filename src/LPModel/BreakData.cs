namespace LPModel
{
    /// <summary>
    /// 社員の凸情報
    /// </summary>
    public class BreakData
    {
        /// <summary>
        /// 社員ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 凸数
        /// </summary>
        public int Break { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BreakData()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="breakCount"></param>
        public BreakData(int id, int breakCount)
        {
            Id = id;
            Break = breakCount;
        }


    }
}
