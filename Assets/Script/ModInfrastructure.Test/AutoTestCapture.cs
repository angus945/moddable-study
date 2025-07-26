using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

namespace ModInfrastructure.Test
{
    /// <summary>
    /// Unity 測試結果自動捕獲系統，完全獨立的測試輸出工具
    /// </summary>
    public static class AutoTestCapture
    {
        private static bool isInitialized = false;
        private static readonly List<TestResultData> capturedResults = new List<TestResultData>();
        private static readonly string outputDirectory = GetProjectRootTestLogDirectory();

        /// <summary>
        /// 初始化測試結果捕獲
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (isInitialized) return;

            Debug.Log("Unity 測試結果自動捕獲已啟動");

            // 確保輸出目錄存在
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            isInitialized = true;
        }

        private static string GetProjectRootTestLogDirectory()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectRoot, "UnitTestLog");
        }

        /// <summary>
        /// 手動記錄測試結果 - 提供給測試腳本呼叫
        /// </summary>
        public static void CaptureTestResult(string testName, bool passed, string message = "", double durationMs = 0)
        {
            var result = new TestResultData
            {
                TestName = testName,
                Passed = passed,
                Message = message,
                DurationMs = durationMs,
                Timestamp = DateTime.Now
            };

            capturedResults.Add(result);

            var status = passed ? "PASS" : "FAIL";
            Debug.Log($"[AUTO-CAPTURE] [{status}] {testName} ({durationMs:F2}ms)");
        }

        /// <summary>
        /// 執行測試並自動記錄結果
        /// </summary>
        public static void ExecuteTest(string testName, Action testAction)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            bool passed = false;
            string errorMessage = "";

            try
            {
                testAction.Invoke();
                passed = true;
            }
            catch (Exception ex)
            {
                passed = false;
                errorMessage = ex.Message;
                throw; // 重新拋出異常，讓 NUnit 能夠捕獲
            }
            finally
            {
                stopwatch.Stop();
                CaptureTestResult(testName, passed, errorMessage, stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        /// <summary>
        /// 驗證相等並記錄
        /// </summary>
        public static void VerifyEqual<T>(string testName, string stepName, T expected, T actual)
        {
            bool passed = EqualityComparer<T>.Default.Equals(expected, actual);
            var message = passed
                ? $"✓ {stepName}: Expected {expected}, Got {actual}"
                : $"✗ {stepName}: Expected {expected}, Got {actual}";

            CaptureTestResult($"{testName}.{stepName}", passed, message);

            if (!passed)
            {
                throw new Exception($"{stepName} failed: Expected {expected}, but was {actual}");
            }
        }

        /// <summary>
        /// 驗證非空並記錄
        /// </summary>
        public static void VerifyNotNull<T>(string testName, string stepName, T actual) where T : class
        {
            bool passed = actual != null;
            var message = passed
                ? $"✓ {stepName}: NotNull"
                : $"✗ {stepName}: Expected NotNull, Got null";

            CaptureTestResult($"{testName}.{stepName}", passed, message);

            if (!passed)
            {
                throw new Exception($"{stepName} should not be null");
            }
        }

        /// <summary>
        /// 輸出所有捕獲的測試結果
        /// </summary>
        public static void WriteAllCapturedResults(string fileName = null)
        {
            if (capturedResults.Count == 0)
            {
                Debug.Log("沒有捕獲到測試結果");
                return;
            }

            fileName = fileName ?? $"AutoCaptured_TestResults_{DateTime.Now:yyyyMMdd_HHmmss}";

            WriteTextReport(fileName);
            WriteJsonReport(fileName);
            WriteSummaryToConsole();

            Debug.Log($"已自動輸出 {capturedResults.Count} 個測試結果到 {outputDirectory}");
        }

        /// <summary>
        /// 清除捕獲的結果
        /// </summary>
        public static void ClearCapturedResults()
        {
            capturedResults.Clear();
        }

        /// <summary>
        /// 獲取捕獲的結果統計
        /// </summary>
        public static (int total, int passed, int failed) GetCapturedStats()
        {
            int total = capturedResults.Count;
            int passed = 0;
            int failed = 0;

            foreach (var result in capturedResults)
            {
                if (result.Passed) passed++;
                else failed++;
            }

            return (total, passed, failed);
        }

        private static void WriteTextReport(string fileName)
        {
            var filePath = Path.Combine(outputDirectory, $"{fileName}.txt");
            var sb = new StringBuilder();

            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("測試結果報告 (自動捕獲)");
            sb.AppendLine($"生成時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();

            var summary = GetSummary();
            sb.AppendLine("測試摘要:");
            sb.AppendLine($"  總測試數: {summary.TotalTests}");
            sb.AppendLine($"  通過: {summary.PassedTests}");
            sb.AppendLine($"  失敗: {summary.FailedTests}");
            sb.AppendLine($"  總執行時間: {summary.TotalDurationMs:F2}ms");
            sb.AppendLine($"  成功率: {(summary.TotalTests > 0 ? (double)summary.PassedTests / summary.TotalTests * 100 : 0):F1}%");
            sb.AppendLine();

            sb.AppendLine("詳細結果:");
            sb.AppendLine("-".PadRight(80, '-'));

            foreach (var result in capturedResults)
            {
                var status = result.Passed ? "PASS" : "FAIL";
                sb.AppendLine($"[{status}] {result.TestName}");

                if (!string.IsNullOrEmpty(result.Message))
                {
                    sb.AppendLine($"        {result.Message}");
                }

                if (result.DurationMs > 0)
                {
                    sb.AppendLine($"        執行時間: {result.DurationMs:F2}ms");
                }

                sb.AppendLine($"        時間戳記: {result.Timestamp:HH:mm:ss.fff}");
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"測試結果已寫入: {filePath}");
        }

        private static void WriteJsonReport(string fileName)
        {
            var filePath = Path.Combine(outputDirectory, $"{fileName}.json");
            var summary = GetSummary();

            var report = new TestReport
            {
                metadata = new TestMetadata
                {
                    generatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    totalTests = summary.TotalTests,
                    passedTests = summary.PassedTests,
                    failedTests = summary.FailedTests,
                    totalDurationMs = summary.TotalDurationMs,
                    successRate = summary.TotalTests > 0 ? (double)summary.PassedTests / summary.TotalTests * 100 : 0
                },
                results = capturedResults.Select(r => new TestResultJson
                {
                    testName = r.TestName,
                    passed = r.Passed,
                    message = r.Message,
                    durationMs = r.DurationMs,
                    timestamp = r.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }).ToArray()
            };

            var json = JsonUtility.ToJson(report, true);
            File.WriteAllText(filePath, json, Encoding.UTF8);
            Debug.Log($"JSON 測試結果已寫入: {filePath}");
        }

        private static void WriteSummaryToConsole()
        {
            var summary = GetSummary();
            Debug.Log("=".PadRight(50, '='));
            Debug.Log("測試執行摘要 (自動捕獲)");
            Debug.Log($"總測試數: {summary.TotalTests} | 通過: {summary.PassedTests} | 失敗: {summary.FailedTests}");
            Debug.Log($"成功率: {(summary.TotalTests > 0 ? (double)summary.PassedTests / summary.TotalTests * 100 : 0):F1}%");
            Debug.Log($"總執行時間: {summary.TotalDurationMs:F2}ms");
            Debug.Log($"結果已保存至: {outputDirectory}");
            Debug.Log("=".PadRight(50, '='));
        }

        private static TestSummary GetSummary()
        {
            int passed = 0, failed = 0;
            double totalDuration = 0;

            foreach (var result in capturedResults)
            {
                if (result.Passed) passed++;
                else failed++;
                totalDuration += result.DurationMs;
            }

            return new TestSummary
            {
                TotalTests = capturedResults.Count,
                PassedTests = passed,
                FailedTests = failed,
                TotalDurationMs = totalDuration
            };
        }

        [Serializable]
        public class TestResultData
        {
            public string TestName;
            public bool Passed;
            public string Message;
            public double DurationMs;
            public DateTime Timestamp;
        }

        [Serializable]
        public class TestSummary
        {
            public int TotalTests;
            public int PassedTests;
            public int FailedTests;
            public double TotalDurationMs;
        }

        [Serializable]
        public class TestReport
        {
            public TestMetadata metadata;
            public TestResultJson[] results;
        }

        [Serializable]
        public class TestMetadata
        {
            public string generatedAt;
            public int totalTests;
            public int passedTests;
            public int failedTests;
            public double totalDurationMs;
            public double successRate;
        }

        [Serializable]
        public class TestResultJson
        {
            public string testName;
            public bool passed;
            public string message;
            public double durationMs;
            public string timestamp;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 編輯器工具：手動觸發測試結果輸出
    /// </summary>
    public static class AutoTestCaptureEditor
    {
        [UnityEditor.MenuItem("Tools/Test Results/Write Auto Captured Results")]
        public static void WriteCapturedResults()
        {
            AutoTestCapture.WriteAllCapturedResults();
        }

        [UnityEditor.MenuItem("Tools/Test Results/Clear Auto Captured Results")]
        public static void ClearCapturedResults()
        {
            AutoTestCapture.ClearCapturedResults();
            UnityEngine.Debug.Log("已清除所有自動捕獲的測試結果");
        }

        [UnityEditor.MenuItem("Tools/Test Results/Show Auto Captured Stats")]
        public static void ShowCapturedStats()
        {
            var (total, passed, failed) = AutoTestCapture.GetCapturedStats();
            UnityEngine.Debug.Log($"自動捕獲的測試統計: 總計 {total}, 通過 {passed}, 失敗 {failed}");
        }
    }
#endif
}
