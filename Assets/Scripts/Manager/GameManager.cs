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
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	void Update () 
	{
		if (MyMainCharacter == null && PhotonNetwork.connected && PhotonNetwork.inRoom) 
		{
			TitleScreenMenu.Singleton.gameObject.SetActive(false);
			MyMainCharacter = PhotonNetwork.Instantiate(CharacterPrefab.name, RightSpawnPoint.transform.position, RightSpawnPoint.transform.rotation, 0);
			MyMainCharacter.GetComponent<CharControl>().MyCursorController = MovementBeacon.GetChild(0).GetComponent<CursorController>();
			CameraManager.Singleton.MyCharacter = MyMainCharacter.transform;
		}
	}

	public void OnCloseGameClicked()
	{
		if (TitleScreenMenu.Singleton.gameObject.activeSelf)
			Application.Quit ();
		else 
		{
			TitleScreenMenu.Singleton.gameObject.SetActive (true);
			if(PhotonNetwork.inRoom)
				PhotonNetwork.LeaveRoom();
		}
	}
}
