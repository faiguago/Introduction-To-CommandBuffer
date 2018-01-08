using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace FI
{

    public class ScreenshotTaker : MonoBehaviour
    {

        #region PRIVATE_VARS

        private bool isTaking;

        private RenderTexture rt;

        [SerializeField]
        private CameraEvent cameraEvent;

        private CommandBuffer commandBuffer;

        [SerializeField, Range(0, 2)]
        private int downFactor;

        #endregion

        #region UNITY_API

        // Use this for initialization
        private void Start()
        {
            CreateCommandBuffer();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
                TakeScreenshot();
        }

        #endregion

        #region PRIVATE_METHODS

        // -----------------------------------------------------------
        private void CreateCommandBuffer()
        {
            commandBuffer = new CommandBuffer();

            rt = new RenderTexture(
                Camera.main.pixelWidth >> downFactor,
                Camera.main.pixelHeight >> downFactor, 0);

            commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive,
                new RenderTargetIdentifier(rt));
        }

        // ------------------------------------
        private void TakeScreenshot()
        {
            StartCoroutine(AddCommandBuffer());
        }

        // -------------------------------------
        private IEnumerator AddCommandBuffer()
        {
            isTaking = true;
            Camera.main.AddCommandBuffer(
                cameraEvent, commandBuffer);

            yield return null;

            SaveScreenshot();
        }

        // -------------------------------------------------------------------
        private void SaveScreenshot()
        {
            Texture2D t = new Texture2D(rt.width, rt.height,
                TextureFormat.RGB24, false);

            Graphics.SetRenderTarget(rt);

            t.ReadPixels(new Rect(0f, 0f, t.width, t.height), 0, 0);
            t.Apply();

            Graphics.SetRenderTarget(null);

            byte[] b = t.EncodeToPNG();
            Destroy(t);

            File.WriteAllBytes(@"D:\" + cameraEvent + ".png", b);

            isTaking = false;
            Camera.main.RemoveCommandBuffer(
                cameraEvent, commandBuffer);
        }

        // Default OnDestroy method
        private void OnDestroy()
        {
            if (rt)
                rt.Release();
        }

        #endregion

    }

}