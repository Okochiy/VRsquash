using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTracker : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;
    [SerializeField] private int target = 0;
    [SerializeField] private GameObject marker;
    int length;
    Mesh mesh;
    List<GameObject> trackers;
    void Start()
    {
        mesh = targetObj.GetComponent<MeshFilter>().mesh;
        length = mesh.vertices.Length;
        trackers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            target++;
            if (target >= length) target = 0;
            DebugUIBuilder.instance.AddLabel($"target changed to {target}");
            foreach (GameObject obj in trackers)
            {
                Destroy(obj);
            }
            trackers.Clear();
        }
        if (OVRInput.Get(OVRInput.Button.Three))
        {
            GameObject newMarker = Instantiate(marker);
            newMarker.transform.parent = targetObj.transform;
            newMarker.transform.localPosition = mesh.vertices[target];
            newMarker.transform.parent = null;
            trackers.Add(newMarker);
        }
    }
}
