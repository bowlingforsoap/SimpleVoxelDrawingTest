using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoxelDrawer : MonoBehaviour {
	[Range(0.0000001f,1f)]
	public float voxelSize;
	[Tooltip("A GameObject with scale Vector3.one.")]
	public GameObject voxelPrefab;

	private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
	private SteamVR_TrackedController trackedController;

	private Dictionary<Vector3, GameObject> voxelData = new Dictionary<Vector3, GameObject>(10000);
	
	private Vector3 brushTipPosition = new Vector3(0f, .0149f, .1123f);
	private Transform brushTip;
	[SerializeField]
	private Material brushTipMaterial;
	[SerializeField]
	private Material brushTipCenterMaterial;

	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
		trackedController = GetComponent<SteamVR_TrackedController>();
	}

	// Use this for initialization
	void Start () {
		voxelPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

		StartCoroutine(InitializationBrushTip());
	}
	
	// Update is called once per frame
	void Update () {
		if (brushTip != null) 
		{
			brushTip.rotation = Quaternion.identity; // Keep brush tip in the same orientation

			if (Controller.GetHairTrigger()) // true, if even lightly pressed
			{
				DrawVoxel(brushTip.position);
			}

			if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Axis0)) {
				// Show eraser
			}

			// if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			if (trackedController.padPressed) {
				Purge(brushTip.position);
			}
		}
	}

	private IEnumerator InitializationBrushTip() {
		Transform controllerModel = GetControllerModelTransform();

		while(true) {
			yield return null;

			try {
				controllerModel.GetChild(0); // Essentially, wait until the controller model is loaded
			} catch (UnityException) { // Transform child out of bounds
				continue;
			}

			break;		
		}

		brushTip = Instantiate(voxelPrefab, Vector3.zero, Quaternion.identity, controllerModel).transform;
		brushTip.transform.localPosition = brushTipPosition;

		brushTip.gameObject.GetComponent<MeshRenderer>().material = brushTipMaterial;

		// Add a mini tip in the center
		GameObject center = Instantiate(voxelPrefab, Vector3.zero, Quaternion.identity, brushTip);
		center.transform.localPosition = Vector3.zero;
		center.GetComponent<MeshRenderer>().material = brushTipCenterMaterial;
	}

	private Transform GetControllerModelTransform() {
		return transform.GetChild(0);
	}

	/// <summary>Returns true, if a new voxel was added, false if the voxel was already present at the given position.</summary>
	private bool DrawVoxel(Vector3 position) {
		bool alreadyPresent = false;
		Vector3 voxelIndex;
		GameObject voxel = null;

		voxelIndex = Utils.PositionToVoxelIndex(position, voxelSize);
		try {
			voxel = voxelData[voxelIndex];
			alreadyPresent = true;
		} catch (KeyNotFoundException) {}

		if (voxel == null) {
			voxelData.Add(voxelIndex, Instantiate(voxelPrefab, voxelIndex, Quaternion.identity));
		}

		return alreadyPresent;
	}

	private void Purge(Vector3 voxelPosition) {
		Vector3[] pointsToErase = Utils.GetBoundingBoxPoints(voxelPosition, voxelSize);

		foreach (Vector3 point in pointsToErase) {
			EraseVoxel(point);
		}
	}

	/// <summary>Returns true, if the an existing voxel was erased, false if the there was no voxel at the index, or some error occured when deleting.</summary>
	private bool EraseVoxel(Vector3 position) {
		Vector3 voxelIndex;
		bool erased = false;

		voxelIndex = Utils.PositionToVoxelIndex(position, voxelSize);
		try {
			GameObject voxel = voxelData[voxelIndex];
			voxelData.Remove(voxelIndex);
			Destroy(voxel);
			erased = true;
		} catch (KeyNotFoundException) {
			erased = false;
		} catch (Exception e) {
			Debug.Log("Error occured while erasing voxel " + voxelIndex + ":");
			Debug.LogError(e.Message);
		}

		return erased;
	}

	

	private static class Utils {
		/// <summary>Corner points + center.</summary>
		public static Vector3[] GetBoundingBoxPoints(Vector3 center, float size) {
			Vector3[] points = new Vector3[9];
			Vector3 cornerPoint;
			int count = 0;
			points[count++] = center;
			float halfSize = size / 2f;

			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < 2; j++) {
					float iIndexMagic = -1 * i * 2 + 1;
					float jIndexMagic = -1 * j * 2 + 1;
					cornerPoint = new Vector3(center.x + halfSize * iIndexMagic, center.y + halfSize, center.z + halfSize * jIndexMagic);
					
					points[count++] = cornerPoint; // Upper corner point
					points[count++] = new Vector3(cornerPoint.x, cornerPoint.y - size, cornerPoint.z); // Lower corner point
				}
			}

			return points;
		}

		public static Vector3 PositionToVoxelIndex(Vector3 position, float voxelSize) {
			Vector3 voxelIndex;

			voxelIndex = position / voxelSize;
			voxelIndex.x = Mathf.Floor(voxelIndex.x);
			voxelIndex.y = Mathf.Floor(voxelIndex.y);
			voxelIndex.z = Mathf.Floor(voxelIndex.z);
			voxelIndex *= voxelSize;

			voxelIndex += new Vector3(voxelSize/2f, voxelSize/2f, voxelSize/2f); // Move half a voxel left

			return voxelIndex;
		}
	}
}
