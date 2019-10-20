using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

public class PostBuild
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string buildPath)
    {
        Debug.Log(buildPath);
        if (target == BuildTarget.iOS) {
            /*
            string projPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            string mainTarget = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            string newTarget = proj.AddAppExtension(mainTarget, "appext", "com.unity3d.product.appext", "appext/Info.plist");
            proj.AddFileToBuild(newTarget, proj.AddFile(buildPath + "/appext/TodayViewController.h", "appext/TodayViewController.h"));
            proj.AddFileToBuild(newTarget, proj.AddFile(buildPath + "/appext/TodayViewController.m", "appext/TodayViewController.m"));
            proj.AddFrameworkToProject(newTarget, "NotificationCenter.framework", true);
            proj.WriteToFile(projPath);
            */
            var plistFile = Path.Combine(buildPath, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistFile);
            plist.root["UIFileSharingEnabled"] = new PlistElementBoolean(true);
            plist.WriteToFile(plistFile);
        }
    }
}
