using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D2_Welt : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject aquarium;
    private Camera camera1;
    private Camera camera2;
    private Camera camera3;
    private Camera camera4;

    void Awake() 
    {
        Time.timeScale = 0;
        aquarium = GameObject.Find("/D2_Welt/Aquarium");
        camera1 = GameObject.Find("/D2_Welt/Camera1").GetComponent<Camera>();
        camera2 = GameObject.Find("/D2_Welt/Camera2").GetComponent<Camera>();
        camera3 = GameObject.Find("/D2_Welt/Camera3").GetComponent<Camera>();
        
        camera4 = GameObject.Find("/D2_Welt/Camera4").GetComponent<Camera>();

        // init
        aquarium.transform.position = Const.target;

        Resolution[] resolutions = Screen.resolutions;

        // Print the resolutions
        foreach (Display res in Display.displays)
        {
            
            Debug.Log(res.renderingWidth + "x" + res.renderingHeight + " : sys " + res.systemWidth+ " X "+res.systemHeight);
        }

        foreach(Display dis in Display.displays)
        {
            dis.Activate();
            
        }

        // TextAsset text_asset = Resources.Load("cameraData") as TextAsset;
        // string text = text_asset.text;

        CamData[] camData= parseData("text");
        
        setup_camera(camera1,camData[0], 0); 
        setup_camera(camera2,camData[1],-120);
        setup_camera(camera3,camData[2],-240);
        
        setup_camera(camera4,camData[3],0);


    }

    private CamData[] parseData(string text)
    {
        CamData[] camData= new CamData[4];

        camData[0] = new CamData();
        camData[1] = new CamData();
        camData[2] = new CamData();
        
        camData[3] = new CamData();

        // -- cam1
        camData[0].fov=19.8f;
        camData[0].height=-Const.height_zylinder/2f-0.04f;//-0.085f;
        camData[0].distance=1.365f+Const.radius;
        camData[0].x_rotation=0;

        // -- cam2
        camData[1].fov=19.8f;
        camData[1].height=-Const.height_zylinder/2f-0.04f;//+0.085f;
        camData[1].distance=1.365f+Const.radius;
        camData[1].x_rotation=0;

        // -- cam3
        camData[2].fov=19.8f;
        camData[2].height=-Const.height_zylinder/2f-0.04f;
        camData[2].distance=1.365f+Const.radius;
        camData[2].x_rotation=0;

        // -- cam4
        camData[3].fov=19.8f;
        camData[3].height=-Const.height_zylinder/2f-0.04f;
        camData[3].distance= - (1.365f+Const.radius);
        camData[3].x_rotation=0;

        return camData;
    }

    public struct CamData
    {
        public float fov;
        public float height;
        public float distance;
        public float x_rotation;
    }

    private void setup_camera(Camera camera, CamData camData, float rot_around)
    {
        camera.transform.position=new Vector3(0,camData.height,camData.distance);
        camera.transform.RotateAround(Vector3.zero, Vector3.up, rot_around);
        
        camera.clearFlags= CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.backgroundColor = Color.green;
     
        camera.fieldOfView=camData.fov;
        camera.nearClipPlane=0.3f;
        camera.farClipPlane = 20;
        Vector3 lookat = new Vector3(0,camera.transform.position.y,0);
        camera.transform.LookAt(lookat);

        
        
    }

    
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
