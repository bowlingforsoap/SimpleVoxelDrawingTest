using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShapeVoxelizationManager : MonoBehaviour
{
	[Tooltip("Mesh to voxelize.")]
	public MeshFilter shape;
	[Tooltip("Parent for all voxels of the voxelized shape.")]
	public Transform voxelizedShapeVoxelsParent;
	public GameObject boundingBoxVisualizationPrefab;

	private VoxelDrawer voxelDrawer;

    // private Bounds shapeBounds;
	private Vector3 firstVoxelCellIndex;
	private Vector3 lastVoxelCellIndex;
	private Dictionary<Vector3, bool> voxelizedShapeMeta = new Dictionary<Vector3, bool>(10000);
	private Dictionary<Vector3, GameObject> voxelizedShape = new Dictionary<Vector3, GameObject>(10000);

    // Use this for initialization
    void Start()
    {
		voxelDrawer = VoxelDrawer.GetVoxelDrawer();

		SetShapeBounds();
		VisualizeShapeBoundingBox();

		StartCoroutine(VoxelizeShape());
    }

    private void SetShapeBounds()
    {
        // Regular bounding box
		Bounds shapeBounds = shape.mesh.bounds;
		Vector3 shapeScale = shape.transform.localScale;

		Vector3 shapeBoundingBoxMin;
		Vector3 shapeBoundingBoxMax;

		shapeBoundingBoxMin = Vector3.Scale(shapeBounds.min, shape.transform.localScale);
		shapeBoundingBoxMin = shape.transform.localRotation * shapeBoundingBoxMin;
		shapeBoundingBoxMin += shape.transform.localPosition;

		shapeBoundingBoxMax = Vector3.Scale(shapeBounds.max, shape.transform.localScale);
		shapeBoundingBoxMax = shape.transform.localRotation * shapeBoundingBoxMax;
		shapeBoundingBoxMax += shape.transform.localPosition;

		// Adjusted for "voxel grid"
		firstVoxelCellIndex = Utils.PositionToVoxelIndex(shapeBoundingBoxMin, voxelDrawer.voxelSize);
		lastVoxelCellIndex = Utils.PositionToVoxelIndex(shapeBoundingBoxMax, voxelDrawer.voxelSize);

		// Adjust start and end point with respect to the orientation of the bounding box
		Vector3 boundingBoxSize = lastVoxelCellIndex - firstVoxelCellIndex;
		Vector3Int incrementDirection = new Vector3Int(Mathf.RoundToInt(boundingBoxSize.x/Mathf.Abs(boundingBoxSize.x)), Mathf.RoundToInt(boundingBoxSize.y/Mathf.Abs(boundingBoxSize.y)), Mathf.RoundToInt(boundingBoxSize.z/Mathf.Abs(boundingBoxSize.z)));
		// Swap dimentions if firstVoxel.smth is larger than finalVoxel.smth
		float temp;
		if (incrementDirection.x < 0) {
			temp = firstVoxelCellIndex.x;
			firstVoxelCellIndex.x = lastVoxelCellIndex.x;
			lastVoxelCellIndex.x = temp;
		}

		if (incrementDirection.y < 0) {
			temp = firstVoxelCellIndex.y;
			firstVoxelCellIndex.y = lastVoxelCellIndex.y;
			lastVoxelCellIndex.y = temp;
		}

		if (incrementDirection.z < 0) {
			temp = firstVoxelCellIndex.z;
			firstVoxelCellIndex.z = lastVoxelCellIndex.z;
			lastVoxelCellIndex.z = temp;
		}
    }

	private void VisualizeShapeBoundingBox() {
		Vector3[] pointsToVisualize = new Vector3[8];

		pointsToVisualize[0] = firstVoxelCellIndex;
		pointsToVisualize[1] = new Vector3(firstVoxelCellIndex.x, firstVoxelCellIndex.y, lastVoxelCellIndex.z);
		pointsToVisualize[2] = new Vector3(firstVoxelCellIndex.x, lastVoxelCellIndex.y, firstVoxelCellIndex.z);
		pointsToVisualize[3] = new Vector3(lastVoxelCellIndex.x, firstVoxelCellIndex.y, firstVoxelCellIndex.z);
		pointsToVisualize[4] = new Vector3(firstVoxelCellIndex.x, lastVoxelCellIndex.y, lastVoxelCellIndex.z);
		pointsToVisualize[5] = new Vector3(lastVoxelCellIndex.x, lastVoxelCellIndex.y, firstVoxelCellIndex.z);
		pointsToVisualize[6] = new Vector3(lastVoxelCellIndex.x, firstVoxelCellIndex.y, lastVoxelCellIndex.z);
		pointsToVisualize[7] = lastVoxelCellIndex;

		foreach (Vector3 point in pointsToVisualize) {
			Instantiate(boundingBoxVisualizationPrefab, point, Quaternion.identity);		
		}
	}

	private IEnumerator VoxelizeShape() {
		float voxelSize;
		Vector3 firstVoxel;
		Vector3 finalVoxel;

		voxelSize = voxelDrawer.voxelSize;
		
		

		Vector3 currVoxelIndex;
		for (currVoxelIndex.x = firstVoxelCellIndex.x; currVoxelIndex.x <= lastVoxelCellIndex.x; currVoxelIndex.x += voxelSize) {
			for (currVoxelIndex.y = firstVoxelCellIndex.y; currVoxelIndex.y <= lastVoxelCellIndex.y; currVoxelIndex.y += voxelSize) {
				for (currVoxelIndex.z = firstVoxelCellIndex.z; currVoxelIndex.z <= lastVoxelCellIndex.z; currVoxelIndex.z += voxelSize) {
					
					if (VoxelInsideShape(currVoxelIndex, voxelSize)) {
						voxelizedShape.Add(currVoxelIndex, voxelDrawer.DrawVoxel(currVoxelIndex, voxelizedShapeVoxelsParent));
					}

					yield return null;
				}
			}
		}
		
	}

	private bool VoxelInsideShape(Vector3 voxelIndex, float voxelSize) {
		// Debug: visualize the process
		/* GameObject testVoxel = Instantiate(boundingBoxVisualizationPrefab, voxelIndex, Quaternion.identity);
		testVoxel.transform.localScale /= 2f; */

		return Physics.CheckSphere(voxelIndex, voxelSize / 2f, 1 << shape.gameObject.layer);
	}

    // Update is called once per frame
    void Update()
    {

    }
}
