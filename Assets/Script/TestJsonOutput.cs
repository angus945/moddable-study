// using UnityEngine;

// public class TestJsonOutput : MonoBehaviour
// {
//     [ContextMenu("Test JSON Output")]
//     void Start()
//     {
//         Debug.Log("開始測試 JSON 輸出...");

//         // 使用新的測試工具
//         Test.Tools.SimpleTestRunner.RunTestSuite("JsonOutputTest", () =>
//         {

//             // 模擬一些測試場景
//             Test.Tools.SimpleTestRunner.ExecuteTest("MockTest1", () =>
//             {
//                 // 模擬成功的測試
//                 Test.Tools.SimpleTestRunner.VerifyEqual("MockTest1", "數值檢查", 100, 100);
//                 Test.Tools.SimpleTestRunner.VerifyTrue("MockTest1", "條件檢查", true, "應該為真");
//             });

//             Test.Tools.SimpleTestRunner.ExecuteTest("MockTest2", () =>
//             {
//                 // 模擬失敗的測試 - 這會故意失敗來測試錯誤記錄
//                 try
//                 {
//                     Test.Tools.SimpleTestRunner.VerifyEqual("MockTest2", "失敗檢查", 100, 95);
//                 }
//                 catch
//                 {
//                     // 忽略異常，讓測試繼續
//                 }
//             });

//             Test.Tools.SimpleTestRunner.ExecuteTest("MockTest3", () =>
//             {
//                 // 模擬物件檢查
//                 var testObject = new { name = "test" };
//                 Test.Tools.SimpleTestRunner.VerifyNotNull("MockTest3", "物件檢查", testObject.ToString());
//                 Test.Tools.SimpleTestRunner.VerifyEqual("MockTest3", "字串檢查", "期望值", "期望值");
//             });

//         });

//         Debug.Log($"結果已保存到: {Test.Tools.TestResultWriter.GetOutputDirectory()}");
//     }
// }
