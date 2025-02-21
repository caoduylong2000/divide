﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace WK {

public class ScriptableObjectToAsset
{
	readonly static string[] labels = {"Data", "ScriptableObject", string.Empty};

	[MenuItem("Assets/Create ScriptableObject")]
	static void Create ()
	{
		foreach (Object selectedObject in Selection.objects) {
			// get path
			string path = getSavePath (selectedObject);

			// create instance
			ScriptableObject obj = ScriptableObject.CreateInstance (selectedObject.name);
			AssetDatabase.CreateAsset (obj, path);
			labels[2] = selectedObject.name;
			// add label
			ScriptableObject sobj = AssetDatabase.LoadAssetAtPath (path, typeof(ScriptableObject)) as ScriptableObject;
			AssetDatabase.SetLabels (sobj, labels);
			EditorUtility.SetDirty (sobj);
		}
	}

	static string getSavePath (Object selectedObject)
	{
		string objectName = selectedObject.name;
		string dirPath = Path.GetDirectoryName (AssetDatabase.GetAssetPath (selectedObject));
		string path = string.Format ("{0}/{1}.asset", dirPath, objectName);

		if (File.Exists (path))
			for (int i=1;; i++) {
				path = string.Format ("{0}/{1}({2}).asset", dirPath, objectName, i);
				if (! File.Exists (path))
					break;
			}

		return path;
	}
}

}
