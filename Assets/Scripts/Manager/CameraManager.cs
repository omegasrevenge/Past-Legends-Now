using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour 
{
	public static CameraManager Singleton;
	public Transform MyCharacter;
	public Transform CameraPivot;
	public float MouseSensitivity = 10f;

	void Awake()
	{
		Singleton = this;
		CameraPivot = transform.parent;
	}

	void Update () 
	{
		CameraPivot.transform.position = MyCharacter.transform.position;
		if(Input.GetMouseButton(0))
			CameraPivot.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y")*MouseSensitivity, Input.GetAxis("Mouse X")*MouseSensitivity,0f);
		Camera.main.transform.localPosition += Camera.main.transform.localPosition.normalized * -Input.GetAxis ("Mouse ScrollWheel") * MouseSensitivity;
	}
}
