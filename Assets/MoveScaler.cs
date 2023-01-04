using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScaler : MonoBehaviour
{
    [SerializeField] OVRCameraRig cameraRig;
    [SerializeField] private float moveScale = (float)3.0;
    // Start is called before the first frame update
    void Start()
    {
        cameraRig.TrackingSpaceChanged += trackingSpace =>
        {
            transform.position = Vector3.Scale(trackingSpace.localPosition, new Vector3(moveScale, 0, moveScale));
        };
        DebugUIBuilder.instance.AddLabel("start");
    }

    void Update()
    {
        DebugUIBuilder.instance.AddLabel("update");
    }
}
