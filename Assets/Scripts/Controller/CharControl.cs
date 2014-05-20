using UnityEngine;
using System.Collections;

public class CharControl : MonoBehaviour 
{
	public NavMeshAgent MyAgent;

	[HideInInspector]
	public CursorController MyCursorController;

	void Start()
	{
		if(!GetComponent<PhotonView>().isMine)
		{
			enabled = false;
			MyAgent.enabled = false;
			return;
		}
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown(1)) 
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) 
			{
				if(hit.collider.gameObject.layer == 8)
				{
					MyAgent.destination = MyCursorController.MoveCursor(hit.point);
				}
			}
		}
	}
}
