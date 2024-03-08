
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Texel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PixelCapture : UdonSharpBehaviour
    {
        public RenderTexture pixelSourceTexture;
        public Texture2D pixelBufferTexture;
        public float readFrequency = 0;

        [Header("UdonGraph Defaults")]
        public int defaultPixelIndex = 0;
        public float defaultPixelThreshold = 0.01f;

        [HideInInspector]
        public int pixelIndex;
        [HideInInspector]
        public float pixelThreshold;
        [HideInInspector]
        public float grayscaleResult;
        [HideInInspector]
        public bool testResult;
        [HideInInspector]
        public Color[] pixelData;

        float nextCheckTime = 0;

        void Start()
        {
            pixelData = new Color[pixelBufferTexture.width * pixelBufferTexture.height];
            pixelIndex = defaultPixelIndex;
            pixelThreshold = defaultPixelThreshold;
        }

        private void OnPostRender()
        {
            float time = Time.time;
            if (nextCheckTime > time)
                return;

            nextCheckTime = time + readFrequency;

            int w = pixelSourceTexture.width;
            int h = pixelSourceTexture.height;

            pixelBufferTexture.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            pixelBufferTexture.Apply();

            for (int y = 0; y < h; y++) {
                int b = y * w;
                for (int x = 0; x < w; x++)
                    pixelData[b + x] = pixelBufferTexture.GetPixel(x, y);
            }
        }

        public void _GetGrayscale()
        {
            if (pixelIndex < 0 || pixelIndex >= pixelData.Length)
            {
                grayscaleResult = 0;
                return;
            }

            grayscaleResult = pixelData[pixelIndex].grayscale;
        }

        public void _IsWhite()
        {
            if (pixelIndex < 0 || pixelIndex >= pixelData.Length)
            {
                testResult = false;
                return;
            }

            Color c = pixelData[pixelIndex];
            float g = c.grayscale;

            testResult = g + pixelThreshold >= 1;
        }

        public void _IsBlack()
        {
            if (pixelIndex < 0 || pixelIndex >= pixelData.Length)
            {
                testResult = false;
                return;
            }

            Color c = pixelData[pixelIndex];
            float g = c.grayscale;

            testResult = g - pixelThreshold <= 0;
        }
    }
}
