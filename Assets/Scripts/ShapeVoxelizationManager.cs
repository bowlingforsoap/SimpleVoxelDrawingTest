using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShapeVoxelizationManager : MonoBehaviour
{
	[Tooltip("Parent for all voxels of the voxelized shape.")]
	public Transform voxelizedShapeVoxelsParent;

	private GameObject shape;
	[SerializeField]
    public GameObject Shape
    {
        get { return shape; }

        set
        {
            shape = value;
            SetShapeBounds();
        }
    }

    private Bounds shapeBounds;
	private Dictionary<Vector3, bool> voxelizedShapeMeta = new Dictionary<Vector3, bool>(10000);
	private Dictionary<Vector3, GameObject> voxelizedShape = new Dictionary<Vector3, GameObject>(10000);

    // Use this for initialization
    void Start()
    {
        if (Shape != null)
        {
            SetShapeBounds();
        }

		StartCoroutine(VoxelizeShape());
    }

    private void SetShapeBounds()
    {
        shapeBounds = Shape.GetComponent<MeshFilter>().mesh.bounds;
    }

	private IEnumerator VoxelizeShape() {
		Vector3 startPoint = shapeBounds.min;
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
				for (int k = 0; k < shapeBounds.size.z; k++) {
					yield return null;

					voxelIndex = Utils.PositionToVoxelIndex(startPoint + new Vector3(voxelSize * i, voxelSize * j, voxelSize * k), voxelSize);

					if (VoxelInsideShape(voxelIndex, voxelSize)) {
						voxelizedShape.Add(voxelIndex, VoxelDrawer.GetVoxelDrawer().DrawVoxel(voxelIndex, voxelizedShapeVoxelsParent));
					}
				}
			}
		}
	}

	private bool VoxelInsideShape(Vector3 voxelIndex, float voxelSize) {
		return Physics.CheckSphere(voxelIndex, voxelSize / 2f, Shape.layer);
	}

    // Update is called once per frame
    void Update()
    {

    }
}
