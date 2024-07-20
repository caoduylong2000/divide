using System.Xml;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public static class XcodePostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        var project = new PBXProject();
        project.ReadFromFile(PBXProject.GetPBXProjectPath(path));

        string projectGuid = project.GetUnityMainTargetGuid();

        // Enable Modules (C and Objective-C)をYESに設定する
        project.SetBuildProperty(projectGuid, "CLANG_ENABLE_MODULES", "YES");

        project.AddBuildProperty(projectGuid, "OTHER_LDFLAGS", "-ObjC");
        
		// Adding required framework
		project.AddFrameworkToProject(projectGuid, "UserNotifications.framework", false);

        project.WriteToFile(PBXProject.GetPBXProjectPath(path));
        
        
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        //for Applovin MAX(InMobi, Pangle)
        plist.root.values.Remove("UIRequiredDeviceCapabilities");
        plist.root.SetString("NSLocationWhenInUseUsageDescription", "Used to deliver better advertising experience");
        PlistElementDict dict = plist.root.CreateDict("NSAppTransportSecurity");
        dict.SetString("NSAllowsArbitraryLoads", "YES");

        plist.WriteToFile(plistPath);
        
    }
}

#endif
