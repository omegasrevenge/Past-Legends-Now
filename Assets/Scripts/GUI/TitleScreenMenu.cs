using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TitleScreenMenu : MonoBehaviour 
{
	public static TitleScreenMenu Singleton;

	public GameObject ListItemPrefab;
	public Transform ListItemSpawn;

	public tk2dButton HostGame;
	public tk2dButton JoinGame;
	public tk2dButton RefreshList;
	public tk2dButton InputName;
	public List<tk2dButton> HostList;
	public tk2dButton CloseGame;

	public int SelectedHostListItem = -1;

	public Color DefaultHostItemColor;
	public Color SelectedHostItemColor;

	public string MyName = "Peter";

	void Awake()
	{
		Singleton = this;
	}

	void Start () 
	{
		HostGame.ButtonDownEvent += OnHostGame;
		JoinGame.ButtonDownEvent += OnJoinGame;
		RefreshList.ButtonDownEvent += OnRefreshList;
		InputName.ButtonDownEvent += OnInputName;
		HostList = new List<tk2dButton> ();
		CloseGame.ButtonDownEvent += OnCloseGame;
	}

	void Update () 
	{
		foreach (tk2dButton item in HostList)
			item.GetComponent<tk2dSlicedSprite> ().color = DefaultHostItemColor;

		if (SelectedHostListItem > -1)
			HostList [SelectedHostListItem].GetComponent<tk2dSlicedSprite> ().color = SelectedHostItemColor;
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
			newListItem.transform.GetChild(0).GetComponent<tk2dTextMesh>().text += PhotonNetwork.GetRoomList()[index].name;
			newListItem.transform.GetChild(0).GetComponent<tk2dTextMesh>().Commit();
			HostList.Add(newListItem.GetComponent<tk2dButton>());
		}
	}
	
	public void OnHostGame(tk2dButton source)
	{
		SelectedHostListItem = -1;
		string hostName = MyName;
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
		tk2dTextMesh inputTextfield = InputName.transform.GetChild (0).GetComponent<tk2dTextMesh> ();
		inputTextfield.text = MyName+"|";
		inputTextfield.Commit ();
		StartCoroutine (CTextfieldHandler(inputTextfield));
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
	
	public void OnCloseGame(tk2dButton source)
	{
		Application.Quit ();
	}

	public IEnumerator CTextfieldHandler(tk2dTextMesh targetTextMesh)
	{
		float elapsedTime = 0f;
		bool showingLine = true;
		while (!Input.GetMouseButtonDown(0)) 
		{
			if(!Input.anyKeyDown)
			{
				elapsedTime += Time.deltaTime;
				if(elapsedTime >= 1f)
				{
					elapsedTime = 0f;
					if(showingLine)
					{
						showingLine = false;
						string newText = targetTextMesh.text;
						newText.Remove(newText.Length-1);
						targetTextMesh.text = newText;
						targetTextMesh.Commit();
					}
					else
					{
						showingLine = true;
						targetTextMesh.text += "|";
						targetTextMesh.Commit();
					}
				}
			}
			else
			{
				if(Input.GetKeyDown(KeyCode.A))
				{
					//TODO
				}
			}
		}
		if (showingLine) 
		{
			string newText = targetTextMesh.text;
			newText.Remove(newText.Length-1);
			targetTextMesh.text = newText;
			targetTextMesh.Commit();
		}

		yield return 0;
	}

	public void AddCharToTextfield(string text, bool showingLine, tk2dTextMesh targetMesh, bool removeChar = false)
	{
		if (removeChar) 
		{
			string newText = targetMesh.text;
			newText.Remove (newText.Length - 1);
			targetMesh.text = newText;
			targetMesh.Commit ();
			return;
		}

		if (showingLine) 
		{
			string newText = targetMesh.text;
			newText.Remove (newText.Length - 1);
			newText += text + "|";
			targetMesh.text = newText;
			targetMesh.Commit ();
		} 
		else 
		{
			targetMesh.text += text;
			targetMesh.Commit();
		}
	}
}
