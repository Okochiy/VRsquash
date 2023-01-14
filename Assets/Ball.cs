using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    int state;  // 0: reset, 1: racket, 2: frontwall, 3: floor, 4: out
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
        // DebugUIBuilder.instance.AddLabel($"normal = {normal}, vel = {vel}, spin = {spin}");
    }

    void bounce(Vector3 normal, float coef, float friction)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        float h = Vector3.Dot(vel, normal);
        if (h > 0) return;
        Vector3 new_vel = friction * vel + (coef + friction) * (-h) * normal;
        this.GetComponent<Rigidbody>().velocity = new_vel;
        DebugUIBuilder.instance.AddLabel($"vel = {vel}, new_vel = {new_vel}");
    }

    void Out()
    {
        if (state != 0)
        {
            state = 4;
            DebugUIBuilder.instance.AddLabel("out!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        DebugUIBuilder.instance.AddLabel("collision");
    }

    // OnCollisionEnterにする(壁のTriggerをFalseにして衝突を検知する)と、跳ね返りベクトルを自作できなくなるためこちらを採用
    void OnTriggerEnter(Collider collider)
    {
        GameObject other = collider.gameObject;
        Vector3 normal = new Vector3(0.0f, 0.0f, 0.0f);
        
        switch (other.tag)
        {
            /*
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
            */
            case "FrontWall":
                normal = new Vector3(0.0f, 0.0f, -1.0f);
                bounce(normal, coef_wall, friction_wall);
                if (state != 0) state = 2;
                break;

            case "FrontOut":
                normal = new Vector3(0.0f, 0.0f, -1.0f);
                bounce(normal, coef_wall, friction_wall);
                Out();
                break;

            case "LeftWall":
                normal = new Vector3(1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                break;

            case "LeftOut":
                normal = new Vector3(1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                Out();
                break;

            case "RightWall":
                normal = new Vector3(-1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                break;

            case "RightOut":
                normal = new Vector3(-1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                Out();
                break;

            case "BackWall":
                normal = new Vector3(0.0f, 0.0f, 1.0f);
                bounce(normal, coef_wall, friction_wall);
                break;

            case "Floor":
                normal = new Vector3(0.0f, 1.0f, 0.0f);
                bounce(normal, coef_floor, friction_floor);
                DebugUIBuilder.instance.AddLabel("bounce");
                if (state == 3)
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                    state = 0;
                }
                if (state != 0) state = 3;
                break;
        }
        DebugUIBuilder.instance.AddLabel($"normal:{normal}");
        calcSpin(normal);
    }
}
