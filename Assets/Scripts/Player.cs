using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject fadeInScreen;
    [SerializeField] private GameObject racket;
    [SerializeField] private float calib_value_pos = 0.01f;
    [SerializeField] private float calib_value_ang = 1f;
    bool fadeoutFlag, fadeinFlag, fadeinCompleted;
    ChangeAlpha screenManager;

    void Start()
    {
        fadeoutFlag = false;
        fadeinFlag = false;
        fadeinCompleted = true;
        screenManager = new ChangeAlpha(fadeInScreen.GetComponent<CanvasGroup>());
    }

    void Update()
    {

        if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick))  // ééçáÉÇÅ[ÉhÇÃêÿä∑Ç¶
        {
            fadeinFlag = true;
            screenManager.StartFadein(1f);
            if (Ball.is_game) ball.GetComponent<Ball>().EndGame();
            else ball.GetComponent<Ball>().StartGame();
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick) && fadeinCompleted)  // óßÇøà íuÇÃí≤êÆ
        {
            fadeinFlag = true;
            screenManager.StartFadein(1f);
        }
        if (fadeinFlag)
        {
            screenManager.UpdateFade();
            if (!screenManager.isFadein)
            {
                transform.parent.position = -transform.localPosition;
                DebugUIBuilder.instance.AddLabel("reset position");
                fadeinFlag = false;
                fadeoutFlag = true;
                screenManager.StartFadeout();
            }
        }
        else if (screenManager.isFadeout) screenManager.UpdateFade();

        CalibRacketPositionZ(OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y);
        CalibRacketPositionY(OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y);
        CalibRacketRotation(OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x);
    }

    bool move_forward_Z = false, move_backward_Z = false;
    void CalibRacketPositionZ(float inYAxis)
    {
        if (inYAxis > 0.5f) move_forward_Z = true;
        else if (inYAxis < -0.5f) move_backward_Z = true;
        else
        {
            if (move_forward_Z)
            {
                move_forward_Z = false;
                racket.transform.localPosition += new Vector3(0.0f, 0.0f, calib_value_pos);
            }
            if (move_backward_Z)
            {
                move_backward_Z = false;
                racket.transform.localPosition -= new Vector3(0.0f, 0.0f, calib_value_pos);
            }
        }
    }
    bool move_forward_Y = false, move_backward_Y = false;
    void CalibRacketPositionY(float inYAxis)
    {
        if (inYAxis > 0.5f) move_forward_Y = true;
        else if (inYAxis < -0.5f) move_backward_Y = true;
        else
        {
            if (move_forward_Y)
            {
                move_forward_Y = false;
                racket.transform.localPosition += new Vector3(0.0f, calib_value_pos, 0.0f);
            }
            if (move_backward_Y)
            {
                move_backward_Y = false;
                racket.transform.localPosition -= new Vector3(0.0f, calib_value_pos, 0.0f);
            }
        }
    }
    bool rotate_forward = false, rotate_backward = false;
    //Vector3 eulerRotation;
    void CalibRacketRotation(float inXAxis)
    {
        if (inXAxis > 0.5f) rotate_forward = true;
        else if (inXAxis < -0.5f) rotate_backward = true;
        else
        {
            if (rotate_forward)
            {
                rotate_forward = false;
                racket.transform.localEulerAngles += new Vector3(calib_value_ang, 0.0f, 0.0f);
                /*
                eulerRotation.y += calib_value;
                racket.transform.localRotation = Quaternion.Euler(eulerRotation);
                */
            }
            if (rotate_backward)
            {
                rotate_backward = false;
                racket.transform.localEulerAngles -= new Vector3(calib_value_ang, 0.0f, 0.0f);
                /*
                eulerRotation.y -= calib_value;
                racket.transform.localRotation = Quaternion.Euler(eulerRotation);
                */
            }
        }
    }
}
