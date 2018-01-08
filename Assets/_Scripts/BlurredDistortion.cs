using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FI
{

    public class BlurredDistortion : MonoBehaviour
    {
        [SerializeField, Range(0, 3)]
        private int iterations;

        private CommandBuffer buffer;

        private Material gaussianBlurMaterial;

        private int temporaryTex;
        private int blurredGrabTex;

        // -------------------------------------
        private void Start()
        {
            CreateBuffer();
            InitializePropertiesAndMaterials();
        }

        // ------------------
        private void Update()
        {
            UpdateBuffer();
        }

        // ----------------------------------------------------------
        private void InitializePropertiesAndMaterials()
        {
            temporaryTex = Shader.PropertyToID("_TempGrabTex");
            blurredGrabTex = Shader.PropertyToID("_BlurredGrabTex");

            gaussianBlurMaterial =
                new Material(Shader.Find("FI/GaussianBlur"));
        }

        // ---------------------------------------------------------------
        private void CreateBuffer()
        {
            buffer = new CommandBuffer();
            buffer.name = "BlurredGrabTexture";
            Camera.main.AddCommandBuffer(CameraEvent.AfterSkybox, buffer);
        }

        // --------------------------------------------------------------------------
        private void UpdateBuffer()
        {
            buffer.Clear();

            buffer.GetTemporaryRT(blurredGrabTex,
                  -2, -2, 0, FilterMode.Bilinear);
            buffer.GetTemporaryRT(temporaryTex,
                -2, -2, 0, FilterMode.Bilinear);

            buffer.Blit(BuiltinRenderTextureType.CurrentActive, blurredGrabTex);

            for (int i = 0; i < iterations; i++)
            {
                buffer.Blit(blurredGrabTex, temporaryTex, gaussianBlurMaterial, 0);
                buffer.Blit(temporaryTex, blurredGrabTex, gaussianBlurMaterial, 1);
            }
        }
    }

}