using UnityEngine;

namespace Test.Tools
{
    /// <summary>
    /// 演示自動測試捕獲系統的示例腳本
    /// </summary>
    public class AutoCaptureDemo : MonoBehaviour
    {
        [ContextMenu("Demo Auto Capture")]
        public void DemoAutoCapture()
        {
            // 清除之前的結果
            UnityTestResultCapture.ClearCapturedResults();

            // 演示測試捕獲
            Debug.Log("開始演示自動測試捕獲...");

            // 模擬一些測試結果
            UnityTestResultCapture.CaptureTestResult("Demo_Test_1", true, "✓ 測試通過", 12.5);
            UnityTestResultCapture.CaptureTestResult("Demo_Test_2", false, "✗ 測試失敗：期望值不匹配", 8.3);
            UnityTestResultCapture.CaptureTestResult("Demo_Test_3", true, "✓ 驗證成功", 15.7);

            // 執行測試並自動記錄結果
            UnityTestResultCapture.ExecuteTest("Demo_ExecuteTest", () =>
            {
                // 模擬一些測試邏輯
                int result = 2 + 2;
                UnityTestResultCapture.VerifyEqual("Demo_ExecuteTest", "Addition", 4, result);
            });

            // 輸出結果
            UnityTestResultCapture.WriteAllCapturedResults("AutoCaptureDemo");

            Debug.Log("演示完成！檢查 UnitTestLog 目錄查看結果文件。");
        }

        [ContextMenu("Show Capture Stats")]
        public void ShowCaptureStats()
        {
            var (total, passed, failed) = UnityTestResultCapture.GetCapturedStats();
            Debug.Log($"當前捕獲統計: 總計 {total}, 通過 {passed}, 失敗 {failed}");
        }

        [ContextMenu("Clear Captured Results")]
        public void ClearCapturedResults()
        {
            UnityTestResultCapture.ClearCapturedResults();
            Debug.Log("已清除所有捕獲的測試結果");
        }
    }
}