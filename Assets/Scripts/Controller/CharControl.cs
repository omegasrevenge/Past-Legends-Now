using UnityEngine;
using System.Collections;

public class CharControl : MonoBehaviour 
{
	public NavMeshAgent MyAgent;
	
	[HideInInspector]
	public Transform curBeaconTrans;

	void Start()
	{
		curBeaconTrans = GameManager.Singleton.MovementBeacon.transform;
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
					curBeaconTrans.position = new Vector3( hit.point.x, curBeaconTrans.position.y ,hit.point.z );
				}
			}
		}

		MyAgent.destination = curBeaconTrans.position;
	}
}
