using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logik2D : MonoBehaviour
{
    private GameObject aquarium;
    private GameObject sprite1;
    private GameObject sprite2;



    void Start()
    {
        aquarium = GameObject.Find("/D2_Welt/Aquarium");

        sprite1 = GameObject.Find("/D2_Welt/Sprites/Circle1");
        sprite2 = GameObject.Find("/D2_Welt/Sprites/Circle2");
        
    }

    public void setup_aqua()
    {

    }

    // Update is called once per frame
    void Update()
    {

        sprite1.transform.Translate(new Vector3(1f,0f,0.0f) * Time.deltaTime);
        sprite2.transform.Translate(new Vector3(-1f,0f,0.0f) * Time.deltaTime);

        if(sprite2.transform.position.x<9900)
        {
             sprite2.transform.position=new Vector3(11000,0f,0.0f);
        }
        if(sprite1.transform.position.x>10100)
        {
            sprite1.transform.position=new Vector3(8900,0f,0.0f);
        }

    }
}
