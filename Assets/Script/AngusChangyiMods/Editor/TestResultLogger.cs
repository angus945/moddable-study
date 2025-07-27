using System;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

[InitializeOnLoad]
public static class TestLoggerBootstrap
{
    static TestLoggerBootstrap()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        api.RegisterCallbacks(new TestResultLogger());
    }
}

public class TestResultLogger : ICallbacks
{
    private string _logDir;
    private StreamWriter _writer;

    public void RunStarted(ITestAdaptor testsToRun)
    {
        _logDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "UnitTestLog");
        Directory.CreateDirectory(_logDir);

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var logPath = Path.Combine(_logDir, $"TestResults_{timestamp}.txt");

        _writer = new StreamWriter(logPath, false);
        _writer.WriteLine($"[Unit Test Log] {DateTime.Now}");
        _writer.WriteLine(new string('=', 60));
    }

    public void RunFinished(ITestResultAdaptor result)
    {
        WriteResultRecursive(result);
        _writer.WriteLine(new string('=', 60));
        _writer.Close();
        Debug.Log($"[TestResultLogger] Exported test log to: {_logDir}");
    }

    public void TestStarted(ITestAdaptor test) { }
    public void TestFinished(ITestResultAdaptor result) { }

    private void WriteResultRecursive(ITestResultAdaptor result, int indent = 0)
    {
        string indentStr = new string(' ', indent * 2);
        string status = result.TestStatus.ToString().ToUpper();
        string line = $"{indentStr}- {result.Name}: {status}";

        if (result.HasChildren)
        {
            _writer.WriteLine(line);
            foreach (var child in result.Children)
            {
                WriteResultRecursive(child, indent + 1);
            }
        }
        else
        {
            _writer.WriteLine(line);
            if (!string.IsNullOrEmpty(result.Message))
            {
                _writer.WriteLine($"{indentStr}  > Message: {result.Message.Trim()}");
            }

            if (!string.IsNullOrEmpty(result.Output))
            {
                _writer.WriteLine($"{indentStr}  > Output: {result.Output.Trim()}");
            }
        }
    }
}
