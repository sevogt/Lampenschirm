using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLightTexture : MonoBehaviour
{
    Texture2D generateLightCorrection(int radiusCylinder, int distBeamer)
    {
        Texture2D texture = new Texture2D(2200, 1200, TextureFormat.RGBA32,  false,  false); 
        for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++) // texture1.width
                {
                    texture.SetPixel(x, y, Color.black);
                    texture.SetPixel(x, y, Color.black);
                    texture.SetPixel(x, y, Color.black);
                }
            }

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width/2; x++) // texture1.width
                {
                    float transparency = calcTransparency(x, radiusCylinder, distBeamer);
                    Color color_transparent = new Color(0f, 0f, 0f, transparency);

                    texture.SetPixel(x, y, color_transparent);
                    texture.SetPixel(texture.width-1-x, y, color_transparent);
                }
            }
            return texture;
    }

    float calcTheta(float radius, float distance, float angleAlpha)
    {
      float alpha = angleAlpha * Mathf.PI / 180;
      float x = Mathf.Sin(alpha) * radius;
      float p = Mathf.Cos(alpha) * radius;
      float phi = Mathf.Tan(x/(distance + radius - p));
      alpha = alpha * 180 / Mathf.PI;
      phi = phi * 180 / Mathf.PI;
      float beta = 180 - alpha - phi;
      float theta = 180 - beta;
      return theta;
    }

    float pixelToAngle(float x)
    {
      int max_angle = 120;
      int max_pixel = 2200;

      x = Mathf.Abs(x);
      float ratio = max_angle/max_pixel;
      return x * ratio;
    }

    float calcTransparency(float x, float radiusCylinder, float distBeamer)
    {
      float x_angle = pixelToAngle(x);
      float theta = calcTheta(radiusCylinder, distBeamer, x_angle);
      float bright_perc = Mathf.Cos(theta*Mathf.PI/180);
      float transparency = 255 * (1-bright_perc);
      return transparency;
    }
}
