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
    [SerializeField] float racket_coef = 0.8f;
    Vector3 prev_pos;
    Vector3 ball_vel;
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

    /*
    void calcSpin(Vector3 normal)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        spin = Vector3.Cross(normal, vel);
        // DebugUIBuilder.instance.AddLabel($"normal = {normal}, vel = {vel}, spin = {spin}");
    }

    void bounce(Vector3 normal, float coef, float friction)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        float h = Vector3.Dot(vel, normal);
        if (h > 0) return;
        Vector3 new_vel = friction * vel + (coef + friction) * (-h) * normal;
        this.GetComponent<Rigidbody>().velocity = new_vel;
        // DebugUIBuilder.instance.AddLabel($"vel = {vel}, new_vel = {new_vel}");
    }
    */

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
            /*
            case "Racket":
                if (state == 1)
                {
                    DebugUIBuilder.instance.AddLabel("shot before bouncing on front wall!");
                }
                state = 1;
                if (shot_by_player)
                {
                    DebugUIBuilder.instance.AddLabel("shot twice!");
                }
                shot_by_player = true;
                break;
            */
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


    void OnTriggerEnter(Collider collider)
    {
        GameObject other = collider.gameObject;
        Vector3 normal = new Vector3(0.0f, 0.0f, 0.0f);
        if (other.tag == "Racket")
        {
            if (hitting_flag) return;
            hitting_flag = true;
            calc_hit = 1;
            racket_normal = other.transform.forward;
            hitMarker.transform.parent = other.transform;
            hitMarker.transform.position = transform.position;
            hitMarker.SetActive(true);
            prev_pos = hitMarker.transform.position;
            if (state == 1)
            {
                DebugUIBuilder.instance.AddLabel("shot before bouncing on front wall!");
            }
            if (state > 0)
            {
                state = 1;
                if (shot_by_player)
                {
                    DebugUIBuilder.instance.AddLabel("shot twice!");
                    opponent_score++;
                    state = -1;
                }
                shot_by_player = true;
            }
        }
        /*
        switch (other.tag)
        {
            
            case "Racket":
                if (state == 1)
                {
                    DebugUIBuilder.instance.AddLabel("shot before bouncing on front wall!");
                }
                state = 1;
                if (shot_by_player)
                {
                    DebugUIBuilder.instance.AddLabel("shot twice!");
                }
                shot_by_player = true;
                break;
            
            case "FrontWall":
                normal = new Vector3(0.0f, 0.0f, -1.0f);
                bounce(normal, coef_wall, friction_wall);
                if (state != 0) state = 2;
                break;

            case "FrontOut":
                normal = new Vector3(0.0f, 0.0f, -1.0f);
                bounce(normal, coef_wall, friction_wall);
                Out();
                break;

            case "LeftWall":
                normal = new Vector3(1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                break;

            case "LeftOut":
                normal = new Vector3(1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                Out();
                break;

            case "RightWall":
                normal = new Vector3(-1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                break;

            case "RightOut":
                normal = new Vector3(-1.0f, 0.0f, 0.0f);
                bounce(normal, coef_wall, friction_wall);
                Out();
                break;

            case "BackWall":
                normal = new Vector3(0.0f, 0.0f, 1.0f);
                bounce(normal, coef_wall, friction_wall);
                break;

            case "Floor":
                normal = new Vector3(0.0f, 1.0f, 0.0f);
                bounce(normal, coef_floor, friction_floor);
                DebugUIBuilder.instance.AddLabel("bounce");
                if (state == 3)
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                    state = 0;
                }
                if (state != 0) state = 3;
                break;
        }
        
        DebugUIBuilder.instance.AddLabel($"normal:{normal}");
        calcSpin(normal);
        */
    }

    void Update()
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
            if (calc_hit >= 15)
            {
                calc_hit = 0;
                Vector3 new_vel = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 vel = (hitMarker.transform.position - prev_pos) * 8;  // 1�t���[����̈ʒu���烉�P�b�g�̑��x���v�Z���A�{�[���̃X�P�[���ɍ��킹��
                if ((vel.z > 0) != (racket_normal.z > 0)) racket_normal *= -1;  // �X�C���O�̕����Ƀ��P�b�g�̖@���x�N�g����������
                Vector3 ball_vel = GetComponent<Rigidbody>().velocity;
                float vel_abs = (vel - ball_vel).magnitude;
                new_vel = vel_abs * racket_normal;
                /*
                new_vel.z = (vel.z - ball_vel.z) * racket_coef;  // �O�����̑��x�����͔��˂̍l����
                new_vel.x = racket_normal.x * new_vel.z / racket_normal.z;  // �������̓��P�b�g�̌����Ɉˑ�(�ʂ̌����ɔ�Ԃ悤�ɂ���)
                new_vel.y = racket_normal.y * new_vel.z / racket_normal.z;  // �㉺��������{�̓��P�b�g�̌����Ɉˑ�
                */
                GetComponent<Rigidbody>().velocity = new_vel;
                /*if (OVRInput.Get(OVRInput.Button.Three))
                {
                    //DebugUIBuilder.instance.AddLabel($"swing_vel: {vel}\nracket_normal: {racket_normal}");
                }*/
            }
        }


        if (!shot_by_player && state == 1) gameObject.layer = 6;
        else gameObject.layer = 0;

        tmp++;
        if (tmp % 20 == 0) DebugUIBuilder.instance.AddLabel($"{state}");
    }

    public static void StartGame()
    {
        is_game = true;
        player_score = 0;
        opponent_score = 0;
        state = -1;
    }

    public static void EndGame()
    {
        is_game = false;
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

}
