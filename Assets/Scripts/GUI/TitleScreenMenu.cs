using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TitleScreenMenu : Photon.MonoBehaviour 
{
	public static TitleScreenMenu Singleton;

	public GameObject ListItemPrefab;
	public Transform ListItemSpawn;

	public tk2dButton HostGame;
	public tk2dButton JoinGame;
	public tk2dButton RefreshList;
	public tk2dButton InputName;
	public tk2dButton InputGameName;
	public List<tk2dButton> HostList;
	public tk2dButton CloseGame;
	public tk2dButton Europe;
	public tk2dButton USA;
	public tk2dButton Asia;

	public int SelectedHostListItem = -1;
	public int SelectedServer = 0;

	private float elapsedTimeOnClose = 0f;
	
	private tk2dSlicedSprite europeSprite;
	private tk2dSlicedSprite usaSprite;
	private tk2dSlicedSprite asiaSprite;

	void Awake()
	{
		Singleton = this;
		europeSprite = Europe.GetComponent<tk2dSlicedSprite> ();
		usaSprite = USA.GetComponent<tk2dSlicedSprite> ();
		asiaSprite = Asia.GetComponent<tk2dSlicedSprite> ();
	}

	void Start () 
	{
		HostGame.ButtonDownEvent += OnHostGame;
		JoinGame.ButtonDownEvent += OnJoinGame;
		RefreshList.ButtonDownEvent += OnRefreshList;
		InputName.ButtonDownEvent += OnInputName;
		InputGameName.ButtonDownEvent += OnInputGameName;
		HostList = new List<tk2dButton> ();
		CloseGame.ButtonDownEvent += OnCloseGame;
		Europe.ButtonDownEvent += OnEuropeClicked;
		USA.ButtonDownEvent += OnUSAClicked;
		Asia.ButtonDownEvent += OnAsiaClicked;
	}

	void Update () 
	{
		if (elapsedTimeOnClose > 0f) elapsedTimeOnClose -= Time.deltaTime;
		
		europeSprite.spriteId = 1;
		usaSprite.spriteId = 1;
		asiaSprite.spriteId = 1;

		switch (SelectedServer) 
		{
		case 0:
			europeSprite.spriteId = 2;
			break;
		case 1:
			usaSprite.spriteId = 2;
			break;
		case 2:
			asiaSprite.spriteId = 2;
			break;
		}
	}
	
	public void OnRefreshList(tk2dButton source)
	{
		SelectedHostListItem = -1;
		foreach (tk2dButton item in HostList) DestroyImmediate (item.gameObject);
		HostList.Clear ();
		if (!PhotonNetwork.insideLobby) return;
		for(int index = 0; index < PhotonNetwork.GetRoomList().Length; index++)
		{
			GameObject newListItem = (GameObject)Instantiate(ListItemPrefab, Vector3.zero, Quaternion.identity);
			newListItem.transform.parent = ListItemSpawn.transform.parent;
			newListItem.transform.localPosition = ListItemSpawn.transform.localPosition+(ListItemSpawn.transform.localPosition*index);
			newListItem.GetComponent<tk2dButton>().ButtonDownEvent += OnAnyHostListItem;
			string[] nameContent = PhotonNetwork.GetRoomList()[index].name.Split('|');
			if(nameContent.Length != 2) continue;
			newListItem.transform.FindChild("GameName").GetComponent<tk2dTextMesh>().text += nameContent[1];
			newListItem.transform.FindChild("GameName").GetComponent<tk2dTextMesh>().Commit();
			newListItem.transform.FindChild("Host").GetComponent<tk2dTextMesh>().text += nameContent[0];
			newListItem.transform.FindChild("Host").GetComponent<tk2dTextMesh>().Commit();
			HostList.Add(newListItem.GetComponent<tk2dButton>());
		}
	}
	
	public void OnHostGame(tk2dButton source)
	{
		SelectedHostListItem = -1;
		string hostName = InputName.transform.GetChild(0).GetComponent<TextField>().MyName+"|"+InputGameName.transform.GetChild(0).GetComponent<TextField>().MyName;
		string count = "";
		bool youMayProceed = false;

		while (!youMayProceed) 
		{
			youMayProceed = true;
			foreach(RoomInfo item in PhotonNetwork.GetRoomList())
			{
				if(hostName+count == item.name)
				{
					if(count.Length == 0) 
						count = "0";
					else
						count = (Convert.ToInt32(count)+1).ToString();
					youMayProceed = false;
					break;
				}
			}
		}
		PhotonNetwork.JoinOrCreateRoom(hostName+count, new RoomOptions(), new TypedLobby());
	}
	
	public void OnJoinGame(tk2dButton source)
	{
		if (SelectedHostListItem == -1) return;

		if(!PhotonNetwork.inRoom && PhotonNetwork.GetRoomList().Length > SelectedHostListItem)
			PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.GetRoomList()[SelectedHostListItem].name, new RoomOptions(), new TypedLobby());
		
		SelectedHostListItem = -1;
	}
	
	public void OnInputName(tk2dButton source)
	{
		InputName.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	}

	public void OnInputGameName(tk2dButton source)
	{
		InputGameName.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	}
	
	public void OnAnyHostListItem(tk2dButton source)
	{
		for(int index = 0; index < HostList.Count; index++)
		{
			if(source == HostList[index]) 
			{
				SelectedHostListItem = index;
				return;
			}
		}
	}
	
	public void OnEuropeClicked(tk2dButton source)
	{
		if (!PhotonNetwork.insideLobby || SelectedServer == 0) return;

		SelectedServer = 0;
		PhotonNetwork.Disconnect ();
	}

	public void OnUSAClicked(tk2dButton source)
	{
		if (!PhotonNetwork.insideLobby || SelectedServer == 1) return;

		SelectedServer = 1;
		PhotonNetwork.Disconnect ();
	}

	public void OnAsiaClicked(tk2dButton source)
	{
		if (!PhotonNetwork.insideLobby || SelectedServer == 2) return;

		SelectedServer = 2;
		PhotonNetwork.Disconnect ();
	}

	void OnDisconnectedFromPhoton()
	{
		string newIp = "";
		switch (SelectedServer) 
		{
		case 0:
			newIp += "app-eu.exitgamescloud.com";
			break;
		case 1:
			newIp += "app-us.exitgamescloud.com";
			break;
		case 2:
			newIp += "app-asia.exitgamescloud.com";
			break;
		}
		PhotonNetwork.PhotonServerSettings.ServerAddress = newIp;
		PhotonNetwork.ConnectUsingSettings("1.0");
	}
	
	public void OnCloseGame(tk2dButton source)
	{
		if (elapsedTimeOnClose > 0f) return;

		elapsedTimeOnClose = 0.2f;
		GameManager.Singleton.OnCloseGameClicked ();
	}
}
