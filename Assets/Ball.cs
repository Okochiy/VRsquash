using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    int state;
    bool shot_by_player;  // true:player, false:opponent
    Vector3 spin;
    public float friction_wall = (float)0.9;
    public float coef_wall = (float)0.9;
    public float friction_floor = (float)0.9;
    public float coef_floor = (float)1.0;
    
    void Start()
    {
        state = 0;
        shot_by_player = false;
        spin = new Vector3(0, 0, 0);
    }

    void calcSpin(Vector3 normal)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        spin = Vector3.Cross(normal, vel);
        DebugUIBuilder.instance.AddLabel($"normal = {normal}, vel = {vel}, spin = {spin}");
    }

    void bounce(Vector3 normal, float coef, float friction)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        float h = Mathf.Abs(Vector3.Dot(vel, normal));
        Vector3 new_vel = friction * vel + (coef + friction) * h * normal;
        this.GetComponent<Rigidbody>().velocity = new_vel;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        Vector3 normal = collision.contacts[0].normal;
        DebugUIBuilder.instance.AddLabel($"normal:{normal}");
        calcSpin(normal);
        switch (other.tag)
        {
            case "Racket":
                if (state == 1)
                {
                    DebugUIBuilder.instance.AddLabel("shot before bouncing on front wall!");
                }
                state = 1;
                if (shot_by_player)
                {
                    DebugUIBuilder.instance.AddLabel("shot twice!");
                }
                shot_by_player = true;
                break;

            case "FrontWall":
                bounce(normal, coef_wall, friction_wall);
                state = 2;
                break;

            case "Wall":
                bounce(normal, coef_wall, friction_wall);
                break;

            case "Floor":
                bounce(normal, coef_floor, friction_floor);
                DebugUIBuilder.instance.AddLabel("bounce");
                if (state == 3)
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                }
                state = 3;
                break;

            case "OutZone":
                bounce(normal, coef_wall, friction_wall);
                state = 4;
                DebugUIBuilder.instance.AddLabel("out!");
                break;
        }
    }
}
