using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {

    // Use this for initialization
    float xRot = 114;
    float yRot = 0;
    float zRot = 180;
    
    int resX = 1280;
    int resY = 1024;
    
    private Vector3 mouseDown;
    private bool renderSelectBox = false;
    
    
    public Texture selectBoxTexture;
    public Material selectBoxMat;
    
    void Start () {
        Camera.main.transform.position = new Vector3(256,100,256);
        Camera.main.transform.rotation = Quaternion.Euler(xRot,  yRot, zRot);
        Screen.SetResolution(resX, resY, true);
    }
    
    
    private void OnGUI()
    {
        if (Input.GetMouseButtonDown(0)){
            mouseDown = Input.mousePosition;
            renderSelectBox = true;
        }
        if (Input.GetMouseButtonUp(0)){
           renderSelectBox = false;
        }
        
        Vector3 mouseCurrent;
        if (Input.GetMouseButton(0) && renderSelectBox == true){
            mouseCurrent = Input.mousePosition;
        } else {
            return;    
        }
        
        int lineWidth = 1;
        
        GUI.color = new Color(1,1,1,0.2f);
        
        GUI.DrawTexture (new Rect(Mathf.Min(mouseDown.x, mouseCurrent.x),Screen.height - Mathf.Max (mouseDown.y, mouseCurrent.y), Mathf.Abs (mouseCurrent.x - mouseDown.x), Mathf.Abs (mouseCurrent.y - mouseDown.y)), selectBoxTexture);    
        
        GUI.color = new Color(1,1,1,1.0f);
        GUI.DrawTexture (new Rect(mouseDown.x,Screen.height - Mathf.Max (mouseDown.y, mouseCurrent.y), lineWidth, Mathf.Abs (mouseCurrent.y - mouseDown.y)), selectBoxTexture);    
        GUI.DrawTexture (new Rect(mouseCurrent.x,Screen.height - Mathf.Max (mouseDown.y, mouseCurrent.y), lineWidth, Mathf.Abs (mouseCurrent.y - mouseDown.y)), selectBoxTexture);
        GUI.DrawTexture (new Rect(Mathf.Min(mouseDown.x, mouseCurrent.x),Screen.height - Mathf.Max (mouseDown.y, mouseCurrent.y), Mathf.Abs (mouseCurrent.x - mouseDown.x), lineWidth), selectBoxTexture);
        GUI.DrawTexture (new Rect(Mathf.Min(mouseDown.x, mouseCurrent.x),Screen.height - Mathf.Min (mouseDown.y, mouseCurrent.y) - lineWidth, Mathf.Abs (mouseCurrent.x - mouseDown.x), lineWidth), selectBoxTexture);
        
    }
    
    // Update is called once per frame
    void Update () {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        float x = Camera.main.transform.position.x;
        float y = Camera.main.transform.position.y;
        float z = Camera.main.transform.position.z;
        Resolution[] resolutions = Screen.resolutions;
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.rotation = Quaternion.Euler(xRot += 2, yRot, zRot);    
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.rotation = Quaternion.Euler(xRot, yRot += 2, zRot);    
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.rotation = Quaternion.Euler(xRot, yRot, zRot +=2);    
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if(scroll > 0) 
        {
             Camera.main.transform.position = new Vector3(x,y -= 10, z);  
        }
        if(scroll < 0) 
        {
             Camera.main.transform.position = new Vector3(x,y += 10,z);  
        }
        
        if(mousePos.x > Screen.width - 5) {
            Camera.main.transform.position = new Vector3(x -= 70 * Time.deltaTime, y, z);        
        }
        if(mousePos.x < 5) {
            Camera.main.transform.position = new Vector3(x += 70 * Time.deltaTime, y, z);        
        }
        if(mousePos.y > Screen.height - 5) {
            Camera.main.transform.position = new Vector3(x, y, z -= 70 * Time.deltaTime);        
        }
        if(mousePos.y <  5) {
            Camera.main.transform.position = new Vector3(x, y, z += 70 * Time.deltaTime);        
        }
    }
}
