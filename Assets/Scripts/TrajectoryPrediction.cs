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
    private float gravity = 9.80665f;
    int count = 0;

    void Start()
    {
        count = 0;
    }

    void Update()
    {
        count++;
        if (count % 5 == 0 && OVRInput.Get(OVRInput.Button.Four)) DebugUIBuilder.instance.AddLabel($"vel:{ball.GetComponent<Rigidbody>().velocity.y}, height:{ball.transform.position.y}, time:{TimeToBounce(ball)}");
    }

    float TimeToBounce(float height, float vel)
    {
        return (Mathf.Sqrt(vel * vel + 2 * gravity * height) + vel) / gravity;
    }

    float TimeToBounce(GameObject obj)
    {
        return TimeToBounce(obj.transform.position.y, obj.GetComponent<Rigidbody>().velocity.y);
    }
}
