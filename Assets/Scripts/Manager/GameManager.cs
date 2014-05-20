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
	}

	void Start () 
	{
		MyMainCharacter = (GameObject)Instantiate (CharacterPrefab, RightSpawnPoint.transform.position, RightSpawnPoint.transform.rotation);
		CameraManager.Singleton.MyCharacter = MyMainCharacter.transform;
	}
}
