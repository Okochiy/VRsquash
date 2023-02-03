using UnityEngine;

// CanvasGroup�R���|�[�l���g���A�^�b�`����Ă��Ȃ��ꍇ�A�A�^�b�`
[RequireComponent(typeof(CanvasGroup))]
public class ChangeAlpha
{
    // �t�F�[�h�����鎞�Ԃ�ݒ�
    private float fadeTime = 1f;
    // �o�ߎ��Ԃ��擾
    public bool isFadein, isFadeout;
    public bool loop = false;
    public float endAlpha = 1f;
    CanvasGroup canvasGroup;
    public ChangeAlpha(CanvasGroup _canvasGroup)
    {
        // alpha�l��0(�����j�ɂ���B
        canvasGroup = _canvasGroup;
        canvasGroup.alpha = 0;
    }


    public void UpdateFade()
    {
        // �o�ߎ��Ԃ�fadeTime�Ŋ������l��alpha�ɓ����
        // ��alpha�l��1(�s����)���ő�B
        if (isFadein)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            if (canvasGroup.alpha >= endAlpha)
            {
                isFadein = false;  // �t�F�[�h�C�����I�������isFadein��false�ɕύX
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
                isFadeout = false;  // �t�F�[�h�A�E�g���I�������isFadeout��false�ɕύX
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