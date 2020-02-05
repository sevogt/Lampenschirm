using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fisch_simple_rotation : MonoBehaviour
{
    
    private bool is_flipped = false;
    public bool do_flip;

    private Vector3 velocity = new Vector3(1,0,0);

    void Start()
    {
        
    }

    public void set_normal_orientation_left()
    {
        do_flip=true;
    }
    public void set_normal_orientation_right()
    {
        do_flip=false;
    }

    public void set_velocity(Vector3 vel)
    {
        velocity=vel;
    }


    public Vector3 get_velocity()
    {
        return velocity;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 dir = velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(velocity.x >0 && is_flipped )
        {
            is_flipped=false;
            GetComponent<SpriteRenderer>().flipY=!GetComponent<SpriteRenderer>().flipY;

        }
        else if (velocity.x <0 && !is_flipped  )
        {
            is_flipped=true;
            GetComponent<SpriteRenderer>().flipY=!GetComponent<SpriteRenderer>().flipY;
        }
    }
}
