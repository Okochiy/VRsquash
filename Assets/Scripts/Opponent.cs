using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField] GameObject ball;
    [SerializeField] float shot_speed = 0.03f;
    [SerializeField] float miss_rate = 0.1f;
    [SerializeField] float force = 1.0f;
    [SerializeField] float max_move_speed = 0.1f;
    [SerializeField] float swing_range = 0.5f;
    [SerializeField] GameObject marker;
    Rigidbody rb;
    Rigidbody ball_rb;
    bool swing = false;
    float ang;
    bool shot_finish;
    bool forehand;
    float accflag = 0.0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball_rb = ball.GetComponent<Rigidbody>();
        marker.transform.position = transform.position + new Vector3(swing_range, 0.0f, 0.0f);
        marker.transform.parent = transform;
    }


    void Update()
    {
        if (swing) swing = Swing();
        else
        {
            Vector3 target = Ball.shot_by_player ? ball.transform.position : new Vector3(0.0f, 0.0f, 0.0f);
            ChaseTarget(target);

        }
        if ((ball.transform.position - transform.position).magnitude <= swing_range && Ball.shot_by_player && (Ball.state == 1 || Ball.state == 2))
        {
            swing = true;
            shot_finish = false;
            ang = 0;
            forehand = (ball.transform.position.x >= transform.position.x);
            if (forehand)
            {
                rb.rotation = Quaternion.AngleAxis(330f, Vector3.up);
            }
            else
            {
                rb.rotation = Quaternion.AngleAxis(210f, Vector3.up);
            }
        }
        
    }

    void LateUpdate()
    {
        if (accflag < 0.5f)
        {
            accflag += Time.deltaTime;
            rb.AddForce(new Vector3(0.0f, 10.0f, 0.0f), ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!swing && collision.gameObject.tag == "Floor")
        {
            accflag = 0.0f;
            //DebugUIBuilder.instance.AddLabel($"opponent bounce on floor");
        }
    }

    bool Swing()
    {
        ang += Time.deltaTime * 360;
        if (forehand)
        {
            rb.rotation = Quaternion.Euler(0, ang, 0);
        }
        else
        {
            rb.rotation = Quaternion.Euler(0, -ang, 0);
        }
        if (ang >= 10 && !shot_finish)
        {
            shot_finish = true;
            Shot();
        }
        if (ang >= 180) return false;
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
            vel.y = Random.Range(shot_speed / 2, shot_speed);
        }
        else
        {
            vel.z = shot_speed * 0.1f;
            vel.y = shot_speed * 0.3f;
        }
        ball_rb.velocity = vel;
        Ball.shot_by_player = false;
        Ball.state = 1;
    }
    
    void ChaseTarget(Vector3 target_position)
    {
        rb.AddForce(Vector3.Scale(Vector3.Normalize(target_position - transform.position), new Vector3(force, force, force)));
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, max_move_speed);
        if (rb.velocity.x >= 0) rb.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
        else rb.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);
    }
}
