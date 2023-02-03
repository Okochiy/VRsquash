using UnityEngine;

// CanvasGroupコンポーネントがアタッチされていない場合、アタッチ
[RequireComponent(typeof(CanvasGroup))]
public class ChangeAlpha
{
    // フェードさせる時間を設定
    private float fadeTime = 1f;
    // 経過時間を取得
    public bool isFadein, isFadeout;
    public bool loop = false;
    public float endAlpha = 1f;
    CanvasGroup canvasGroup;
    public ChangeAlpha(CanvasGroup _canvasGroup)
    {
        // alpha値を0(透明）にする。
        canvasGroup = _canvasGroup;
        canvasGroup.alpha = 0;
    }


    public void UpdateFade()
    {
        // 経過時間をfadeTimeで割った値をalphaに入れる
        // ※alpha値は1(不透明)が最大。
        if (isFadein)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            if (canvasGroup.alpha >= endAlpha)
            {
                isFadein = false;  // フェードインが終了するとisFadeinをfalseに変更
                if (loop)
                {
                    isFadeout = true;
                }
            }
        }
        else if (isFadeout)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            if (canvasGroup.alpha <= 0)
            {
                isFadeout = false;  // フェードアウトが終了するとisFadeoutをfalseに変更
                if (loop)
                {
                    isFadein = true;
                }
            }
        }
    }

    public void StartFadein(float endalpha)
    {
        endAlpha = endalpha;
        isFadein = true;
        isFadeout = false;
        this.loop = false;
    }

    public void StartFadeout()
    {
        isFadeout = true;
        isFadein = false;
        this.loop = false;
    }

    public void StartLoop(float endalpha)
    {
        StartFadein(endalpha);
        this.loop = true;
    }

    public void Destroy()
    {
        canvasGroup.alpha = 0;
    }
}