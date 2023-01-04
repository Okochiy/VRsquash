using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScaler : MonoBehaviour
{
    [SerializeField] private GameObject centerEye;
    [SerializeField] private float moveScale = (float)0.5;
    int counter;
    // Start is called before the first frame update
    void Start()
    {
        DebugUIBuilder.instance.AddLabel("start");
        counter = 0;
    }

    void Update()
    {
        counter++;
        counter = counter % 20;
        if (counter == 0)
        {
            DebugUIBuilder.instance.AddLabel($"eyelocalpos:{centerEye.transform.localPosition}\neyeworldpos:{centerEye.transform.position}");
            DebugUIBuilder.instance.AddLabel($"playerlocalpos:{transform.localPosition}\nplayerworldpos:{transform.position}");
        }
        transform.position = Vector3.Scale(centerEye.transform.localPosition, new Vector3(moveScale, 0, moveScale));
    }
}
