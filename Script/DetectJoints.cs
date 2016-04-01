using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;


public class DetectJoints : MonoBehaviour {


	public int queueSize = 15;	
	private Queue<Quaternion> rotations;

	public int playernum=0;
	public GameObject BodySrcManager;
	public JointType TrackedJoint;
	private BodySourceManager bodyManager;
	private Body[] bodies;
	public float multiplier = 10f;
	public Vector2 player1Offset= new Vector2(10,10);
	public Vector2 player2Offset= new Vector2(10,10);
	public float zmultiplier = 10f;

	public Vector3 player1Position= new Vector3(10,10,10);
	public float player1PositionRadius=10;

	public Vector3 player2Position= new Vector3(10,10,10);
	public float player2PositionRadius=10;
	public bool limit=true;

	private int activeBodyIndex=-1;
	private int activeBody2Index = -2;

	private MenuDetectJoints tuple;

	// Use this for initialization
	void Start () {
		if (BodySrcManager == null) {

			Debug.Log ("Assign Game Object with Body Source Manager");
		}
		else {
			bodyManager = BodySrcManager.GetComponent<BodySourceManager> ();

		}


	}


	// Update is called once per frame
	void Update () {
		if (bodyManager == null) {
			return;
		} else {
			bodies = bodyManager.GetData ();


		}



		if (bodies == null) {
			return;
		}
		//int playernum1 = bodyManager.playnum ();
		//Debug.Log ("returned num" + bodies);
	


		//foreach (var body in bodies) {
		//playernum++;
			//if (playernum > 2) {
			//	continue;
			//}
			//else {

		float minZpoint = float.MaxValue;
		for (int i = 0; i < bodies.Length; i++) {

			var body = bodies [i];
				
			if (body == null) {

				continue;
			}



			if (body.IsTracked) {
				
				/* Tracking body
					trackedIds.Add (body.TrackingId);
					ulong sf1 = body.TrackingId;
					
				Debug.Log ("Tracking ID of body" + sf1 + "and player number is " + playernum);

					List<ulong> knownIds = new List<ulong> (Bodies.Keys);

					// First delete untracked bodies
					foreach (ulong trackingId in knownIds) {
						if (!trackedIds.Contains (trackingId)) {
							Destroy (Bodies [trackingId]);
							Bodies.Remove (trackingId);
							playernum--;
						}
					}

*/		
				float zMeters = body.Joints [JointType.SpineBase].Position.Z;

				if (zMeters < minZpoint) {
					Debug.Log ("value of i" + i);
					minZpoint = zMeters;
					activeBodyIndex = i;
					activeBody2Index = i-1;
				}
			}
		}

		if(activeBodyIndex != -1)
		{
			Body body =bodies[activeBodyIndex];
			if(body.IsTracked){
				bool move = false;

				//ApplyJointRotation(body);
				//if (body.TrackingId == sf1) {
				Debug.Log ("moving spine of 1st player" + body.TrackingId + "index" + activeBodyIndex);


				var spinePos = body.Joints [JointType.SpineBase].Position;
				//Debug.Log ("X:" + spinePos.X + ", Z:" + spinePos.Z);
				if (spinePos.Z > player1Position.z - player1PositionRadius &&
					spinePos.Z < player1Position.z + player1PositionRadius
					&& spinePos.X > player1Position.x - player1PositionRadius &&
					spinePos.X < player1Position.x + player1PositionRadius) {
					move = true;
				}
				if (!limit || move) {
					var	pos = body.Joints [TrackedJoint].Position;

					GameObject.Find ("Sphere").transform.position = new Vector3 ((pos.X + player1Offset.x) * multiplier, (pos.Y + player1Offset.y) * (multiplier - 7));


					//gameObject.transform.rotation = new Vector3 (pos.X,pos.Y,pos.Z);

				}
			}


			if (activeBody2Index >=0) {
				Body body1 = bodies [activeBody2Index];

				if (body1.IsTracked) {
					Debug.Log ("moving spine of 2nd body " + body1.TrackingId + "index" + activeBody2Index);

					var spinePos1	= body1.Joints [JointType.SpineBase].Position;
					//Debug.Log ("X:" + spinePos1.X + ", Z:" + spinePos1.Z);
					bool move1 = false;

					if (spinePos1.Z > player2Position.z - player2PositionRadius &&
					   spinePos1.Z < player2Position.z + player2PositionRadius
					   && spinePos1.X > player2Position.x - player2PositionRadius &&
					   spinePos1.X < player2Position.x + player2PositionRadius) {
						move1 = true;
					}
					if (!limit || move1) {

						var pos1 = body1.Joints [TrackedJoint].Position;

						GameObject.Find ("Sphere1").transform.position = new Vector3 ((pos1.X + player2Offset.x) * multiplier, (pos1.Y + player2Offset.y) * (multiplier - 7));
					}
				}
			}
				
			
				}
	}
		
	private void ApplyJointRotation(Body body)
	{
		
		Quaternion lastRotation = new Quaternion();//Utils.GetQuaternion(body.JointOrientations[this.TrackedJoint]) * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
		this.rotations.Enqueue(lastRotation);
		this.transform.rotation = SmoothFilter(this.rotations, this.transform.rotation);
		this.rotations.Dequeue();
	}

	private Quaternion SmoothFilter(Queue<Quaternion> quaternions, Quaternion lastMedian)
	{
		Quaternion median = new Quaternion(0, 0, 0, 0);

		foreach (Quaternion quaternion in quaternions)
		{
			float weight = 1 - (Quaternion.Dot(lastMedian, quaternion) / (Mathf.PI / 2)); // 0 degrees of difference => weight 1. 180 degrees of difference => weight 0.
			Quaternion weightedQuaternion = Quaternion.Lerp(lastMedian, quaternion, weight);

			median.x += weightedQuaternion.x;
			median.y += weightedQuaternion.y;
			median.z += weightedQuaternion.z;
			median.w += weightedQuaternion.w;
		}

		median.x /= quaternions.Count;
		median.y /= quaternions.Count;
		median.z /= quaternions.Count;
		median.w /= quaternions.Count;

		return NormalizeQuaternion(median);
	}

	public Quaternion NormalizeQuaternion(Quaternion quaternion)
	{
		float x = quaternion.x, y = quaternion.y, z = quaternion.z, w = quaternion.w;
		float length = 1.0f / (w * w + x * x + y * y + z * z);
		return new Quaternion(x * length, y * length, z * length, w * length);
	}

}
