namespace MonsterCompanySimModel.Models
{
    /// <summary>
    /// クリティカル指定
    /// </summary>
    public enum CriticalState
    {
        /// <summary>
        /// 通常
        /// </summary>
        normal,

        /// <summary>
        /// クリティカル回避
        /// </summary>
        noCrit,

        /// <summary>
        /// クリティカル確定
        /// </summary>
        Crit
    }
}