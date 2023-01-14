using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private float speed = (float)2.0;
    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            DebugUIBuilder.instance.AddLabel($"shoot!\nnormal {this.transform.up}");
            ball.transform.position = this.transform.position;
            ball.GetComponent<Rigidbody>().velocity = speed * this.transform.up;
        }
    }
}
