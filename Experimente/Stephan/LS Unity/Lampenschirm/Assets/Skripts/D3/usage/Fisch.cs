using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fisch : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 goal = new Vector3(0,0,0);
    private Vector3 movement = new Vector3(0,0,0);
    private float dist_last = 0;
    private float speed = 0.2f;
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
        float dist_curr = Vector3.Distance(goal, transform.position);
        
        if ( dist_curr >= dist_last) 
        {
            
            if(goal.y<0)
            {
                goal = new Vector3(0.2f,0.35f,0.0f);
            }
            else
            {
                goal = new Vector3(-0.2f,-0.35f,0.0f);
            }
            dist_curr = Vector3.Distance(goal, transform.position);
        }
        // transform.LookAt(goal);
        movement=goal - transform.position ;
        movement = Vector3.Normalize(movement);
        movement*=speed;
        transform.Translate(movement * Time.deltaTime);
        dist_last=dist_curr;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "Aquarium")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("Do something here");
        }
    }
}
