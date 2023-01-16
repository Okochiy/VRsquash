using UnityEngine;

// CanvasGroup�R���|�[�l���g���A�^�b�`����Ă��Ȃ��ꍇ�A�A�^�b�`
[RequireComponent(typeof(CanvasGroup))]
public class ChangeAlpha
{
    // �t�F�[�h�����鎞�Ԃ�ݒ�
    private float fadeTime = 1f;
    // �o�ߎ��Ԃ��擾
    private bool isFadein, isFadeout;
    private bool loop = false;
    CanvasGroup canvasGroup;
    public ChangeAlpha(CanvasGroup _canvasGroup)
    {
        // alpha�l��0(�����j�ɂ���B
        canvasGroup = _canvasGroup;
        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    public void UpdateFade()
    {
        // �o�ߎ��Ԃ�fadeTime�Ŋ������l��alpha�ɓ����
        // ��alpha�l��1(�s����)���ő�B
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