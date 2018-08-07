using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private Dictionary<Vector3, bool> voxelData = new Dictionary<Vector3, bool>(10000);
	
	private Vector3 brushTipPosition = new Vector3(0f, .0149f, .1123f);
	private Transform brushTip;
	[SerializeField]
	private Material brushTipMaterial;

	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
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
	}

	private Transform GetControllerModelTransform() {
		return transform.GetChild(0);
	}

	/// <summary>Returns true, if a new voxel was added, false if the voxel was already present at the given position.</summary>
	private bool DrawVoxel(Vector3 position) {
		Vector3 voxelIndex;
		bool alreadyPresent;

		voxelIndex = PositionToVoxelIndex(position);
		voxelData.TryGetValue(voxelIndex, out alreadyPresent);

		if (!alreadyPresent) {
			voxelData.Add(voxelIndex, true);
			Instantiate(voxelPrefab, voxelIndex, Quaternion.identity);
		}

		return alreadyPresent;
	}

	private Vector3 PositionToVoxelIndex(Vector3 position) {
		Vector3 voxelIndex;

		voxelIndex = position / voxelSize;
		voxelIndex.x = Mathf.Floor(voxelIndex.x);
		voxelIndex.y = Mathf.Floor(voxelIndex.y);
		voxelIndex.z = Mathf.Floor(voxelIndex.z);
		voxelIndex *= voxelSize;

		return voxelIndex;
	}
}
