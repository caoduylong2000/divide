using UnityEngine;
using UnityEditor;

namespace WK {

[CustomEditor(typeof(CurveAnimator))]
public class CurveAnimatorEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(5f);
		//base.OnInspectorGUI();
		DrawProperties();
	}

	protected void DrawProperties ()
	{
		CurveAnimator ta = (CurveAnimator)target;
		
		EditorGUIUtility.labelWidth = 120f;
		GUI.changed = false;

        GUILayout.BeginHorizontal();
        float duration = EditorGUILayout.FloatField("Duration", ta.duration, GUILayout.Width(160f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            CurveAnimatorTools.RegisterUndo("Time Change", ta);
            ta.duration = UnityEngine.Mathf.Max(duration, 0.01f);
            EditorUtility.SetDirty(ta);
        }

		CurveAnimator.EStyle style = (CurveAnimator.EStyle)EditorGUILayout.EnumPopup("Play Style", ta.style);
		bool playOnAwake = EditorGUILayout.Toggle("Play On Awake", ta.playOnAwake);
		if (GUI.changed)
		{
			CurveAnimatorTools.RegisterUndo("Base Change", ta);
			ta.style = style;
			ta.playOnAwake = playOnAwake;
			EditorUtility.SetDirty(ta);
		}

		if (CurveAnimatorTools.DrawHeader("Position", ref ta.enablePosition))
		{
			ta.enablePosition = true;
			
			Vector3 from = EditorGUILayout.Vector3Field("From", ta.positionFrom);
			Vector3 to = EditorGUILayout.Vector3Field("To", ta.positionTo);
			AnimationCurve positionAnimationCurve = EditorGUILayout.CurveField("Animation Curve", ta.positionAnimationCurve);
			
			if (GUI.changed)
			{
				CurveAnimatorTools.RegisterUndo("Position Change", ta);
				ta.positionFrom = from;
				ta.positionTo = to;
				ta.positionAnimationCurve = positionAnimationCurve;
				EditorUtility.SetDirty(ta);
			}
		}

		if (CurveAnimatorTools.DrawHeader("Rotation", ref ta.enableRotation))
		{
			ta.enableRotation = true;
			
			float from = EditorGUILayout.FloatField("From", ta.rotationFrom);
			float to = EditorGUILayout.FloatField("To", ta.rotationTo);
			AnimationCurve rotationAnimationCurve = EditorGUILayout.CurveField("Animation Curve", ta.rotationAnimationCurve);
			
			if (GUI.changed)
			{
				CurveAnimatorTools.RegisterUndo("Rotation Change", ta);
				ta.rotationFrom = from;
				ta.rotationTo = to;
				ta.rotationAnimationCurve = rotationAnimationCurve;
				EditorUtility.SetDirty(ta);
			}
		}

		if(CurveAnimatorTools.DrawHeader("Scale", ref ta.enableScale))
		{
			ta.enableScale = true;

			var boldtext = new GUIStyle (GUI.skin.label);
			boldtext.fontStyle = FontStyle.Bold;
			bool scaleSyncXY = EditorGUILayout.Toggle("Sync XY", ta.scaleSyncXY);

			if(scaleSyncXY)
			{
				float from = EditorGUILayout.FloatField("From", ta.scaleFrom);
				float to = EditorGUILayout.FloatField("To", ta.scaleTo);
				AnimationCurve scaleAnimationCurve = EditorGUILayout.CurveField("Animation Curve", ta.scaleAnimationCurve);
				
				if (GUI.changed)
				{
					CurveAnimatorTools.RegisterUndo("Scale Change", ta);
					ta.scaleSyncXY = scaleSyncXY;
					ta.scaleFrom = from;
					ta.scaleTo = to;
					ta.scaleAnimationCurve = scaleAnimationCurve;
					EditorUtility.SetDirty(ta);
				}
				
			} else {
				EditorGUILayout.LabelField("Scals X", boldtext);
				float from = EditorGUILayout.FloatField("      From", ta.scaleFrom);
				float to = EditorGUILayout.FloatField("      To", ta.scaleTo);
				AnimationCurve scaleAnimationCurve = EditorGUILayout.CurveField("      Animation Curve", ta.scaleAnimationCurve);
				EditorGUILayout.LabelField("Scals Y", boldtext);
				float fromY = EditorGUILayout.FloatField("      From", ta.scaleFromY);
				float toY = EditorGUILayout.FloatField("      To", ta.scaleToY);
				AnimationCurve scaleAnimationCurveY = EditorGUILayout.CurveField("      Animation Curve", ta.scaleAnimationCurveY);

				if (GUI.changed)
				{
					CurveAnimatorTools.RegisterUndo("Scale Change", ta);
					ta.scaleSyncXY = scaleSyncXY;
					ta.scaleFrom = from;
					ta.scaleTo = to;
					ta.scaleAnimationCurve = scaleAnimationCurve;
					ta.scaleFromY = fromY;
					ta.scaleToY = toY;
					ta.scaleAnimationCurveY = scaleAnimationCurveY;
					EditorUtility.SetDirty(ta);
				}
			}
		}

		if (CurveAnimatorTools.DrawHeader("Color", ref ta.enableColor))
		{
			ta.enableColor = true;
			
			Color from = EditorGUILayout.ColorField("From", ta.colorFrom);
			Color to = EditorGUILayout.ColorField("To", ta.colorTo);
			AnimationCurve colorAnimationCurve = EditorGUILayout.CurveField("Animation Curve", ta.colorAnimationCurve);
			
			if (GUI.changed)
			{
				CurveAnimatorTools.RegisterUndo("Color Change", ta);
				ta.colorFrom = from;
				ta.colorTo = to;
				ta.colorAnimationCurve = colorAnimationCurve;
				EditorUtility.SetDirty(ta);
			}
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("OnComplete"), new GUIContent("On Complete"));
		serializedObject.ApplyModifiedProperties();
    }
}

}
