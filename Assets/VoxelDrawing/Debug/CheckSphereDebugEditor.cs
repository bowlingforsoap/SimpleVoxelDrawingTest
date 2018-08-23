using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckSphereDebug))]
public class CheckSphereDebugEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		CheckSphereDebug scriptInstance = (CheckSphereDebug) target;

		if (GUILayout.Button("Check Sphere")) {
			Debug.Log(scriptInstance.CheckSphere());
		}
	}
}
