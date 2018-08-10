using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoxelDrawer : MonoBehaviour {
	[Tooltip("Parent GameObject for all voxels to be spawned.")]
	public Transform voxelsParent;
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

	private static VoxelDrawer voxelDrawer;

	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
		trackedController = GetComponent<SteamVR_TrackedController>();
	}

	// Use this for initialization
	void Start () {
		voxelDrawer = this;

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
				DrawVoxel(brushTip.position, voxelsParent);
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
	public GameObject DrawVoxel(Vector3 position, Transform voxelsParent) {
		Vector3 voxelIndex;
		GameObject voxel = null;

		voxelIndex = Utils.PositionToVoxelIndex(position, voxelSize);
		try {
			voxel = voxelData[voxelIndex];
		} catch (KeyNotFoundException) {}

		if (voxel == null) {
			voxelData.Add(voxelIndex, Instantiate(voxelPrefab, voxelIndex, Quaternion.identity, voxelsParent));
		}

		return voxel;
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

	public static VoxelDrawer GetVoxelDrawer() {
		return voxelDrawer;
	}
}
