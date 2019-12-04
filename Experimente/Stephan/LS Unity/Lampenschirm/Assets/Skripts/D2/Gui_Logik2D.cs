using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        ;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
