using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public static int state;  //-3: �X�R�A�̃t�F�[�h�A�E�g, -2: �X�R�A�̕\��(�v���C���[�̓��͑҂�), -1: �X�R�A������������,  0: reset, 1: racket, 2: frontwall, 3: floor

    /*----�{�[����ł����Ƃ��̌v�Z�֘A----*/
    public static bool shot_by_player;  // true:player, false:opponent
    bool hitting_flag;  //�A�����ă{�[����ł��Ȃ��悤�ɊǗ����邽�߂̂���
    int calc_hit;  //�{�[����ł������̋������v�Z�����ǂ����̃t���O
    int count = 0;  // hitting_flag�����Z�b�g����J�E���g
    [SerializeField] GameObject hitMarker;
    [SerializeField] float max_speed = 10.5f;
    Vector3 prev_pos;
    Vector3 ball_vel, prev_ball_vel;
    Vector3 racket_normal;
    /*----------------------------------*/
    public static bool is_game;
    static int player_score;
    static int opponent_score;
    int reset_count = 0;
    [SerializeField] GameObject scoreImage;
    [SerializeField] GameObject infoImage;
    [SerializeField] GameObject playerScoreText;
    [SerializeField] GameObject opponentScoreText;
    [SerializeField] private GameObject leftHand;
    [SerializeField] GameObject racket;
    ChangeAlpha changeScore, changeInfo;  // �X�R�A��\�����邽�߂̃I�u�W�F�N�g

    int tmp = 0;

    /*
    Vector3 spin;
    public float friction_wall = (float)0.9;
    public float coef_wall = (float)0.9;
    public float friction_floor = (float)0.9;
    public float coef_floor = (float)1.0;
    */

    void Start()
    {
        state = -1;
        shot_by_player = false;
        hitting_flag = false;
        calc_hit = 0;
        hitMarker.SetActive(false);
        changeScore = new ChangeAlpha(scoreImage.GetComponent<CanvasGroup>());
        changeInfo = new ChangeAlpha(infoImage.GetComponent<CanvasGroup>());

        //spin = new Vector3(0, 0, 0);
    }

    void Out()
    {
        if (state > 0)
        {
            state = 4;
            //DebugUIBuilder.instance.AddLabel("out!");
            Score(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state == 0)
        {
            transform.position = leftHand.transform.position;
            GetComponent<Rigidbody>().useGravity = false;
        }
        GameObject other = collision.gameObject;
        Vector3 normal = collision.contacts[0].normal;
        // DebugUIBuilder.instance.AddLabel("collision");
        switch (other.tag)
        {
            
            case "Racket":
                //if (state == 1)
                //{
                //    DebugUIBuilder.instance.AddLabel("shot before bouncing on front wall!");
                //}
                //state = 1;
                //if (shot_by_player)
                //{
                //    DebugUIBuilder.instance.AddLabel("shot twice!");
                //}
                //shot_by_player = true;
                hitting_flag = true;
                calc_hit = 1;
                ball_vel = prev_ball_vel;
                racket.GetComponent<MeshCollider>().enabled = false;  // �A���Փ˂������
                DebugUIBuilder.instance.AddLabel("hit");
                racket_normal = other.transform.forward;
                hitMarker.transform.parent = other.transform;
                hitMarker.transform.position = transform.position;
                hitMarker.SetActive(true);
                prev_pos = hitMarker.transform.position;
                break;
            
            case "FrontWall":
                if (state == 1) state = 2;
                break;

            case "FrontOut":
                Out();
                break;

            case "LeftWall":
                break;

            case "LeftOut":
                Out();
                break;

            case "RightWall":
                break;

            case "RightOut":
                Out();
                break;

            case "BackWall":
                break;

            case "Floor":
                DebugUIBuilder.instance.AddLabel("bounce");
                if (state == 3)   // ���łɃ{�[����1�o�E���h������̏ꍇ
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                    Score(true);  // �ł��Ԃ��Ȃ������v���C���[�̎��_
                }
                else if (state == 2)  // �{�[�����O�ǂɓ͂��Ȃ������ꍇ
                {
                    Score(false);  // �����ł����v���C���[�̎��_
                }
                else if (state > 0) state = 3;
                break;

            case "Opponent":
                if (state > 0)
                {
                    player_score++;
                    state = -1;
                }
                break;
        }
    }

    void Update()
    {
        if (is_game)
        {
            if (state == -1)  // �X�R�A�̕\�����X�^�[�g
            {
                playerScoreText.GetComponent<Text>().text = player_score.ToString();
                opponentScoreText.GetComponent<Text>().text = opponent_score.ToString();
                changeScore.StartFadein(0.7f);
                changeInfo.StartLoop(1f);
                DebugUIBuilder.instance.AddLabel("start fade in");
                state = -2;
                return;
            }
            if (state == -2)  // �X�R�A��\�����Ȃ���v���C���[�̓��͑҂�
            {
                changeInfo.UpdateFade();
                changeScore.UpdateFade();
                if (OVRInput.GetDown(OVRInput.Button.Three))  // ���͂������-3�֑J��
                {
                    changeScore.StartFadeout();
                    changeInfo.StartFadeout();
                    state = -3;
                }
                return;
            }
            if (state == -3)  // �X�R�A�̃t�F�[�h�A�E�g
            {
                changeInfo.UpdateFade();
                changeScore.UpdateFade();
                if (!(changeInfo.isFadeout || changeScore.isFadeout)) state = 0;  // �X�R�A�\�����t�F�[�h�A�E�g�����珉����Ԃ�
                return;
            }
        }
        
        prev_ball_vel = GetComponent<Rigidbody>().velocity;

        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            Toss();
        }

        if (hitting_flag)
        {
            count++;
            if (count >= 10)
            {
                hitting_flag = false;
                count = 0;
                //hitMarker.SetActive(false);
                // DebugUIBuilder.instance.AddLabel("hitting_flag reset");
            }
        }

        if (calc_hit != 0)
        {
            calc_hit++;
            if (calc_hit == 3)
            {
                Vector3 new_vel = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 vel = (hitMarker.transform.position - prev_pos) / Time.deltaTime;  // 1�t���[����̈ʒu���烉�P�b�g�̑��x���v�Z���A�{�[���̃X�P�[���ɍ��킹��
                if ((vel.z > 0) != (racket_normal.z > 0)) racket_normal *= -1;  // �X�C���O�̕����Ƀ��P�b�g�̖@���x�N�g����������
                float vel_abs = (vel - ball_vel).magnitude;
                if (vel_abs > max_speed) vel_abs = max_speed;  // �X�C���O����������ꍇ�͐�������
                new_vel = vel_abs * racket_normal;
                GetComponent<Rigidbody>().velocity = new_vel;
                /*if (OVRInput.Get(OVRInput.Button.Three))
                {
                    //DebugUIBuilder.instance.AddLabel($"swing_vel: {vel}\nracket_normal: {racket_normal}");
                }*/
            }
            if (calc_hit >= 10)
            {
                calc_hit = 0;
                racket.GetComponent<MeshCollider>().enabled = true;
            }
        }

        ChangeMaxSpeed(OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x);  // max_speed���E�X�e�B�b�N�̍��E�ŕύX


        if (!shot_by_player && state == 1) gameObject.layer = 6;
        else gameObject.layer = 0;

        //tmp++;
        //if (tmp % 20 == 0) DebugUIBuilder.instance.AddLabel($"{state}");
    }

    public void StartGame()
    {
        is_game = true;
        player_score = 0;
        opponent_score = 0;
        state = -1;
    }

    public void EndGame()
    {
        is_game = false;
        changeInfo.Destroy();
        changeScore.Destroy();
    }

    void Score(bool pointByHit)
    {
        if (pointByHit)
        {
            if (shot_by_player) player_score++;
            else opponent_score++;
        }
        else
        {
            if (shot_by_player) opponent_score++;
            else player_score++;
        }
        DebugUIBuilder.instance.AddLabel($"player:{player_score}\ncpu:{opponent_score}");
        state = -1;
    }

    void Toss()  // �T�[�u��łۂ̃g�X
    {
        transform.position = leftHand.transform.position;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 3.0f, 0.0f);
        state = 1;
    }

    void ChangeMaxSpeed(float inXAxis)
    {
        if (inXAxis > 0.5f)
        {
            max_speed += 0.1f;
            DebugUIBuilder.instance.AddLabel($"max speed: {max_speed}");
        }
        else if (inXAxis < -0.5f)
        {
            max_speed -= 0.1f;
            if (max_speed < 0) max_speed = 0;
            DebugUIBuilder.instance.AddLabel($"max speed: {max_speed}");
        }
    }
}
