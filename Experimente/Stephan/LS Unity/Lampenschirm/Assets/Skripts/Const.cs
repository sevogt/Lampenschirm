using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const 
{
    public static readonly Vector3 target = new Vector3(0,0,0);
    public static readonly Vector3 sprite_centre = new Vector3(1000,0,0);
    public static readonly float initial_dist_to_centre = 1.93f + radius;
    public static readonly float initial_height_from_centre = -0.14f;
    public static readonly float radius = 0.35f;

    public static readonly int LEFT=1;
    public static readonly int RIGHT=-1; 
 

    public static readonly float startTimer = 6;
    public static readonly float camMovementTime=2f;

    public static readonly float maxSpeed=0.5f;
    public static readonly float maxSpeed_close_to_target=0.2f;
    
    public static readonly float maxSpeed_flee=1.4f;

    public static readonly float acceleration_base=0.2f;
    public static readonly float acceleration_close=-0.2f;
    public static readonly float acceleration_flee=0.5f;

    public static readonly float run_away_time = 1f; // sekunde

    public static readonly float affected_radius = 0.4f;

    public static readonly float flee_distance = 5f;

    public static readonly float wassergrenze_oben = 13.2f;
    public static readonly float wassergrenze_unten = -12f;
    public static readonly float safety_margin = 0.8f;
    public static readonly float safety_margin2 = 1.6f;

    public static readonly float world_limit_left = 9980.31f;
    public static readonly float world_limit_right = 10020.68f;

    public static readonly float world_limit_top_hard = 35f;
    public static readonly float world_limit_bottom_hard = -20f;

    public static readonly float camOrtoPos1=0;
    public static readonly float camOrtoPos2=15;

    public static readonly float buffer1 = 0.02f;

    public static readonly int höhe = 400;
    public static readonly float height_zylinder = 0.4f;
    public static readonly int weite = 2199;
    
}
