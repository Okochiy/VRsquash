using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField] GameObject ball;
    [SerializeField] float shot_speed = 8f;
    [SerializeField] float miss_rate = 0f;
    [SerializeField] float force = 10f;
    [SerializeField] float max_move_speed = 1.5f;
    [SerializeField] float swing_range = 1f;
    Rigidbody rb;
    Rigidbody ball_rb;
    bool swing = false;  // スイング動作中かどうか
    float ang, delang;  // スイング動作で用いる回転角
    bool forehand;  // スイングがフォアハンドかどうか
    float jumpflag = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball_rb = ball.GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (swing) swing = Swing();  // スイング動作中なら、継続
        else  // 目標位置へ動く
        {
            Vector3 target;
            if (Ball.shot_by_player && Ball.state > 0) target = TrajectoryPrediction.goal;
            else target = new Vector3(0.0f, 0.0f, 0.0f);
            ChaseTarget(target);
            if (rb.velocity.y == 0)
            {
                jumpflag = 0.1f;
            }
        }
        if ((ball.transform.position - transform.position).magnitude <= swing_range && Ball.shot_by_player && (Ball.state == 2 || Ball.state == 3))
        {
            swing = true;
            Shot();
            forehand = (ball.transform.position.x >= transform.position.x);
            delang = 0;
            if (forehand)
            {
                rb.rotation = Quaternion.AngleAxis(0f, Vector3.up);
                ang = 0;
            }
            else
            {
                rb.rotation = Quaternion.AngleAxis(180f, Vector3.up);
                ang = 180;
            }
        }
        
    }

    // 弾むように移動する
    void FixedUpdate()
    {
        if (jumpflag > 0)
        {
            if (jumpflag < 0.25)
            {
                rb.AddForce(new Vector3(0f, 20f, 0f), ForceMode.Acceleration);
                jumpflag += Time.deltaTime;
            }
            else jumpflag = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!swing && collision.gameObject.tag == "Floor")
        {
            jumpflag = 0.1f;
        }
    }

    bool Swing()
    {
        delang += Time.deltaTime * 360;
        ang += forehand ? -Time.deltaTime * 360 : Time.deltaTime * 360;
        
        rb.rotation = Quaternion.AngleAxis(ang, Vector3.up);
        if (delang >= 180) return false;
        return true;
    }

    void Shot()
    {
        DebugUIBuilder.instance.AddLabel($"shot by opponent");
        Vector3 vel = new Vector3(0.0f, 0.0f, 0.0f);
        if (Random.Range(0.0f, 1.0f) > miss_rate)
        {
            vel.z = shot_speed;
            vel.x = Random.Range(-shot_speed/2, shot_speed/2);
            vel.y = Random.Range(2*shot_speed / 3, shot_speed);
        }
        else
        {
            vel.z = shot_speed * 0.1f;
            vel.y = shot_speed * 0.3f;
        }
        ball_rb.velocity = vel;
        Ball.shot_by_player = false;
        Ball.state = 1;
        Ball.num_bounce = 0;
    }
    
    void ChaseTarget(Vector3 target_position)
    {
        rb.AddForce(Vector3.Scale(Vector3.Normalize(target_position - transform.position), new Vector3(force, 0f, force)));
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, max_move_speed);
        if (rb.velocity.x >= 0) rb.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
        else rb.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);
    }
}
