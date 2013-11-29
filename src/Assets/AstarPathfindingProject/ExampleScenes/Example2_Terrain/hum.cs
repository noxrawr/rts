using UnityEngine;
using System.Collections;
using Pathfinding;

public class hum : MonoBehaviour {

	// Use this for initialization
	
	Path p;
	
	void Start () {
	 Seeker seeker = GetComponent<Seeker>();
//Start a new path to the targetPosition, return the result to the OnPathComplete function
      seeker.StartPath (transform.position,new Vector3(200,0,150), OnPathComplete);
	}
	
	// Update is called once per frame
	void Update () {
	    if(p != null) {
			for( int i = 0; i < p.vectorPath.Count; i++) {
		transform.position = new Vector3 ( p.vectorPath[i].x, p.vectorPath[i].y, p.vectorPath[i].z);
		}
			}
	}
	
	 public void OnPathComplete (Path p) {
		this.p = p;
Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
}
}
