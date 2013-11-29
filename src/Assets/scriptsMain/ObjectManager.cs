using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RVO;
using Pathfinding;

public class ObjectManager : MonoBehaviour {

    // Use this for initialization
    //public GameObject cube;
    public List<GameObject> objectList = new List<GameObject>();
    
    public GameObject cube;
    public GameObject powerPlant;
    public GameObject humvee;
    
    protected SelectionManager selectBox;

    protected int localPlayerTeam = 1;
    
    protected RVO.Vector2[] targetList;
    
    public List<Unit> unitList = new List<Unit>();
    
    
    GridGraph gg;
    
    public void updateGraph()
    {
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
        for(int i = 0; i < allUnits.Length; i ++) {
            Unit unit = allUnits[i].GetComponent("Unit") as Unit;
            if(unit.currentState == UnitState.idle) {
                Bounds b = unit.collider.bounds;
                b.size = b.size * 1.8f;

                
                IntRect clampedRect = MathHelper.getBoundsRect(b, gg);
                //IntRect gridRect = new IntRect(0,0,gg.width -1, gg.depth -1);
                //IntRect clampedRect = IntRect.Intersection (originalRect, gridRect);
                for (int x = clampedRect.xmin; x <= clampedRect.xmax;x++) {
                    for (int z = clampedRect.ymin;z <= clampedRect.ymax;z++) {
                        int index = z* gg.width+x;
                        gg.nodes[index].penalty = 10000;
                    }    
                }    
            }
        }
        //AstarPath.active.UpdateGraphs (new Bounds(Vector3.forward,new Vector3(5,5,5)));
    }
    
    protected void Start () {
        gg = AstarPath.active.astarData.gridGraph;
        //InvokeRepeating("updateGraph", 0, 1);
        updateGraph();
        selectBox = gameObject.AddComponent ("SelectionManager") as SelectionManager;

        Simulator.Instance.setTimeStep(1);
        //sim.setTimeStep (0.25f);
        Simulator.Instance.setAgentDefaults(30, 20, 30, 15, 3.5f, 10, new RVO.Vector2());
        
        
        QualitySettings.antiAliasing = 8;
        System.Random rng = new System.Random();
        int randomX;
        int randomY;
        int randomZ;
        
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
        for(int i = 0; i < allUnits.Length; i ++) {
            Unit unit = allUnits[i].GetComponent("Unit") as Unit;
            //unit.rotation = allUnits[i].transform.eulerAngles;
            unit.team = localPlayerTeam;
            objectList.Add (allUnits[i]);
            //Simulator.Instance.addAgent (new RVO.Vector2(unit.transform.position.x, unit.transform.position.z));
            //RVO.Vector2 goalVector = new RVO.Vector2(unit.transform.position.x, unit.transform.position.z) - Simulator.Instance.getAgentPosition(i);
            //Simulator.Instance.setAgentPrefVelocity (i, new RVO.Vector2(unit.transform.position.x, unit.transform.position.z));
            //if (RVOMath.absSq(goalVector) > 1.0f)
            {
                //goalVector = RVOMath.normalize(goalVector) * Time.deltaTime * 10;
                //Simulator.Instance.setAgentPrefVelocity(i, goalVector);
                //realGoal = (realGoal - unitPos).normalized;
            }
            unitList.Add (unit);
            //guo = new GraphUpdateObject(unit.collider.bounds);
            //guo.addPenalty = 0;
            //AstarPath.active.UpdateGraphs (guo);
        }
        for(int i = 0; i < unitList.Count; i++) {
            for(int z = 0; z < unitList.Count; z++) {
                unitList[i].addObstacle (unitList[z]);
            }
        }
        //Debug.Log (Simulator.Instance.getNumAgents ());
        allUnits = GameObject.FindGameObjectsWithTag("Building");
        for(int i = 0; i < allUnits.Length; i ++) {
            Building unit = allUnits[i].GetComponent("Building") as Building;
            //unit.rotation = allUnits[i].transform.eulerAngles;
            unit.team = localPlayerTeam;
            objectList.Add (allUnits[i]);
        }


        selectBox.ObjectList = objectList;
    }
    
    // Update is called once per frame
    void Update () {
        /*
        //Debug.Log(objectList.Count);
        bool goalReached = true;
        Simulator.Instance.doStep ();
        for(int i = 0; i < objectList.Count; i++) {
            //obj.speed = 0;
            Unit unit = objectList[i].GetComponent("Unit") as Unit;
            //obj.rigidbody.AddForce (transform.right * 1);
            if(unit == null  || unit.path == null || unit.path.Length < 1) {
                continue;    
            }
            
            RVO.Vector2 target = new RVO.Vector2(unit.path[unit.pathIndex].x, unit.path[unit.pathIndex].z);
            //RVO.Vector2 currentPos = new RVO.Vector2(unit.transform.position.x, unit.transform.position.z);
            //Debug.Log (Simulator.Instance.getAgentPosition(i));
            UnityEngine.Vector2 unitPos = new UnityEngine.Vector2(unit.path[unit.pathIndex].x, unit.path[unit.pathIndex].z);
            //UnityEngine.Vector2 realGoal = UnityEngine.Vector3.Normalize (new Vector3(), new Vector3());
            RVO.Vector2 goalVector = target - Simulator.Instance.getAgentPosition(i);
            RVO.Vector2 realDest = Simulator.Instance.getAgentPosition(i);
            //Debug.Log (goalVector.x () + " " + goalVector.y ());
            RVO.Vector2 currentSimPos = Simulator.Instance.getAgentPosition(i);
            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector) * Time.deltaTime * 25;
                //realGoal = (realGoal - unitPos).normalized;
            }
            Simulator.Instance.setAgentPrefVelocity(i, goalVector);
            //unit.rvoPath = new Vector3(realDest.x (), unit.path[unit.pathIndex].y, realDest.y ());
            
            Quaternion  rot = Quaternion.LookRotation(new Vector3(currentSimPos.x (),unit.path[unit.pathIndex].y , currentSimPos.y ()) - unit.transform.position);
            float rotateVelocity = Mathf.Min (3.0f * Time.deltaTime, 1); 
            //unit.transform.rotation = Quaternion.Lerp(unit.transform.rotation, rot, rotateVelocity);
            
            
            //unit.transform.position = new Vector3(realDest.x (), unit.path[unit.pathIndex].y, realDest.y ());
            if (RVOMath.absSq(Simulator.Instance.getAgentPosition(i) -  goalVector) > 20.0f * 20.0f)
            {
                goalReached = false;
                
            }
        }
        if(goalReached) {
            for(int i = 0; i < Simulator.Instance.getNumAgents(); i++) {
                //Debug.Log (Simulator.Instance.getAgentPosition(i) + " " + i);    
            }
        }
        
        //Vector3 pos = new Vector3(1000,150, 1000);
        
        //obj.rigidbody.AddTorque (0, 10, 0);
        
        //Customobject troep = new Customobject(Cube);
        //Instantiate(troep.obj, pos, Quaternion.identity);
        //GameObject myCube = (GameObject)Instantiate(Resources.Load("cube"), pos, Quaternion.identity);
        //Camera.main.transform.position = new Vector3(10, 10, 10);    
        */
    }
}
