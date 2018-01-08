using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FI
{
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class OnRenderImageTest : MonoBehaviour
    {

        private Material mat;
        
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!mat)
            {
                mat = new Material(Shader.Find("FI/GaussianBlur"));
                mat.hideFlags = HideFlags.HideAndDontSave;
            }
            
            RenderTexture temp1 = RenderTexture.GetTemporary(
                source.width >> 1, source.height >> 1);
            Graphics.Blit(source, temp1, mat);

            for (int i = 1; i < 5; i++)
            {
                RenderTexture temp2 = RenderTexture.GetTemporary(
                    source.width >> 1, source.height >> 1);
                Graphics.Blit(temp1, temp2, mat, 0);
                Graphics.Blit(temp2, temp1, mat, 1);
                RenderTexture.ReleaseTemporary(temp2);
            }

            Graphics.Blit(temp1, destination);
            RenderTexture.ReleaseTemporary(temp1);
        }
    }

}