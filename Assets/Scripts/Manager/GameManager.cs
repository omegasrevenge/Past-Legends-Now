using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public static GameManager Singleton;

	public GameObject RightSpawnPoint;
	public GameObject LeftSpawnPoint;
	public Transform MovementBeacon;
	public GameObject CharacterPrefab;

	[HideInInspector]
	public GameObject MyMainCharacter;

	void Awake()
	{
		Singleton = this;
		Application.runInBackground = true;
		Application.targetFrameRate = 50;
	}

	void Update () 
	{
		if (MyMainCharacter == null && PhotonNetwork.connected && PhotonNetwork.inRoom) 
		{
			MyMainCharacter = PhotonNetwork.Instantiate(CharacterPrefab.name, RightSpawnPoint.transform.position, RightSpawnPoint.transform.rotation, 0);
			CameraManager.Singleton.MyCharacter = MyMainCharacter.transform;
		}
	}
}
