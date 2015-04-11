namespace BoonieBear.DeckUnit.DataStorageService
{
    public interface ITraceService
    {
        string Error { get; }

        /// <summary>
        /// 根据运行模式生成不同的记录文件
        /// </summary>
        /// <returns>生成结果</returns>
        bool SetupService();

        bool TearDownService();
        long Save(string sType, object bTraceBytes);
    }
}