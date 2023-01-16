using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float speed = (float)2.0;
    [SerializeField] private GameObject leftHand;
    // Update is called once per frame
    void Update()
    {
        /*
        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            DebugUIBuilder.instance.AddLabel($"shoot!\nnormal {this.transform.up}");
            ball.transform.position = this.transform.position;
            ball.GetComponent<Rigidbody>().velocity = speed * this.transform.up;
        }
        */
        if (Ball.state == -3 && OVRInput.GetDown(OVRInput.Button.Four))
        {
            Toss();
        }
    }

    void Toss()
    {
        ball.transform.position = leftHand.transform.position;
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 3.0f, 0.0f);
    }
}
