using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Attach this script to a sphere of size voxelSize to be able to drag around the scene and check whether it intersects with different objects.  
public class CheckSphereDebug : MonoBehaviour {
	public bool CheckSphere() {
		int layerMask = 1 << 8;
		return Physics.CheckSphere(transform.position, VoxelDrawer.GetVoxelDrawer().voxelSize, layerMask);
	}
}
