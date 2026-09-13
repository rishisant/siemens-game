using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

/** Writes a durable report for the arcade/network presentation checks. */
[InitializeOnLoad]
public static class ByteCityVerification
{
    private class Reporter : ICallbacks
    {
        public void RunStarted(ITestAdaptor tests){}
        public void TestStarted(ITestAdaptor test){}
        public void TestFinished(ITestResultAdaptor result){}
        public void RunFinished(ITestResultAdaptor result)
        {
            Directory.CreateDirectory("Captures");
            File.WriteAllText("Captures/arcade-runtime-tests.txt",result.ResultState+"\n"+result.PassCount+" passed\n"+result.FailCount+" failed\n"+Failures(result));
        }
        private static string Failures(ITestResultAdaptor result)
        {string text=result.FailCount>0?result.Name+": "+result.Message+"\n":"";foreach(var child in result.Children)text+=Failures(child);return text;}
    }
    private static TestRunnerApi api;
    static ByteCityVerification(){api=ScriptableObject.CreateInstance<TestRunnerApi>();api.RegisterCallbacks(new Reporter());}
    public static void Run()
    {api.Execute(new ExecutionSettings(new Filter {testMode=TestMode.PlayMode,testNames=new[]{"ArcadePresentationTests"}}));}
}
