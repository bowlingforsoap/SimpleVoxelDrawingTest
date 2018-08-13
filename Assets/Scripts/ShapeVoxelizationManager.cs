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
	private Vector3 shapeBoundingBoxMin;
	private Vector3 shapeBoundingBoxMax;
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
        Bounds shapeBounds = shape.mesh.bounds;
		Vector3 shapeScale = shape.transform.localScale;

		shapeBoundingBoxMin = Vector3.Scale(shapeBounds.min, shape.transform.localScale);
		shapeBoundingBoxMin = shape.transform.localRotation * shapeBoundingBoxMin;
		shapeBoundingBoxMin += shape.transform.localPosition;

		shapeBoundingBoxMax = Vector3.Scale(shapeBounds.max, shape.transform.localScale);
		shapeBoundingBoxMax = shape.transform.localRotation * shapeBoundingBoxMax;
		shapeBoundingBoxMax += shape.transform.localPosition;
    }

	private void VisualizeShapeBoundingBox() {
		Vector3[] pointsToVisualize = new Vector3[8];

		pointsToVisualize[0] = shapeBoundingBoxMin;
		pointsToVisualize[1] = new Vector3(shapeBoundingBoxMin.x, shapeBoundingBoxMin.y, shapeBoundingBoxMax.z);
		pointsToVisualize[2] = new Vector3(shapeBoundingBoxMin.x, shapeBoundingBoxMax.y, shapeBoundingBoxMin.z);
		pointsToVisualize[3] = new Vector3(shapeBoundingBoxMax.x, shapeBoundingBoxMin.y, shapeBoundingBoxMin.z);
		pointsToVisualize[4] = new Vector3(shapeBoundingBoxMin.x, shapeBoundingBoxMax.y, shapeBoundingBoxMax.z);
		pointsToVisualize[5] = new Vector3(shapeBoundingBoxMax.x, shapeBoundingBoxMax.y, shapeBoundingBoxMin.z);
		pointsToVisualize[6] = new Vector3(shapeBoundingBoxMax.x, shapeBoundingBoxMin.y, shapeBoundingBoxMax.z);
		pointsToVisualize[7] = shapeBoundingBoxMax;

		foreach (Vector3 point in pointsToVisualize) {
			Instantiate(boundingBoxVisualizationPrefab, point, Quaternion.identity);		
		}
	}

	private IEnumerator VoxelizeShape() {
		/* Vector3 startPoint = shapeBounds.min;
		float voxelSize;

		//while (true) { // Wait until VoxelDrawer initialized
		//	try {
				voxelSize = VoxelDrawer.GetVoxelDrawer().voxelSize;
		//		break;
		//	} catch (NullReferenceException) {}
		//}

		Vector3 voxelIndex;
		for (int i = 0; i < shapeBounds.size.x; i++) {
			for (int j = 0; j < shapeBounds.size.y; j++) {
				for (int k = 0; k < shapeBounds.size.z; k++) { */
					yield return null;
/* 
					voxelIndex = Utils.PositionToVoxelIndex(startPoint + new Vector3(voxelSize * i, voxelSize * j, voxelSize * k), voxelSize);

					if (VoxelInsideShape(voxelIndex, voxelSize)) {
						voxelizedShape.Add(voxelIndex, voxelDrawer.DrawVoxel(voxelIndex, voxelizedShapeVoxelsParent));
					}
				}
			}
		} */
	}

	private bool VoxelInsideShape(Vector3 voxelIndex, float voxelSize) {
		return Physics.CheckSphere(voxelIndex, voxelSize / 2f, shape.gameObject.layer);
	}

    // Update is called once per frame
    void Update()
    {

    }
}
