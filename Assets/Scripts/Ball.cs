using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public static int state;  //-3: �X�R�A�̃t�F�[�h�A�E�g, -2: �X�R�A�̕\��(�v���C���[�̓��͑҂�), -1: �X�R�A������������,  0: reset, 1: racket, 2: frontwall, 3: floor
    public static int num_bounce;  // ���Ƀo�E���h�����񐔁B�����n�_�\���̂��ߒǉ��B

    /*----�{�[����ł����Ƃ��̌v�Z�֘A----*/
    public static bool shot_by_player;  // true:player, false:opponent
    int calc_hit;  //�{�[����ł������̋������v�Z�����ǂ����̃t���O
    [SerializeField] GameObject hitMarker;  // �{�[�������P�b�g�ɂ��������ꏊ�̃}�[�J�[
    Vector3 prev_pos;
    Vector3 ball_vel, prev_ball_vel;
    Vector3 racket_normal;
    /*----------------------------------*/
    public static bool is_game;
    static int player_score;
    static int opponent_score;
    [SerializeField] GameObject opponent;
    Collider opponentCollider;
    [SerializeField] GameObject scoreImage;
    [SerializeField] GameObject infoImage;
    [SerializeField] GameObject playerScoreText;
    [SerializeField] GameObject opponentScoreText;
    [SerializeField] GameObject playerPointDescription;
    [SerializeField] GameObject opponentPointDescription;
    [SerializeField] private GameObject leftHand;
    [SerializeField] GameObject racket;
    ChangeAlpha changeScore, changeInfo;  // �X�R�A��\�����邽�߂̃I�u�W�F�N�g

    float time = 0f;  // ��O�����m���邽�߂̎���
    float max_speed = 11f;

    void Start()
    {
        state = -1;
        num_bounce = 0;
        shot_by_player = false;
        calc_hit = 0;
        hitMarker.SetActive(false);
        changeScore = new ChangeAlpha(scoreImage.GetComponent<CanvasGroup>());
        changeInfo = new ChangeAlpha(infoImage.GetComponent<CanvasGroup>());
        opponentCollider = opponent.GetComponent<Collider>();
    }

    
    void OnCollisionEnter(Collision collision)
    {
        
        GameObject other = collision.gameObject;
        Vector3 normal = collision.contacts[0].normal;
        time = 0;
        switch (other.tag)
        {
            
            case "Racket":
                if (state >= 0) state = 1;                
                shot_by_player = true;
                calc_hit = 1;
                ball_vel = prev_ball_vel;
                racket.GetComponent<MeshCollider>().enabled = false;  // 10�t���[���̊Ԃ͘A���Փ˂������
                DebugUIBuilder.instance.AddLabel("hit");
                racket_normal = other.transform.forward;
                hitMarker.transform.parent = other.transform;
                hitMarker.transform.position = transform.position;
                prev_pos = transform.position;
                break;
            
            case "FrontWall":
                if (state == 1)
                {
                    state = 2;
                    if (shot_by_player) gameObject.layer = 6;
                    else gameObject.layer = 0;

                }
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
                num_bounce++;
                if (num_bounce == 2)   // ���łɃ{�[����1�o�E���h������̏ꍇ
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                    SetPointDescription(!shot_by_player, "not up!");
                    Score(true);  // �ł��Ԃ��Ȃ������v���C���[�̎��_
                }
                else if (state == 1)  // �{�[�����O�ǂɓ͂��Ȃ������ꍇ
                {
                    SetPointDescription(shot_by_player, "not up!");
                    Score(false);  // �����ł����v���C���[�̎��_
                }
                if (state == 0) HoldBall();
                else if (state > 0) state = 3;
                break;

            case "Opponent":
                if (state > 0)
                {
                    DebugUIBuilder.instance.AddLabel("hit opponent's body");
                    SetPointDescription(false, "body!");
                    player_score++;
                    state = -1;
                }
                break;
        }
        if (state != 3) num_bounce = 0;
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
                if (!(changeInfo.isFadeout || changeScore.isFadeout))  // �X�R�A�\�����t�F�[�h�A�E�g�����珉����Ԃ�
                {
                    HoldBall();
                    state = 0;
                }
                return;
            }
        }

        if (state > 0)
        {
            time += Time.deltaTime;
            if (time > 4) Out();  // �v���[����4�b�ȏ�{�[���̏Փ˂����o����Ȃ��ꍇ�͏�O�Ɣ���
        }
        else time = 0;
        
        prev_ball_vel = GetComponent<Rigidbody>().velocity;

        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            Toss();
        }


        if (calc_hit != 0)  // �{�[����ł����Ƃ��̌v�Z
        {
            calc_hit++;
            if (calc_hit == 3)  // ���P�b�g�̑��x�����߂邽�߁A�{�[���Ƃ̏Փ˂���1�t���[����Ɍv�Z����
            {
                Vector3 new_vel = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 vel = (hitMarker.transform.position - prev_pos) / Time.deltaTime;  // 1�t���[����̈ʒu���烉�P�b�g�̑��x���v�Z���A�{�[���̃X�P�[���ɍ��킹��
                if ((vel.z > 0) != (racket_normal.z > 0)) racket_normal *= -1;  // �X�C���O�̕����Ƀ��P�b�g�̖@���x�N�g����������
                float vel_abs = (vel - ball_vel).magnitude;
                if (vel_abs > max_speed) vel_abs = max_speed;  // �X�C���O����������ꍇ�͐�������
                new_vel = vel_abs * racket_normal;
                GetComponent<Rigidbody>().velocity = new_vel;
            }
            if (calc_hit >= 10)
            {
                calc_hit = 0;
                racket.GetComponent<MeshCollider>().enabled = true;
            }
        }
    }


    public void StartGame()  // �������[�h�̊J�n(Player����Ă΂��)
    {
        is_game = true;
        player_score = 0;
        opponent_score = 0;
        state = -1;
        playerPointDescription.GetComponent<Text>().text = null;
        opponentPointDescription.GetComponent<Text>().text = null;
    }


    public void EndGame()  // �t���[�v���C���[�h�̊J�n(Player����Ă΂��)
    {
        is_game = false;
        changeInfo.Destroy();
        changeScore.Destroy();
    }


    void SetPointDescription(bool isPlayer, string description)  // �|�C���g�����������R���X�R�A�Ɠ����ɕ\��������
    {

        if (isPlayer)
        {
            playerPointDescription.GetComponent<Text>().text = description;
            opponentPointDescription.GetComponent<Text>().text = null;
        }
        else
        {
            playerPointDescription.GetComponent<Text>().text = null;
            opponentPointDescription.GetComponent<Text>().text = description;
        }
    }


    void Out()  // �{�[�����A�E�g�����Ƃ�
    {
        if (state > 0)
        {
            state = 4;
            SetPointDescription(shot_by_player, "out!");
            Score(false);
        }
    }


    void Score(bool pointByHit)  // �|�C���g���������Ƃ�
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
        state = -1;
    }

    void HoldBall()  // �{�[��������Ɏ���
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.parent = leftHand.transform;
        transform.position = leftHand.transform.position;
    }


    void Toss()  // �T�[�u��łۂ̃g�X
    {
        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = leftHand.transform.position;
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 3.0f, 0.0f);
        state = 0;
    }
}
