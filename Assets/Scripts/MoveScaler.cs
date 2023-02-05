using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VR‹óŠÔ‚Å‚ÌˆÚ“®—Ê‚ğ‘‚â‚·
public class MoveScaler : MonoBehaviour
{
    [SerializeField] private GameObject centerEye;
    [SerializeField] private float moveScale = (float)2.5;
    int counter;

    void Start()
    {
        DebugUIBuilder.instance.AddLabel("start");
        counter = 0;
    }

    void Update()
    {
        transform.localPosition = Vector3.Scale(centerEye.transform.localPosition, new Vector3(moveScale, 0, moveScale));
    }
}
