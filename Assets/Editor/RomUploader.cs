using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RomUploader {

	private const string cKeyIsFtpUpload = "KeyFtpUpload";
	static bool IsFtpUpload {
		get {
			string value = EditorUserSettings.GetConfigValue (cKeyIsFtpUpload);
			return!string.IsNullOrEmpty (value) && value.Equals ("True");
		}
		set {
			EditorUserSettings.SetConfigValue (cKeyIsFtpUpload, value.ToString ());
		}
	}

	private const string cKeyFTPURL = "KeyFTPURL";
	static string FTPURL {
		get {
			return EditorUserSettings.GetConfigValue (cKeyFTPURL);
		}
		set {
			EditorUserSettings.SetConfigValue(cKeyFTPURL, value);
		}
	}

	private const string cKeyFTPUserName = "KeyFTPUserName";
	static string UserName {
		get {
			return EditorUserSettings.GetConfigValue (cKeyFTPUserName);
		}
		set {
			EditorUserSettings.SetConfigValue(cKeyFTPUserName, value);
		}
	}

	private const string cKeyFTPPassword = "KeyFTPPassword";
	static string FTPPass {
		get {
			return EditorUserSettings.GetConfigValue (cKeyFTPPassword);
		}
		set {
			EditorUserSettings.SetConfigValue(cKeyFTPPassword, value);
		}
	}

	[PreferenceItem("Rom Upload")] 
	static void OnGUI()
	{
		IsFtpUpload = EditorGUILayout.BeginToggleGroup("Android Rom FTP Upload", IsFtpUpload);
        FTPURL = EditorGUILayout.TextField("FTP URL", FTPURL);
        UserName = EditorGUILayout.TextField("User Name", UserName);
        FTPPass = EditorGUILayout.PasswordField("Password", FTPPass);
		EditorGUILayout.EndToggleGroup();
    }

    [PostProcessBuildAttribute(1)]
    static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        Debug.Log( pathToBuiltProject );
        if( IsFtpUpload )
        {
#if UNITY_ANDROID
            FTPUploader ftp = new FTPUploader(FTPURL, UserName, FTPPass);

            string filename = System.IO.Path.GetFileName( pathToBuiltProject );
            ftp.Upload(filename, pathToBuiltProject);

            ftp = null;// Release Resources
#endif
        }
    }

}
