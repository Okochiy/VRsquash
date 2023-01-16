using UnityEngine;

// CanvasGroupコンポーネントがアタッチされていない場合、アタッチ
[RequireComponent(typeof(CanvasGroup))]
public class ChangeAlpha
{
    // フェードさせる時間を設定
    private float fadeTime = 1f;
    // 経過時間を取得
    private bool isFadein, isFadeout;
    private bool loop = false;
    CanvasGroup canvasGroup;
    public ChangeAlpha(CanvasGroup _canvasGroup)
    {
        // alpha値を0(透明）にする。
        canvasGroup = _canvasGroup;
        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    public void UpdateFade()
    {
        // 経過時間をfadeTimeで割った値をalphaに入れる
        // ※alpha値は1(不透明)が最大。
        if (isFadein)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            if (canvasGroup.alpha >= 1)
            {
                isFadein = false;
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
                isFadeout = false;
                if (loop)
                {
                    isFadein = true;
                }
            }
        }
    }

    public void StartFadein()
    {
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

    public void StartLoop()
    {
        StartFadein();
        this.loop = true;
    }
}