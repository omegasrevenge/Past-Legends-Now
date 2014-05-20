using UnityEngine;

public class NetworkChar : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	private Transform myTransform;

	void Awake()
	{
		myTransform = transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (photonView.isMine) return;

		myTransform.position = Vector3.Lerp(myTransform.position, this.correctPlayerPos, Time.deltaTime * 5);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(myTransform.position);
			stream.SendNext(myTransform.rotation);
		}
		else
		{
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
}



//using UnityEngine;
//
//public class NetworkChar : MonoBehaviour
//{
//	public double m_InterpolationBackTime = 0.03;
//	public double m_ExtrapolationLimit = 0.2;
//	public float displacementCoefficient = 0.5f;
//	
//	private Transform proxyState;
//
//	private PhotonView MyNetworkView;
//	
//	
//	internal struct State
//	{
//		internal double timestamp;
//		internal Vector3 pos;
//		internal Vector3 velocity;
//		internal Quaternion rot;
//		//internal Vector3 angularVelocity;
//	}
//	
//	// We store twenty states with "playback" information
//	State[] m_BufferedState = new State[20];
//	// Keep track of what slots are used
//	int m_TimestampCount;
//	
//	void Awake()
//	{
//		MyNetworkView = GetComponent<PhotonView> ();
//	}
//	
//	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//	{
//		if (stream.isWriting)
//		{
//			Vector3 pos = transform.position;
//			Quaternion rot = transform.rotation;
//			
//			stream.Serialize(ref pos);
//			stream.Serialize(ref rot);
//		}
//		else
//		{
//			Vector3 pos = Vector3.zero;
//			Quaternion rot = Quaternion.identity;
//
//			stream.Serialize(ref pos);
//			stream.Serialize(ref rot);
//			
//			// Shift the buffer sideways, deleting state 20
//			for (int i = m_BufferedState.Length - 1; i >= 1; i--)
//			{
//				m_BufferedState[i] = m_BufferedState[i - 1];
//			}
//
//			State state = new State();
//			state.timestamp = info.timestamp;
//			state.pos = pos;
//			state.rot = rot;
//			m_BufferedState[0] = state;
//
//			m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);
//
//			for (int i = 0; i < m_TimestampCount - 1; i++)
//			{
//				if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
//					Debug.Log("State inconsistent");
//			}
//		}
//	}
//
//	void Update()
//	{
//		if (MyNetworkView.isMine) return;
//
//		// This is the target playback time of the rigid body
//		double interpolationTime = Network.time - m_InterpolationBackTime;
//		
//		// Use interpolation if the target playback time is present in the buffer
//		if (m_BufferedState[0].timestamp > interpolationTime)
//		{
//			// Go through buffer and find correct state to play back
//			for (int i = 0; i < m_TimestampCount; i++)
//			{
//				if (m_BufferedState[i].timestamp <= interpolationTime || i == m_TimestampCount - 1)
//				{
//					// The state one slot newer (<100ms) than the best playback state
//					State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];
//					// The best playback state (closest to 100 ms old (default time))
//					State lhs = m_BufferedState[i];
//					
//					// Use the time between the two slots to determine if interpolation is necessary
//					double length = rhs.timestamp - lhs.timestamp;
//					float t = 0.0F;
//					// As the time difference gets closer to 100 ms t gets closer to 1 in 
//					// which case rhs is only used
//					// Example:
//					// Time is 10.000, so sampleTime is 9.900 
//					// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
//					// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
//					if (length > 0.0001)
//						t = (float)((interpolationTime - lhs.timestamp) / length);
//					
//					// if t=0 => lhs is used directly
//					transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t * displacementCoefficient);
//					transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
//					return;
//				}
//			}
//		}
//		// Use extrapolation
//		else
//		{
//			State latest = m_BufferedState[0];
//			
//			float extrapolationLength = (float)(interpolationTime - latest.timestamp);
//			// Don't extrapolation for more than 500 ms, you would need to do that carefully
//			if (extrapolationLength < m_ExtrapolationLimit)
//			{
//				//float axisLength = extrapolationLength * latest.angularVelocity.magnitude * Mathf.Rad2Deg;
//				//Quaternion angularRotation = Quaternion.AngleAxis(axisLength, latest.angularVelocity);
//				Quaternion angularRotation = Quaternion.identity;
//				
//				transform.localPosition = latest.pos + latest.velocity * extrapolationLength * displacementCoefficient;
//				transform.localRotation = angularRotation * latest.rot;
//				//charctrl.velocity = latest.velocity;
//				//rigidbody.angularVelocity = latest.angularVelocity;
//			}
//		}
//	}
//}