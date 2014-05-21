using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TitleScreenMenu : Photon.MonoBehaviour 
{
	public static TitleScreenMenu Singleton;

	public GameObject ListItemPrefab;
	public Transform ListItemSpawn;
	[HideInInspector]
	public tk2dUIItem HostGame;
	[HideInInspector]
	public tk2dUIItem JoinGame;
	[HideInInspector]
	public tk2dUIItem RefreshList;
	[HideInInspector]
	public tk2dUIItem InputName;
	[HideInInspector]
	public tk2dUIItem InputGameName;
	[HideInInspector]
	public List<tk2dUIItem> HostList;
	[HideInInspector]
	public tk2dUIItem CloseGame;
	[HideInInspector]
	public tk2dUIItem Europe;
	[HideInInspector]
	public tk2dUIItem USA;
	[HideInInspector]
	public tk2dUIItem Asia;
	
	[HideInInspector]
	public int SelectedHostListItem = -1;
	[HideInInspector]
	public int SelectedServer = 0;

	private float elapsedTimeOnClose = 0f;
	
	private tk2dSlicedSprite europeSprite;
	private tk2dSlicedSprite usaSprite;
	private tk2dSlicedSprite asiaSprite;

	void Awake()
	{
		HostGame = GameObject.Find ("HostGame").GetComponent<tk2dUIItem>();
		JoinGame = GameObject.Find ("JoinGame").GetComponent<tk2dUIItem>();
		RefreshList = GameObject.Find ("RefreshList").GetComponent<tk2dUIItem>();
		InputName = GameObject.Find ("InputName").GetComponent<tk2dUIItem>();
		InputGameName = GameObject.Find ("InputGameName").GetComponent<tk2dUIItem>();
		CloseGame = GameObject.Find ("CloseGame").GetComponent<tk2dUIItem>();
		Europe = GameObject.Find ("Europe").GetComponent<tk2dUIItem>();
		USA = GameObject.Find ("USA").GetComponent<tk2dUIItem>();
		Asia = GameObject.Find ("Asia").GetComponent<tk2dUIItem>();
		
		Singleton = this;
		europeSprite = Europe.GetComponent<tk2dSlicedSprite> ();
		usaSprite = USA.GetComponent<tk2dSlicedSprite> ();
		asiaSprite = Asia.GetComponent<tk2dSlicedSprite> ();
	}

	void Start () 
	{
		HostGame.OnDownUIItem += OnHostGame;
		JoinGame.OnDownUIItem += OnJoinGame;
		RefreshList.OnDownUIItem += OnRefreshList;
		InputName.OnDownUIItem += OnInputName;
		InputGameName.OnDownUIItem += OnInputGameName;
		HostList = new List<tk2dUIItem> ();
		CloseGame.OnDownUIItem += OnCloseGame;
		Europe.OnDownUIItem += OnEuropeClicked;
		USA.OnDownUIItem += OnUSAClicked;
		Asia.OnDownUIItem += OnAsiaClicked;
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
	
	public void OnRefreshList(tk2dUIItem source)
	{
		SelectedHostListItem = -1;
		foreach (tk2dUIItem item in HostList) DestroyImmediate (item.gameObject);
		HostList.Clear ();
		if (!PhotonNetwork.insideLobby) return;
		for(int index = 0; index < PhotonNetwork.GetRoomList().Length; index++)
		{
			GameObject newListItem = (GameObject)Instantiate(ListItemPrefab, Vector3.zero, Quaternion.identity);
			newListItem.transform.parent = ListItemSpawn.transform.parent;
			newListItem.transform.localPosition = ListItemSpawn.transform.localPosition+(ListItemSpawn.transform.localPosition*index);
			newListItem.GetComponent<tk2dUIItem>().OnDownUIItem += OnAnyHostListItem;
			string[] nameContent = PhotonNetwork.GetRoomList()[index].name.Split('|');
			if(nameContent.Length != 2) continue;
			newListItem.transform.FindChild("GameName").GetComponent<tk2dTextMesh>().text += nameContent[1];
			newListItem.transform.FindChild("GameName").GetComponent<tk2dTextMesh>().Commit();
			newListItem.transform.FindChild("Host").GetComponent<tk2dTextMesh>().text += nameContent[0];
			newListItem.transform.FindChild("Host").GetComponent<tk2dTextMesh>().Commit();
			HostList.Add(newListItem.GetComponent<tk2dUIItem>());
		}
	}
	
	public void OnHostGame(tk2dUIItem source)
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
	
	public void OnJoinGame(tk2dUIItem source)
	{
		if (SelectedHostListItem == -1) return;
		
		if(!PhotonNetwork.inRoom && PhotonNetwork.GetRoomList().Length > SelectedHostListItem)
			PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.GetRoomList()[SelectedHostListItem].name, new RoomOptions(), new TypedLobby());
		
		SelectedHostListItem = -1;
	}
	
	public void OnInputName(tk2dUIItem source)
	{
		InputName.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	}

	public void OnInputGameName(tk2dUIItem source)
	{
		InputGameName.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	}
	
	public void OnAnyHostListItem(tk2dUIItem source)
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
	
	public void OnEuropeClicked(tk2dUIItem source)
	{
		if (!PhotonNetwork.insideLobby || SelectedServer == 0) return;
		
		SelectedServer = 0;
		PhotonNetwork.Disconnect ();
	}

	public void OnUSAClicked(tk2dUIItem source)
	{
		if (!PhotonNetwork.insideLobby || SelectedServer == 1) return;
		
		SelectedServer = 1;
		PhotonNetwork.Disconnect ();
	}

	public void OnAsiaClicked(tk2dUIItem source)
	{
		if (!PhotonNetwork.insideLobby || SelectedServer == 2) return;
		
		SelectedServer = 2;
		PhotonNetwork.Disconnect ();
	}

	void OnDisconnectedFromPhoton()
	{
		if (Time.timeSinceLevelLoad < 3f) return;

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
	
	public void OnCloseGame(tk2dUIItem source)
	{
		if (elapsedTimeOnClose > 0f) return;
		
		elapsedTimeOnClose = 0.2f;
		GameManager.Singleton.OnCloseGameClicked ();
	}
}
