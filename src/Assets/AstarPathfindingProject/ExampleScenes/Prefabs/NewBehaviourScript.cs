using UnityEngine;
using System.Collections;
using Pathfinding;
public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	Seeker seeker;
	
	bool search;
	Path follow;
	int pathsFollowed;
	
	public Vector3 destination;
	Vector2 destinationPos;
	Vector3 lastNode;
	
	public int team;
	public float health;
	public float speed;
	public bool isSelectable;
	public bool isMovable;
	public float turnRate;
	public float acceleration;
	public float maxVelocity;
	public float currentVelocity;
	public float destinationMargin;
	public UnitState currentState;
	
	bool move;
	
	public Transform trackTarget;
	
	void Start () {
	    seeker = GetComponent<Seeker>();
		//Debug.Log ("got  her");
		search = true;
		destination = trackTarget.transform.position;
		move = false;
	}
	
	public enum UnitState
	{
	    moveOrder, 
		attackOrder, 
		idle, 
		moveAuto, 
		attackAuto, 
		attackGround, 
		attackAir, 
		passive, 
		aggressive,
		defensive,
	}
	
	// Update is called once per frame
	void Update () {
		float x = trackTarget.transform.position.x;
		float xDes = destination.x;
		if(x == xDes && !move) {
			//Debug.Log ("rejected");
			return;
		} else {
			//Debug.Log ("destination: " + destination + " tracktarget: " + trackTarget.transform.position);
			destination = trackTarget.transform.position;
		}
		//Debug.Log (destination);
		
		if(search) {
			//seeker.StartPath (transform.position, destination, OnPathComplete);
			Debug.Log ("Searching");
			search = false;
		}
		
		if(follow == null || pathsFollowed >= follow.vectorPath.Count) {
		    return;	
		}
		Debug.Log (follow.vectorPath.Count);
		Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
		if(currentState == UnitState.moveOrder) {
			float angle = getViewDeltaAngle(new Vector3(follow.vectorPath[pathsFollowed].x, follow.vectorPath[pathsFollowed].y, follow.vectorPath[pathsFollowed].z), this);
			if(pathsFollowed < follow.vectorPath.Count - 1) {
				currentVelocity = calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(destinationPos, currentPos), acceleration, false, angle);
			} else {
				currentVelocity = calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(destinationPos, currentPos), acceleration, true, angle);
			}
			//Debug.Log (Vector2.Distance (destinationPos,currentPos));
			if(Vector2.Distance(destinationPos,currentPos) <= acceleration * destinationMargin) {
			    if(pathsFollowed < follow.vectorPath.Count) {
					destinationPos = new Vector2(follow.vectorPath[pathsFollowed].x, follow.vectorPath[pathsFollowed].z);
				} else {
					Debug.Log ("it got here");
					currentVelocity = 0;
					currentState = UnitState.idle;
					search = true;
					move = false;
					Debug.Log ("node count: " + follow.vectorPath.Count  + " current node: "  + pathsFollowed);
				    return;	
			    }
				pathsFollowed++;
			}
			lastNode = new Vector3(follow.vectorPath[follow.vectorPath.Count -1].x, follow.vectorPath[follow.vectorPath.Count -1].y, follow.vectorPath[follow.vectorPath.Count -1].z);
			executeMove(new Vector3(follow.vectorPath[pathsFollowed -1].x, follow.vectorPath[pathsFollowed -1].y, follow.vectorPath[pathsFollowed -1].z) , currentVelocity, this, turnRate);
			
			//Debug.Log (destination);
			
		} else {
		    Debug.Log ("broke out");	
		}
		
		//seeker.StartPath (transform.position,new Vector3(0,0,0), OnPathComplete);
	}
	
	public void OnPathComplete (Path p) {
    	Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		follow = p;
		pathsFollowed = 0;
		destinationPos = new Vector2(follow.vectorPath[pathsFollowed].x, follow.vectorPath[pathsFollowed].z);
		currentState = UnitState.moveOrder;
		move = true;
    }
	
	public float calculateVelocity(float maxVelocity, float currentVelocity, float distanceToTarget, float acceleration, bool lastNode, float angle)
	{
		if((currentVelocity / acceleration + 1 >= distanceToTarget * 2.5 
			&& currentVelocity > acceleration - currentVelocity 
			&& lastNode)
			|| (angle > 70
		    && currentVelocity > maxVelocity / 4
		    )) {
		    Debug.Log ("got here:" + angle + " vel: " + currentVelocity);
			return currentVelocity - acceleration;	
		}
		if(currentVelocity == maxVelocity) {
			return currentVelocity;
		}
		if(currentVelocity == 0 && distanceToTarget > acceleration * 2) {
		    return acceleration;	
		}
		if(currentVelocity / acceleration + 1 <= distanceToTarget + (acceleration * 3) && currentVelocity < maxVelocity) {
		    return currentVelocity + (acceleration /5);	
		}
		
		return currentVelocity;
	}
		
	public float getViewDeltaAngle(Vector3 target, MonoBehaviour unit)
	{
		Vector3 direction = (target - transform.position).normalized;
		return Vector3.Angle (direction, transform.forward);
	}
		
	public void executeMove(Vector3 target, float velocity, MonoBehaviour unit, float turnRate)
	{
		float center =  GetComponent<MeshFilter>().mesh.bounds.extents.y * transform.lossyScale.y /2;
	    Vector3 targetDir = target;
		targetDir.y += center;
		//Debug.Log ("unit pos: " + transform.position + " target: " +  targetDir.y);
		Quaternion  rot = Quaternion.LookRotation(targetDir - transform.position);
	       float rotateVelocity = Mathf.Min (3.0f * Time.deltaTime, 1); 
	    transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotateVelocity);
		transform.Translate(Vector3.forward * Time.deltaTime * (velocity));

	}
}

