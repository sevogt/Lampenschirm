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

    private bool licht_an=true;
    public void gui_intro_skip()
    {
        GameObject.Find("/D2_Welt").GetComponent<Logik2D>().intro_skip();
    }

    public void gui_schwarm()
    {
        GameObject.Find("/D2_Welt").GetComponent<Logik2D>().make_schwarm();
    }

    public void gui_licht_textur()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }
        // Texture2D licht_texture = generate_licht_texture(data);
        // GameObject.Find("/licht_maske").GetComponent<Renderer>().material.mainTexture=licht_texture;
    }

    public void gui_licht( )
    {
        
        licht_an=!licht_an;
        GameObject.Find("/D2_Welt").GetComponent<Logik2D>().licht(licht_an);
        
        var text_comp = GameObject.Find("/D2_Welt/Canvas/Licht/Licht").GetComponent<UnityEngine.UI.Text>();
        if(licht_an)
        {
            text_comp.text="Licht ausschalten";
        }
        else
        {
            text_comp.text="Licht einschalten";
        }
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
        input2.text=( Mathf.Abs(camera.transform.position.z) - (Const.radius)).ToString();

        var input3 = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        input3.text=(camera.fieldOfView).ToString();
    }

    public void gui_hohe_input()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,(-Const.height_zylinder/2f)-data,camera.transform.position.z);

    }

    public void gui_hohe_plus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        data+=0.001f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,(-Const.height_zylinder/2f)-data,camera.transform.position.z);

    }

    public void gui_hohe_minus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/hohe").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        data-=0.001f;
        
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,(-Const.height_zylinder/2f)-data,camera.transform.position.z);

    }

    public void gui_distanz_input()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,camera.transform.position.y,-(data+Const.radius));

    }

    public void gui_distanz_plus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        data+=0.001f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,camera.transform.position.y,-(data+Const.radius));

    }

    public void gui_distanz_minus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/weite").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        data-=0.001f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.transform.position=new Vector3(camera.transform.position.x,camera.transform.position.y,-(data+Const.radius));

    }

    public void gui_fov_input()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.fieldOfView=data;
    }

    public void gui_fov_plus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

        data+=0.01f;
        input.text =data.ToString();

        Camera camera = GameObject.Find("/D2_Welt/Camera"+active_beamer).GetComponent<Camera>();
        camera.fieldOfView=data;
    }

    public void gui_fov_minus()
    {
        var input = GameObject.Find("/D2_Welt/Canvas/fov").GetComponent<InputField>();
        string text = input.text;
        float data=0;
        try
        {
            data = float.Parse(text);
        }
        catch
        {
            ;
        }

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
