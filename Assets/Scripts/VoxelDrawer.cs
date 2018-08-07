using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelDrawer : MonoBehaviour {
	public float voxelSize;
	[Tooltip("A GameObject with scale Vector3.one.")]
	public GameObject voxelPrefab;

	private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Use this for initialization
	void Start () {
		voxelPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);
	}
	
	// Update is called once per frame
	void Update () {
		if (Controller.GetHairTrigger()) // Hair-Trigger press
		{
			Instantiate(voxelPrefab, transform.position, Quaternion.identity);
		}
	}
}
