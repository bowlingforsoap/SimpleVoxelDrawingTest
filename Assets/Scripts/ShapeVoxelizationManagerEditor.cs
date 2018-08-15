using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeVoxelizationManager))]
public class ShapeVoxelizationManagerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		ShapeVoxelizationManager shapeVoxelizationManager = (ShapeVoxelizationManager) target;
		// Naive approach to start/stop/restart
		if (GUILayout.Button("Voxelize Shape")) {
			shapeVoxelizationManager.StartVoxelizeShape();
		}
		if (GUILayout.Button("Stop Voxelization")) {
			shapeVoxelizationManager.StopVoxelizeShape();
		}
	}
}
