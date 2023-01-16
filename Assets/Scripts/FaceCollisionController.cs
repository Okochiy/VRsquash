using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ラケットを速く振った時にボールが貫通しないようにするためのもの
// 速度に応じて干渉する面を増やすことで実装

public class FaceCollisionController : MonoBehaviour
{
    [SerializeField] GameObject racketFace;
    [SerializeField] GameObject copyFace;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject marker;
    [SerializeField] int max_face = 10;
    Vector3 prev_pos;
    GameObject[] faces;
    int face_count;
    bool first = true;
    void Start()
    {
        first = true;
        marker.transform.parent = racketFace.transform;
        marker.transform.localPosition = racketFace.GetComponent<MeshFilter>().mesh.vertices[66];  // 面の先端の座標
        faces = new GameObject[max_face];
        face_count = max_face;
        for (int i = 0; i < max_face; i++)
        {
            float offset = racketFace.transform.localPosition.z * (i / 2 + 2) * ((i % 2) * 2 - 1);  // -0.06, 0.06, -0.09, 0.09, -0.12, 0.12, ...
            GameObject newFace = Instantiate(copyFace);
            newFace.transform.position = racketFace.transform.position;
            newFace.transform.rotation = racketFace.transform.rotation;
            newFace.transform.parent = racketFace.transform;
            newFace.transform.localPosition = new Vector3(0.0f, 0.0f, -0.03f + offset);
            faces[i] = newFace;
            newFace.SetActive(false);
            // DebugUIBuilder.instance.AddLabel($"push face {i} to {newFace.transform.localPosition}");
        }
    }

    void Update()
    {
        Vector3 cur_pos = marker.transform.position;
        float vel = (cur_pos - prev_pos - ball.GetComponent<Rigidbody>().velocity).magnitude;  // ボールとの相対速度の大きさ
        prev_pos = cur_pos;
        // DebugUIBuilder.instance.AddLabel($"vel:{vel}");
        ChangeFaceNum((int)(vel / 0.05));  // 速さ0.05につき1つ、干渉の面を増やす
    }

    void ChangeFaceNum(int n)
    {
        if (n > max_face) n = max_face;
        for (; face_count > n; face_count--)
        {
            faces[face_count - 1].SetActive(false);
            // DebugUIBuilder.instance.AddLabel($"delete face {face_count}");
        }
        for (; face_count < n; face_count++)
        {
            faces[face_count].SetActive(true);
            // DebugUIBuilder.instance.AddLabel($"activate face {face_count}");
        }
    }
}
