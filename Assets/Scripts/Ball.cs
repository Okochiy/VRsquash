using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public static int state;  //-3: スコアのフェードアウト, -2: スコアの表示(プレイヤーの入力待ち), -1: スコアが動いた直後,  0: reset, 1: racket, 2: frontwall, 3: floor
    public static int num_bounce;  // 床にバウンドした回数。落下地点予測のため追加。

    /*----ボールを打ったときの計算関連----*/
    public static bool shot_by_player;  // true:player, false:opponent
    int calc_hit;  //ボールを打った時の挙動を計算中かどうかのフラグ
    [SerializeField] GameObject hitMarker;  // ボールがラケットにあたった場所のマーカー
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
    ChangeAlpha changeScore, changeInfo;  // スコアを表示するためのオブジェクト

    float time = 0f;  // 場外を検知するための時間
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
                racket.GetComponent<MeshCollider>().enabled = false;  // 10フレームの間は連続衝突を避ける
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
                if (num_bounce == 2)   // すでにボールが1バウンドした後の場合
                {
                    DebugUIBuilder.instance.AddLabel("not up!");
                    SetPointDescription(!shot_by_player, "not up!");
                    Score(true);  // 打ち返せなかったプレイヤーの失点
                }
                else if (state == 1)  // ボールが前壁に届かなかった場合
                {
                    SetPointDescription(shot_by_player, "not up!");
                    Score(false);  // それを打ったプレイヤーの失点
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
            if (state == -1)  // スコアの表示をスタート
            {
                playerScoreText.GetComponent<Text>().text = player_score.ToString();
                opponentScoreText.GetComponent<Text>().text = opponent_score.ToString();
                changeScore.StartFadein(0.7f);
                changeInfo.StartLoop(1f);
                state = -2;
                return;
            }
            if (state == -2)  // スコアを表示しながらプレイヤーの入力待ち
            {
                changeInfo.UpdateFade();
                changeScore.UpdateFade();
                if (OVRInput.GetDown(OVRInput.Button.Three))  // 入力があれば-3へ遷移
                {
                    changeScore.StartFadeout();
                    changeInfo.StartFadeout();
                    state = -3;
                }
                return;
            }
            if (state == -3)  // スコアのフェードアウト
            {
                changeInfo.UpdateFade();
                changeScore.UpdateFade();
                if (!(changeInfo.isFadeout || changeScore.isFadeout))  // スコア表示がフェードアウトしたら初期状態へ
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
            if (time > 4) Out();  // プレー中に4秒以上ボールの衝突が検出されない場合は場外と判定
        }
        else time = 0;
        
        prev_ball_vel = GetComponent<Rigidbody>().velocity;

        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            Toss();
        }


        if (calc_hit != 0)  // ボールを打ったときの計算
        {
            calc_hit++;
            if (calc_hit == 3)  // ラケットの速度を求めるため、ボールとの衝突から1フレーム後に計算する
            {
                Vector3 new_vel = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 vel = (hitMarker.transform.position - prev_pos) / Time.deltaTime;  // 1フレーム後の位置からラケットの速度を計算し、ボールのスケールに合わせる
                if ((vel.z > 0) != (racket_normal.z > 0)) racket_normal *= -1;  // スイングの方向にラケットの法線ベクトルを向ける
                float vel_abs = (vel - ball_vel).magnitude;
                if (vel_abs > max_speed) vel_abs = max_speed;  // スイングが速すぎる場合は制限する
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


    public void StartGame()  // 試合モードの開始(Playerから呼ばれる)
    {
        is_game = true;
        player_score = 0;
        opponent_score = 0;
        state = -1;
        playerPointDescription.GetComponent<Text>().text = null;
        opponentPointDescription.GetComponent<Text>().text = null;
    }


    public void EndGame()  // フリープレイモードの開始(Playerから呼ばれる)
    {
        is_game = false;
        changeInfo.Destroy();
        changeScore.Destroy();
    }


    void SetPointDescription(bool isPlayer, string description)  // ポイントが入った理由をスコアと同時に表示させる
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


    void Out()  // ボールがアウトしたとき
    {
        if (state > 0)
        {
            state = 4;
            SetPointDescription(shot_by_player, "out!");
            Score(false);
        }
    }


    void Score(bool pointByHit)  // ポイントが動いたとき
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

    void HoldBall()  // ボールを左手に持つ
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.parent = leftHand.transform;
        transform.position = leftHand.transform.position;
    }


    void Toss()  // サーブを打つ際のトス
    {
        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = leftHand.transform.position;
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 3.0f, 0.0f);
        state = 0;
    }
}
