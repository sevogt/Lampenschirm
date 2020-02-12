using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLightTexture : MonoBehaviour
{
    public static Texture2D generateLightCorrection(float radiusCylinder, float distBeamer)
    {
        Texture2D texture = new Texture2D(2200, 1200, TextureFormat.RGBA32,  false,  false); 
        
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width/2; x++) // texture1.width
                {
                    float transparency = calcTransparency((float)x, radiusCylinder, distBeamer);
                    Color color_transparent = new Color(0f, 0f, 0f, checkThreshold(transparency));
                    

                    texture.SetPixel(x, y, color_transparent);
                    texture.SetPixel(texture.width-1-x, y, color_transparent);
                    
                }
            }
            
            texture.Apply();
            return texture;
    }

    static float calcTheta(float radius, float distance, float angleAlpha)
    {
      float alpha = angleAlpha * Mathf.PI / 180f;
      float x = Mathf.Sin(alpha) * radius;
      float p = Mathf.Cos(alpha) * radius;
      float phi = Mathf.Tan(x/(distance + radius - p));
      alpha = alpha * 180f / Mathf.PI;
      phi = phi * 180f / Mathf.PI;
      float beta = 180f - alpha - phi;
      float theta = 180f - beta;
      return theta;
    }

    static float pixelToAngle(float x)
    {
      float max_angle = 120f;
      float max_pixel = 2200f;

      x = Mathf.Abs(x);
      float ratio = max_angle/max_pixel;
      return x * ratio;
    }

    static float calcTransparency(float x, float radiusCylinder, float distBeamer)
    {
      float x_angle = pixelToAngle(x);
      float theta = calcTheta(radiusCylinder, distBeamer, x_angle);
      float bright_perc = Mathf.Cos(theta*Mathf.PI/180f);
      float transparency = (1-bright_perc);
      return transparency;
    }

    static float checkThreshold(float t)
    {
      if(t > 0.4f)
        return 0.4f;
      else
        return t;
    }
}
