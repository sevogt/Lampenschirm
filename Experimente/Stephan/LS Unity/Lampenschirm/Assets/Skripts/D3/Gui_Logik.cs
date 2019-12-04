using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gui_Logik : MonoBehaviour
{
   
    const string aquarium = "aquarium";
    const string balle = "balle";

    private bool active = true;
    private bool einmalig_group = true;
    private string modus = aquarium;
    private GameObject toggle_group;
    private GameObject canvas_aqua;
    private GameObject canvas_ball;

    void Start()
    {
        
        active= false;
        toggle_group = GameObject.Find("/D3_Welt/Canvas/ToggleGroup");
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            if(allObjects[i].name=="CAqua")
            {
                canvas_aqua=allObjects[i];
            }
            if(allObjects[i].name=="CBall")
            {
                canvas_ball=allObjects[i];
            }
        }
    }

    public void s_modus_aquarium()
    {
        modus = aquarium;
        canvas_aqua.SetActive(true);
        canvas_ball.SetActive(false);
        
    }

    public void s_modus_balle()
    {
        modus = balle;
        canvas_aqua.SetActive(false);
        canvas_ball.SetActive(true);
    }


    public void gui_exit()
    {
        Application.Quit();
    }

    public void gui_start_ball()
    {
        if(einmalig_group)
        {
            toggle_group.SetActive(false);
            einmalig_group=false;
            GameObject.Find("/D3_Welt").GetComponent<Logik>().start_ball();
        }
        state_change();
        
    }

    public void gui_start_aqua()
    {
        if(einmalig_group)
        {
            toggle_group.SetActive(false);
            einmalig_group=false;
            GameObject.Find("/D3_Welt").GetComponent<Logik>().start_aqua();
        }
        state_change();
       
    }
    

    public void gui_reset()
    {
        active=false;
        Time.timeScale = 0;
        var txt = GameObject.Find("/D3_Welt/Canvas/Running").GetComponent<UnityEngine.UI.Text>();
        txt.text= "Pause";
        GameObject.Find("/D3_Welt").GetComponent<Logik>().reset_all();
        toggle_group.SetActive(true);
        GameObject.Find("/D3_Welt/Canvas/ToggleGroup/ToggleA").GetComponent<UnityEngine.UI.Toggle>().isOn=true;
        einmalig_group=true;
        canvas_aqua.SetActive(true);
        canvas_ball.SetActive(false);
    }

    private void state_change()
    {
        if(active)
        {
            active=false;
            Time.timeScale = 0;
            var txt = GameObject.Find("/D3_Welt/Canvas/Running").GetComponent<UnityEngine.UI.Text>();
            txt.text= "Pause";
        }
        else
        {
            active=true;
            Time.timeScale = 1;
            var txt = GameObject.Find("/D3_Welt/Canvas/Running").GetComponent<UnityEngine.UI.Text>();
            txt.text= "Running";
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
