using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffect
{
    private MeshRenderer screen;  // 視覚効果を出すために目の前に固定するもの。quadを想定
    float fadeSpeed = 0.02f;
    float alpha;
    bool isFadeOut = false;
    bool isFadeIn = false;
    Texture2D drawTexture;
    Color[] buffer;
    Material material;

    public VisualEffect(MeshRenderer target)
    {
        screen = target;
        material = target.sharedMaterial;
        screen.enabled = false;
    }

    public void startFadeOut(Texture2D image, float speed)
    {
        alpha = 0.0f;
        Color[] pixels = image.GetPixels();
        buffer = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            buffer[i] = pixels[i];
            buffer[i].a = alpha;
        }
        drawTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
        drawTexture.SetPixels(buffer);
        drawTexture.Apply();
        material.SetTexture("_MainTex", drawTexture);
        screen.enabled = true;
        fadeSpeed = speed;
        isFadeOut = true;
        isFadeIn = false;
    }

    public void startFadeIn(float speed)
    {
        if (!isFadeOut)
        {
            DebugUIBuilder.instance.AddLabel("startFadeIn can be called after startFadeOut called");
        }
        fadeSpeed = speed;
        isFadeIn = true;
        isFadeOut = false;
    }

    public bool updateFade()
    {
        if (isFadeOut)
        {
            if (alpha >= 1.0f) return true;
            alpha += fadeSpeed;
            SetAlpha();
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
                return true;
            }
        }
        else if (isFadeIn)
        {
            if (alpha <= 0.0f) return true;
            alpha -= fadeSpeed;
            SetAlpha();
            if (alpha < 0.0f)
            {
                alpha = 0.0f;
                screen.enabled = false;
                return true;
            }
        }
        return false;
    }

    void SetAlpha()
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i].a = alpha;
        }
        drawTexture.SetPixels(buffer);
        drawTexture.Apply();
        material.SetTexture("_MainTex", drawTexture);
    }

}
