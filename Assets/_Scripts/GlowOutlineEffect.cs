using UnityEngine;
using UnityEngine.Rendering;

namespace FI
{

    public class GlowOutlineEffect : MonoBehaviour
    {
        #region ENUMS

        private enum Pass
        {
            PrePass,
            BlurredPass,
            FinalPass,
            Result
        }

        [SerializeField]
        private Pass passToShow;

        #endregion

        #region PRIVATE_VARS

        [SerializeField, Range(0, 3)]
        private int iterations;

        [SerializeField, Range(0, 10)]
        private float intensity;

        [SerializeField]
        private Color glowColor;

        [SerializeField]
        private Renderer[] toRender;

        private Material unlitMat;
        private Material gaussianBlurMat;
        private Material finalPassMat;
        private Material glowOutlineMat;

        private Material showPrePassMat;
        private Material showBlurredMat;
        private Material showFinalPassMat;

        private CommandBuffer buffer;

        private int prePassTexID;
        private int blurredTexID;
        private int finalTexID;

        private int tempText1ID;

        #endregion

        #region UNITY_API

        // Use this for initialization
        private void Start()
        {
            InitializeBuffer();
            InitializeMaterials();
            InitializeProperties();
        }

        // --------------------------
        private void Update()
        {
            UpdateBuffer();
            UpdateGlowProperties();
        }

        #endregion

        #region PRIVATE_METHODS

        // -----------------------------------------------------------------------
        private void InitializeBuffer()
        {
            buffer = new CommandBuffer();
            Camera.main.AddCommandBuffer(CameraEvent.BeforeImageEffects, buffer);
        }

        // -------------------------------------------------------------------
        private void InitializeMaterials()
        {
            unlitMat = new Material(Shader.Find("Unlit/Color"));
            gaussianBlurMat = new Material(Shader.Find("FI/GaussianBlur"));
            finalPassMat = new Material(Shader.Find("FI/FinalPass"));
            glowOutlineMat = new Material(Shader.Find("FI/GlowOutline"));

            showPrePassMat = new Material(Shader.Find("FI/ShowPrePass"));
            showBlurredMat = new Material(Shader.Find("FI/ShowBlurred"));
            showFinalPassMat = new Material(Shader.Find("FI/ShowFinalPass"));
        }

        // ----------------------------------------------------
        private void InitializeProperties()
        {
            prePassTexID = Shader.PropertyToID("_PrePassTex");
            blurredTexID = Shader.PropertyToID("_BlurredTex");
            tempText1ID = Shader.PropertyToID("_TempTex1");

            finalTexID = Shader.PropertyToID("_FinalTex");
        }

        // --------------------------------------------------
        private void UpdateGlowProperties()
        {
            Shader.SetGlobalFloat("_Intensity", intensity);
            Shader.SetGlobalColor("_GlowColor", glowColor);
        }

        // --------------------------
        private void UpdateBuffer()
        {
            buffer.Clear();

            UpdatePrePassTexture();
            UpdateBlurredTexture();
            UpdateFinalTexture();
        }

        // ----------------------------------------------------------------------
        private void UpdatePrePassTexture()
        {
            buffer.GetTemporaryRT(prePassTexID, -1, -1,
                0, FilterMode.Bilinear, RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Default, QualitySettings.antiAliasing);

            buffer.SetRenderTarget(prePassTexID);
            buffer.ClearRenderTarget(true, true, Color.clear);

            for (int i = 0; i < toRender.Length; i++)
            {
                if (toRender[i].gameObject.activeSelf)
                    buffer.DrawRenderer(toRender[i], unlitMat);
            }
        }

        // ----------------------------------------------------------------------
        private void UpdateBlurredTexture()
        {
            buffer.GetTemporaryRT(blurredTexID, -2, -2, 0, FilterMode.Bilinear);
            buffer.GetTemporaryRT(tempText1ID, -2, -2, 0, FilterMode.Bilinear);

            buffer.Blit(prePassTexID, blurredTexID, gaussianBlurMat);

            for (int i = 0; i < iterations; i++)
            {
                buffer.Blit(blurredTexID, tempText1ID, gaussianBlurMat, 0);
                buffer.Blit(tempText1ID, blurredTexID, gaussianBlurMat, 1);
            }
        }

        // ----------------------------------------------------------------------
        private void UpdateFinalTexture()
        {
            buffer.GetTemporaryRT(finalTexID, -1, -1, 0, FilterMode.Bilinear);

            buffer.Blit(null, finalTexID, finalPassMat);
        }

        // -------------------------------------------------------------------------
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            switch (passToShow)
            {
                case Pass.PrePass:
                    Graphics.Blit(null, destination, showPrePassMat);
                    break;
                case Pass.BlurredPass:
                    Graphics.Blit(null, destination, showBlurredMat);
                    break;
                case Pass.FinalPass:
                    Graphics.Blit(null, destination, showFinalPassMat);
                    break;
                default:
                    Graphics.Blit(source, destination, glowOutlineMat);
                    break;
            }
        }

        #endregion

    }

}