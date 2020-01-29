using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRender : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject maske;
    string nummer="";
    string nam="";
    void Start()
    {
        nummer = gameObject.name.Substring(gameObject.name.Length-1);
        maske = GameObject.Find("/D2_Welt/ZylinderMaske"+nummer);
        // Debug.Log(nummer+"  "+nam);
        nam = maske.name;
  
    }

    private void OnPreCull() 
    {
        maske.GetComponent<Renderer>().enabled=true;
        // Debug.Log("pre "+nummer+" "+nam);
        
    }

    private void OnPostRender() 
    {
        maske.GetComponent<Renderer>().enabled=false;
        // Debug.Log("post "+nummer+" "+nam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
