using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPrediction : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject backWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject floor;
    public bool debug = true;
    public static Vector3 goal;

    private float gravity = 9.80665f;
    private float sideBounciness, frontBounciness, backBounciness;  // 壁の跳ね返り
    private float rightlimit, leftlimit, frontlimit, backlimit; // 壁の座標

    void Start()
    {
        goal = new Vector3(0f, 0f, 0f);

        sideBounciness = (ball.GetComponent<Collider>().material.bounciness + leftWall.GetComponent<Collider>().material.bounciness) / 2;
        frontBounciness = (ball.GetComponent<Collider>().material.bounciness + frontWall.GetComponent<Collider>().material.bounciness) / 2;
        backBounciness = (ball.GetComponent<Collider>().material.bounciness + backWall.GetComponent<Collider>().material.bounciness) / 2;
        
        rightlimit = rightWall.transform.position.x;
        leftlimit = leftWall.transform.position.x;
        frontlimit = frontWall.transform.position.z;
        backlimit = backWall.transform.position.z;
    }

    void Update()
    {
        float[] pred = PointOfBounceTwice();
        goal = new Vector3(pred[0], 0f, pred[1]);
    }

    float TimeToBounce(float height, float vel)  // 地面にバウンドするまでの時間
    {
        return (Mathf.Sqrt(vel * vel + 2 * gravity * height) + vel) / gravity;
    }

    float TimeToBounce()
    {
        return TimeToBounce(ball.transform.position.y, ball.GetComponent<Rigidbody>().velocity.y);
    }

    float TimeToBounceTwice()  // ボールが地面に2バウンドするまでの時間
    {
        if (Ball.num_bounce > 1) return 0f; 
        if (Ball.num_bounce == 1) return TimeToBounce();
        float height = ball.transform.position.y;
        float vel = ball.GetComponent<Rigidbody>().velocity.y;
        float bounciness = (ball.GetComponent<Collider>().material.bounciness + floor.GetComponent<Collider>().material.bounciness) / 2;
        return TimeToBounce() + TimeToBounce(0f, bounciness * Mathf.Sqrt(vel * vel + 2 * gravity * height));
    }

    float[] NextBounceOnWall(float[] arr)
    {
        float pos_x = arr[0], pos_z = arr[1], vel_x = arr[2], vel_z = arr[3], time = arr[4], limit = arr[5];
        float[] timesToWall = new float[4];
        timesToWall[0] = (rightlimit - pos_x) / vel_x;  // rightwall
        timesToWall[1] = (leftlimit - pos_x) / vel_x;  // leftwall
        timesToWall[2] = (frontlimit - pos_z) / vel_z;  // frontwall
        timesToWall[3] = (backlimit - pos_z) / vel_z;  // backwall
        float delta = 100;
        int bounceWall = -1;
        
        for (int i = 0; i < 4; i++)
        {
            if (timesToWall[i] > 0 && timesToWall[i] < delta)
            {
                delta = timesToWall[i];
                bounceWall = i;
            }
        }

        if (time + delta > limit) delta = limit - time;  // 壁に跳ね返るより先に落下する場合
        time += delta;
        
        float[] res = new float[6];
        
        res[0] = pos_x + vel_x * delta;
        res[1] = pos_z + vel_z * delta;
        switch (bounceWall)
        {
            case 0:  // 右壁
                res[2] = -vel_x * sideBounciness;
                res[3] = vel_z;
                break;

            case 1:  // 左壁
                res[2] = -vel_x * sideBounciness;
                res[3] = vel_z;
                break;

            case 2:  // 前壁
                res[2] = vel_x;
                res[3] = -vel_z * frontBounciness;
                break;

            case 3:  // 後壁
                res[2] = vel_x;
                res[3] = -vel_z * backBounciness;
                break;
        }
        res[4] = time;
        res[5] = limit;
        
        if (debug)
        {
            string[] wallnames = new string[4] { "right", "left", "front", "back" };
            DebugUIBuilder.instance.AddLabel($"bounce on {wallnames[bounceWall]} wall");
        }

        return res;
    }

    float[] PointOfBounceTwice()  // ボールが2回目のバウンドをする場所を予測する
    {
        Vector3 pos = ball.transform.position, vel = ball.GetComponent<Rigidbody>().velocity;
        float limit = TimeToBounceTwice();
        float[] arr = new float[6] { pos.x, pos.z, vel.x, vel.z, 0f, limit };
        while (arr[4] != arr[5])
        {
            arr = NextBounceOnWall(arr);
        }
        return new float[2] { arr[0], arr[1] };
    }
}
