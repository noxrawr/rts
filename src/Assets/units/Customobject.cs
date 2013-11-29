using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class Customobject : MonoBehaviour {

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
	public float maxForce;
	public float mass;
	public float slowingRadius;
	public float maxCollisionDistance;
	public float maxAvoidForce;
	
	public GridGraph gg;
	
	protected List<Unit> obstacleList;
	
	protected GameObject gameObject;
	protected Seeker seeker;
	#region Properties

	#endregion
	
	public List<Unit> Obstacles
	{
	    set{ this.obstacleList = value;}
		get{return this.obstacleList;}
	}
	
	public void addObstacle(Unit unit)
	{
		if(obstacleList == null) {
			obstacleList = new List<Unit>();
		}
		if(!unit.Equals (this)) {
		    obstacleList.Add (unit);	
		}
	}
	
	void Start () {
		//Debug.Log ("seeker:" + seeker);
	    //Vector3 pos = new Vector3(1000,150, 1000);
		//Instantiate(spawnObject, pos, Quaternion.identity);
	}
	
	public void instantiate(GameObject cube)
	{

	}
	
	// Update is called once per frame
	void Update () {

	}
}
