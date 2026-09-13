using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

/** @brief Repeatable desktop and desktop-browser playtest builds. */
public static class ByteCityBuild
{
    [MenuItem("Byte City/Build Mac playtest")]
    public static void Mac() { Build(BuildTarget.StandaloneOSX, "Builds/Mac/Byte City.app"); }

    [MenuItem("Byte City/Build browser playtest")]
    public static void Web()
    {
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = false;
        Build(BuildTarget.WebGL, "Builds/Web");
    }

    private static void Build(BuildTarget target, string path)
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop playing before building.");
        Directory.CreateDirectory("Builds");
        string reportPath = "Builds/" + target + "-report.txt";
        File.WriteAllText(reportPath, "Building " + target + " at " + DateTime.Now);
        try
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = path, target = target, options = BuildOptions.None
            });
            File.WriteAllText(reportPath, report.summary.result + "\n" + report.summary.totalErrors + " errors\n" + report.summary.totalTime + "\n" + report.summary.totalSize + " bytes\n" +
                string.Join("\n", report.steps.SelectMany(step => step.messages).Where(message => message.type == UnityEngine.LogType.Error).Select(message => message.content)));
        }
        catch (Exception error) { File.WriteAllText(reportPath, error.ToString()); throw; }
    }
}
