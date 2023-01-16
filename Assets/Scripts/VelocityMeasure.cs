using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMeasure : MonoBehaviour
{
    [SerializeField] private GameObject racketFace;
    [SerializeField] private GameObject marker;
    /*
    [SerializeField] private GameObject marker2;
    [SerializeField] private float m1_x, m1_y, m1_z;  // local position of marker1
    [SerializeField] private float m2_x, m2_y, m2_z;  // local position of marker
    Vector3 prev_pos_1, prev_pos_2;
    */
    Vector3 prev_pos;
    Vector3 offset;
    void Start()
    {
        /*
        marker1.transform.parent = racketFace.transform;
        marker2.transform.parent = racketFace.transform;
        marker1.transform.localPosition = new Vector3(m1_x, m1_y, m1_z);
        marker2.transform.localPosition = new Vector3(m2_x, m2_y, m2_z);
        */
        marker.transform.parent = racketFace.transform;
        marker.transform.localPosition = racketFace.GetComponent<MeshFilter>().mesh.vertices[66];
    }
    void Update()
    {
        /*
        Vector3 cur_pos_1 = marker1.transform.position, cur_pos_2 = marker2.transform.position;
        Vector3 v1 = cur_pos_1 - prev_pos_1, v2 = cur_pos_2 - prev_pos_2;
        prev_pos_1 = cur_pos_1;
        prev_pos_2 = cur_pos_2;
        */
        Vector3 cur_pos = marker.transform.position;
        Vector3 vel = cur_pos - prev_pos;
        prev_pos = cur_pos;
        if (OVRInput.Get(OVRInput.Button.Three))
        {
            marker.SetActive(true);
            DebugUIBuilder.instance.AddLabel($"vel:{vel.magnitude}");
        }
        
        else
        {
            /*
            marker1.SetActive(false);
            marker2.SetActive(false);
            */
            marker.SetActive(false);
        }
    }
}
