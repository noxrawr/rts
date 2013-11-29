using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct UnitInfo
{
	public int team;
	public bool isSelectable;
	public bool isMovable;
}

public class SelectionManager : ObjectManager {

	Vector3 startPos;
    Vector3 endPos;
	//public List<GameObject> objectList = new List<GameObject>();
	public List<GameObject> selectedObjects = new List<GameObject>();
	
	public SelectionManager()
	{
	    
	}
	
	public List<GameObject> ObjectList
	{
	    set{this.objectList = value;}	
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown(1)){
			selectedObjects.Clear ();
			return;
		}
		
		if (Input.GetMouseButtonDown(0)){
			startPos = Input.mousePosition;
			//Debug.Log (startPos.x);
		}
		//Vector3 startVector = startPosWorld.GetPoint(300);
		endPos = new Vector3();
		if (Input.GetMouseButtonUp(0)){
			endPos = Input.mousePosition;
		}
		
		if(endPos.x > 0){
			selectRectangle(startPos, endPos);
			Vector3 hitCoords = Vector3.zero;
			GameObject selectByRay = selectRay(endPos, out hitCoords);
			if(selectByRay != null) {
			    processRaySelect(startPos, endPos, selectByRay, hitCoords);
			}
			//Debug.Log (selectedObjects.Count.ToString ());
		}

		GameObject.Find("GUI Text").guiText.text = selectedObjects.Count.ToString ();
	}
	
	protected void processRaySelect(Vector3 startPos, Vector3 endPos, GameObject selectedByRay, Vector3 destination)
	{
		UnitInfo info = new UnitInfo();
		bool getUnitinfo = getUnitInfo(selectedByRay, out info);
		if(!getUnitinfo && selectedObjects.Count > 0 && !isSelectBoxIntended(startPos, endPos)) {
		    orderMove (destination);	
		}
		if(!isSelectBoxIntended(startPos, endPos) && getUnitinfo == true) {
		    selectedObjects.Clear ();
			selectedObjects.Add (selectedByRay);
		}
	}
	
	protected void orderMove(Vector3 destination)
	{
		for(int i = 0; i < selectedObjects.Count; i++) {
		    //selectedObjects[i].get
			UnitInfo info = new UnitInfo();
			getUnitInfo (selectedObjects[i], out info);
			if(info.isMovable) {
				Unit currentUnit = selectedObjects[i].GetComponent("Unit") as Unit;
				currentUnit.search = true;
				currentUnit.orderMove (destination);
			}
		}
	}
	
	
	protected bool getUnitInfo(GameObject selectedObj, out UnitInfo info)
	{
		string tag = selectedObj.tag;
		info = new UnitInfo();
		
		if(tag == "Unit")
		{
			Unit selectedUnit = (Unit)selectedObj.GetComponent(tag);
			info.isSelectable = selectedUnit.isSelectable;
			info.team = selectedUnit.team;
			info.isMovable = selectedUnit.isMovable;
			return true;
		}
		else if(tag == "Building")
		{
			Building selectedBuilding = (Building)selectedObj.GetComponent(tag);
			info.isSelectable = selectedBuilding.isSelectable;
			info.team = selectedBuilding.team;
			info.isMovable = selectedBuilding.isMovable;
			
			return true;
		}
		else
		{
			return false;	
		}
	}
	
	/*
	protected void temp(GameObject selectedObj)
	{
		if(tag == "Unit")
		{
			Unit selectedUnit = (Unit)selectedObj.GetComponent(tag);
			if(selectedUnit.team == localPlayerTeam) {
			    Destroy (selectedObj);
				objectList.Remove (selectedObj);
			}
		}
		if(tag == "Building")
		{
			Building selectedBuilding = (Building)selectedObj.GetComponent(tag);
			if(selectedBuilding.team == localPlayerTeam) {
			    Destroy (selectedObj);
				objectList.Remove (selectedObj);
			}
		}
	}
	*/
	
	
	protected bool isSelectBoxIntended(Vector3 startPos, Vector3 endPos)
	{
		if((Mathf.Abs(startPos.x - endPos.x) < (float)Screen.width / 130.0f && 
			Mathf.Abs(startPos.y - endPos.y) < (float)Screen.height / 130.0f)) {
		    return false;	
		}
		return true;
	}
	
	protected void selectRectangle(Vector3 startPos, Vector3 endPos)
	{
		if(!isSelectBoxIntended(startPos, endPos)) {
		    return;	
		}
		selectedObjects.Clear ();
		for(int i = 0; i < objectList.Count; i++) {
		    Vector3 screenPos = Camera.main.WorldToScreenPoint(objectList[i].transform.position);
			
			if(screenPos.x >= Mathf.Min(startPos.x, endPos.x) && 
			   screenPos.x <= Mathf.Max(startPos.x, endPos.x) &&
			   screenPos.y <= Mathf.Max(startPos.y, endPos.y) &&
			   screenPos.y >= Mathf.Min(startPos.y, endPos.y))
			{
			    selectedObjects.Add(objectList[i]);
			}
		}
	}
	
	/*
	protected void selectRay(Vector3 endPos)
	{
		Ray posRay = Camera.main.ScreenPointToRay(endPos);
		RaycastHit hit;
		if(Physics.Raycast(posRay, out hit))
        {
			if (hit.collider != null)
			{
				GameObject selectedObj = hit.collider.gameObject;
				string tag = hit.collider.gameObject.tag;
				//Debug.Log (tag);
				if(selectedObjects.Contains (selectedObj)) {
				    return;	
				}
				if(tag == "Unit")
				{
					Unit selectedUnit = (Unit)selectedObj.GetComponent(tag);
					if(selectedUnit.team == 1 && selectedUnit.isSelectable) {
						selectedObjects.Add (selectedObj);
					}
				}
				if(tag == "Building")
				{
					Building selectedBuilding = (Building)selectedObj.GetComponent(tag);
					if(selectedBuilding.team == 1 && selectedBuilding.isSelectable) {
						selectedObjects.Add (selectedObj);
					}
				}	
			}
	    }
	}
	*/
	
	protected GameObject selectRay(Vector3 endPos, out Vector3 hitCoords)
	{
		hitCoords = Vector2.zero;
		Ray posRay = Camera.main.ScreenPointToRay(endPos);
		RaycastHit hit;
		if(Physics.Raycast(posRay, out hit))
        {
			if (hit.collider != null)
			{
				hitCoords = hit.point;
				return hit.collider.gameObject;
			}
	    }
		
		return null;
	}
}
