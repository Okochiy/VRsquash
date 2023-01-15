using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static int state;  // 0: reset, 1: racket, 2: frontwall, 3: floor, 4: out

    /*----ボールを打ったときの計算関連----*/
    bool shot_by_player;  // true:player, false:opponent
    bool hitting_flag;  //連続してボールを打たないように管理するためのもの
    int calc_hit;  //ボールを打った時の挙動を計算中かどうかのフラグ
    int count = 0;  // hitting_flagをリセットするカウント
    [SerializeField] GameObject hitMarker;
    [SerializeField] float racket_coef = 0.8f;
    Vector3 prev_pos;
    Vector3 ball_vel;
    Vector3 racket_normal;
    /*----------------------------------*/

    /*
    Vector3 spin;
    public float friction_wall = (float)0.9;
    public float coef_wall = (float)0.9;
    public float friction_floor = (float)0.9;
    public float coef_floor = (float)1.0;
    */

    void Start()
    {
        state = 0;
        shot_by_player = false;
        hitting_flag = false;
        calc_hit = 0;
        hitMarker.SetActive(false);
        //spin = new Vector3(0, 0, 0);
    }

    /*
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
        // DebugUIBuilder.instance.AddLabel($"vel = {vel}, new_vel = {new_vel}");
    }
    */

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
        GameObject other = collision.gameObject;
        Vector3 normal = collision.contacts[0].normal;
        // DebugUIBuilder.instance.AddLabel("collision");
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
                if (state != 0) state = 2;
                break;

            case "FrontOut":
                Out();
                break;

            case "LeftWall":
                break;

            case "LeftOut":
                Out();
                break;

            case "RightWall":
                break;

            case "RightOut":
                Out();
                break;

            case "BackWall":
                break;

            case "Floor":
                DebugUIBuilder.instance.AddLabel("bounce");
                if (state == 3)
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                    state = 0;
                }
                if (state != 0) state = 3;
                break;
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        GameObject other = collider.gameObject;
        Vector3 normal = new Vector3(0.0f, 0.0f, 0.0f);
        if (other.tag == "Racket")
        {
            if (hitting_flag) return;
            hitting_flag = true;
            calc_hit = 1;
            racket_normal = other.transform.forward;
            hitMarker.transform.parent.position = other.transform.position;
            hitMarker.transform.position = transform.position;
            hitMarker.SetActive(true);
            prev_pos = hitMarker.transform.position;
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
        }
        /*
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
        */
    }

    void Update()
    {
        if (hitting_flag)
        {
            count++;
            if (count >= 10)
            {
                hitting_flag = false;
                count = 0;
                hitMarker.SetActive(false);
            }
        }
        if (calc_hit != 0)
        {
            calc_hit++;
            if (calc_hit >= 3)
            {
                calc_hit = 0;
                Vector3 new_vel = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 vel = (hitMarker.transform.position - prev_pos) * 5;  // 1フレーム後の位置からラケットの速度を計算し、ボールのスケールに合わせる
                if ((vel.z > 0) != (racket_normal.z > 0)) racket_normal *= -1;  // スイングの方向にラケットの法線ベクトルを向ける
                Vector3 ball_vel = GetComponent<Rigidbody>().velocity;
                float vel_abs = (vel - ball_vel).magnitude;
                new_vel = vel_abs * racket_normal;
                /*
                new_vel.z = (vel.z - ball_vel.z) * racket_coef;  // 前向きの速度成分は反射の考え方
                new_vel.x = racket_normal.x * new_vel.z / racket_normal.z;  // 横方向はラケットの向きに依存(面の向きに飛ぶようにする)
                new_vel.y = racket_normal.y * new_vel.z / racket_normal.z;  // 上下方向も基本はラケットの向きに依存
                */
                GetComponent<Rigidbody>().velocity = new_vel;
            }
        }
    }
}
