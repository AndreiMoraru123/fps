
/*
 * Canvas controller goal is decouple UI from pipeline (PredefinedBase) and unity device (OAKDevice)
 * In order to give some flexibility we're using Material as basic object with Texture2D
 * In that way we can use Image in Canvas UI or just Material in objects
 */

using UnityEngine;
using UnityEngine.UI;

namespace OAKForUnity
{
    public class HandTrackingCanvasController : MonoBehaviour
    {
        // public attributes

        // Pipeline for texture binding
        public HandTrackingPipeline pipeline;

        [Header("UI Binding")]
        public Image colorCameraImage;
        private bool _init = false;

        // Binding Textures. Wait to have pipeline running.
        private void Init()
        {
            // Texture2D binding
            colorCameraImage.material.mainTexture = pipeline.colorTexture;
            _init = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (pipeline.deviceRunning && !_init) Init();
        }
    }
}