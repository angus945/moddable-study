using ModArchitecture;

public class ModA_Init : IModInitializer
{
    public void Initialize()
    {
        UnityEngine.Debug.Log($"ModA模組初始化執行");
    }
}