using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gui_Logik2D : MonoBehaviour
{
    // Start is called before the first frame update

    private bool running=false;
    private bool first=true;
    private UnityEngine.UI.Text start_button_text;
    void Start()
    {
        start_button_text = GameObject.Find("/D2_Welt/Canvas/Start/Start").GetComponent<UnityEngine.UI.Text>();
        
    }

    public void gui_exit()
    {
        Application.Quit();
    }

    public void gui_mehr()
    {
        GameObject.Find("/D2_Welt").GetComponent<Logik2D>().mehr(3);
    }

    public void gui_start_aqua()
    {
        if(running)
        {
            running=false;
            start_button_text.text="Start";
            Time.timeScale = 0;
        }
        else
        {
            running=true;
            start_button_text.text="Stop";
            Time.timeScale = 1;

            if(first)
            {
                GameObject.Find("/D2_Welt").GetComponent<Logik2D>().setup_aqua();
                first=false;
            }
        } 
    }

    private int active_beamer = 1;

    public void gui_setup_beamer(int num)
    {
        active_beamer=num;

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();

        var input1 = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        input1.text=Mathf.Abs((camera.transform.position.y + (Const.height_zylinder/2f))).ToString();

        var input2 = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        input2.text=(camera.transform.position.z - (Const.radius)).ToString();

        var input3 = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        input3.text=(camera.fieldOfView).ToString();
    }

    public void gui_hohe_input()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,(-Const.height_zylinder/2f)-data,camera.transform.position.z);

    }

    public void gui_hohe_plus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        data+=0.001f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,(-Const.height_zylinder/2f)-data,camera.transform.position.z);

    }

    public void gui_hohe_minus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        data-=0.001f;
        
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,(-Const.height_zylinder/2f)-data,camera.transform.position.z);

    }

    public void gui_distanz_input()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,camera.transform.position.y,data+Const.radius);

    }

    public void gui_distanz_plus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        data+=0.001f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,camera.transform.position.y,data+Const.radius);

    }

    public void gui_distanz_minus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        data-=0.001f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,camera.transform.position.y,data+Const.radius);

    }

    public void gui_fov_input()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.fieldOfView=data;
    }

    public void gui_fov_plus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        data+=0.01f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.fieldOfView=data;
    }

    public void gui_fov_minus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        string text = input.text;
        float data = float.Parse(text);

        data-=0.01f;
        input.text =data.ToString();



        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.fieldOfView=data;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
