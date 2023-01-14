using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private Texture2D fadeoutImage;
    bool fadeoutFlag, fadeinFlag, fadeinCompleted;
    private VisualEffect visualEffect;
    // Start is called before the first frame update
    void Start()
    {
        fadeoutFlag = false;
        fadeinFlag = false;
        fadeinCompleted = true;
        visualEffect = new VisualEffect(screen.GetComponent<MeshRenderer>());
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick) && fadeinCompleted)
        {
            fadeoutFlag = true;
            visualEffect.startFadeOut(fadeoutImage, 0.02f);
        }
        if (fadeoutFlag)
        {
            if (visualEffect.updateFade())
            {
                fadeinFlag = true;
                fadeoutFlag = false;
                fadeinCompleted = false;
                visualEffect.startFadeIn(0.02f);
                transform.parent.position = -transform.localPosition;
            }
        }
        if (fadeinFlag)
        {
            if (visualEffect.updateFade())
            {
                fadeinFlag = false;
                fadeinCompleted = true;
            }
        }

    }
}
