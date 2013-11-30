using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using Pathfinding;
using System.Threading;
using System.Collections.Generic;

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

[RequireComponent(typeof(Seeker))]
public class Unit : Customobject {
    
    // Use this for initialization
    
    public UnitState currentState;
    
    Move m;
    protected Vector3 destination;
    protected int pathsFollowed;
    protected Path follow;
    public bool search;
    Vector2 destinationPos;
    
    Vector3 lastNode;
    public Texture selectBoxTexture;
    
    public Vector3[] path;
    public int pathIndex;
    bool beginPath;
    bool followPath;
    float angle;
    public Vector3 rvoPath;
    float center;
    int rr = 0;
    
    protected bool isLastNode;
    
    public Vector3 currentVel;
    
    Bounds pathfinderBounds;
    
    void Start () {
        currentVel = new Vector3(0,0,0);
        
        m = new Move();
        destination = transform.position;
        isMovable = true;
        currentState = UnitState.idle;
        //turnRate = 5;
        seeker = GetComponent<Seeker>();
        follow = null;
        search = false;
        beginPath = false;
        followPath = false;
        path = new Vector3[0];
        rvoPath = Vector3.zero;
        center =  GetComponent<MeshFilter>().mesh.bounds.extents.y * transform.lossyScale.y /2;

        currentVel = MathHelper.truncateLength(currentVel, maxVelocity);
        maxForce = 0.1f;
        mass = 120;
        
        gg = AstarPath.active.astarData.gridGraph;
        //seeker.StartPath (transform.position, new Vector3(242,0,131), OnPathComplete);
    }
    
    public bool Equals(Customobject obj)
    {
        if(obj == null) {
            return false;    
        }
        if(obj.transform.position == transform.position && obj.transform.eulerAngles == transform.eulerAngles) {
            return true;    
        }
        return false;
    }
    
    // Update is called once per frame
    /*
    void Update () {
        if(search) {
            seeker.StartPath (transform.position, destination, OnPathComplete);
            search = false;
        }
        if(follow == null || pathsFollowed >= follow.vectorPath.Count) {
            //Debug.Log ("busted out");
            return;    
        }
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        if(currentState == UnitState.moveOrder) {
            float angle = m.getViewDeltaAngle(new Vector3(follow.vectorPath[pathsFollowed].x, follow.vectorPath[pathsFollowed].y, follow.vectorPath[pathsFollowed].z), this);
            if(pathsFollowed < follow.vectorPath.Count) {
                currentVelocity = m.calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(destinationPos, currentPos), acceleration, false, angle);
            } else {
                currentVelocity = m.calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(destinationPos, currentPos), acceleration, true, angle);
            }
            //Debug.Log (Vector2.Distance (destinationPos,currentPos));
            if(Vector2.Distance(destinationPos,currentPos) <= acceleration * destinationMargin) {
                if(pathsFollowed < follow.vectorPath.Count) {
                    destinationPos = new Vector2(follow.vectorPath[pathsFollowed].x, follow.vectorPath[pathsFollowed].z);
                } else {
                    //currentVelocity = 0;
                    follow = null;
                    currentState = UnitState.idle;
                    Debug.Log ("node count: " + follow.vectorPath.Count  + " current node: "  + pathsFollowed);
                    return;    
                }
                pathsFollowed++;
            }
            //Debug.Log ("currentpos: "+ transform.position + " end: " + lastNode);
            m.executeMove(new Vector3(follow.vectorPath[pathsFollowed -1].x, follow.vectorPath[pathsFollowed -1].y, follow.vectorPath[pathsFollowed -1].z) , 10, this, turnRate);
            
            //Debug.Log (destination);
            
        } else {
            Debug.Log ("broke out");    
        }
    }
    */
    
    void Update () {
        if(search == true) {
            setPenalty (0);
            seeker.StartPath (transform.position, destination, OnPathComplete);    
            search = false;
        }
         if(currentState == UnitState.moveOrder) {
            if(path.Length > 0) {
                angle = m.getViewDeltaAngle(path[pathIndex], this);    
            }
            if(beginPath) {
                currentVelocity = m.calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(toVector2(path[pathIndex]), toVector2(transform.position)), acceleration, false, angle);
                //m.executeMove ();
                //m.executeMove(path[pathIndex] , currentVelocity, this, turnRate, center);
                m.seek(path[pathIndex] , currentVelocity, this, turnRate, center, false);
                //m.executeMove(rvoPath , currentVelocity, this, turnRate);
                //Debug.Log ("detination:" + path[pathIndex] + " current: " + transform.position);
                
                followPath = true;
                beginPath = false;
                return;
            }
            if(followPath){
                //Debug.Log ("following");
                if(pathIndex == path.Length -1) {
                    isLastNode = true;
                    currentVelocity = m.calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(toVector2(path[pathIndex]), toVector2(transform.position)), acceleration, true, angle);    
                } else {
                    isLastNode = false;
                    currentVelocity = m.calculateVelocity(maxVelocity, currentVelocity, Vector2.Distance(toVector2(path[pathIndex]), toVector2(transform.position)), acceleration, false, angle);
                }
                float delta = Vector3.Distance(transform.position, path[pathIndex]);
                //Debug.Log (delta);
                //m.executeMove(path[pathIndex] , currentVelocity, this, turnRate, center);
                m.seek(path[pathIndex] , currentVelocity, this, turnRate, center, isLastNode);
                //m.executeMove(rvoPath , currentVelocity, this, turnRate);
                if(delta < 18) {
                    //Debug.Log (path.Length);
                    if(pathIndex == path.Length -1) {
                        followPath = false;
                        currentState = UnitState.idle;
                        setPenalty(10000);
                        return;
                    }
                    pathIndex++;
                    //Debug.Log (path[pathIndex] + " current" + transform.position);
                }
            }
            //m.executeMove ();    
        }
        
    }
    
    public void setPenalty(uint penalty)
    {
        pathfinderBounds = transform.collider.bounds;
        pathfinderBounds.size = transform.collider.bounds.size * 1.8f;
        
        IntRect clampedRect = MathHelper.getBoundsRect(pathfinderBounds, gg);
        for (int x = clampedRect.xmin; x <= clampedRect.xmax;x++) {
            for (int z = clampedRect.ymin;z <= clampedRect.ymax;z++) {
                int index = z* gg.width+x;
                gg.nodes[index].penalty = penalty;
            }    
        }    
    }
    
    void LateUpdate() {
       
    }
    
    protected Vector2 toVector2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    
    private void OnGUI()
    {
        Vector3 loc = Vector3.zero;
        Vector3 screenPos3 = Camera.main.WorldToScreenPoint(transform.position + currentVel * 250);
    
        GUI.color = new Color(1,1,1,1.0f);
        GUI.DrawTexture (new Rect(screenPos3.x -4 ,Screen.height - screenPos3.y -2, 4, 4), selectBoxTexture);

    }
    
     public void OnPathComplete (Path p) {
        //this.follow = p;
        //pathsFollowed = 0;
        //destinationPos = new Vector2(follow.vectorPath[pathsFollowed].x, follow.vectorPath[pathsFollowed].z);
        //lastNode = new Vector3(follow.vectorPath[follow.vectorPath.Count -1].x, follow.vectorPath[follow.vectorPath.Count -1].y, follow.vectorPath[follow.vectorPath.Count -1].z);
        //Debug.Log ("x:" + p.vectorPath[0].x+  " y: " + p.vectorPath[0].y + " z: " + p.vectorPath[0].z);
        //Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
        
        this.path = p.vectorPath.ToArray ();

        this.pathIndex = 0;
        beginPath = true;
     }
    
    public void orderMove(Vector3 destination)
    {
        this.destination = destination;
        currentState = UnitState.moveOrder;
        //Move m = new Move();
        //m.executeMove(destination, this.transform.eulerAngles, 1, this);
        //Debug.Log ("X: " + destination.x + " Y: " + destination.y + " Z: " + destination.z);
    }
}
