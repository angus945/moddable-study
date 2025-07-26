using ModArchitecture;
using ModArchitecture.Logger;

public class CoreEntry : IModEntry
{
    public void Initialize()
    {
        ModLogger.Log("Core initialized");
    }

    public void OnGameStart()
    {
        ModLogger.Log("Core game started");
        // 這裡可以添加核心模組的啟動邏輯
    }

    public void OnGameEnd()
    {
        ModLogger.Log("Core game ended");
        // 這裡可以添加核心模組的結束邏輯
    }
}