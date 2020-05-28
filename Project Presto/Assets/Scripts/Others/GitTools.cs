using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;

public class MenuItems
{
#if UNITY_EDITOR
    [MenuItem("Tools/NoahHelperTools/InstallGit")]
    private static void InstallGit()
    {
        var scriptpath = Path.Combine(Application.streamingAssetsPath, "InstallScoopAndGit.ps1");
        var realArg = $"-noexit -file \"{scriptpath}\"";
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            WindowStyle = ProcessWindowStyle.Normal,
            Arguments = realArg
        };
        Process.Start(startInfo);
    }
    #endif
}