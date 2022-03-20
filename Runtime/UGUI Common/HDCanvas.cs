//-----------------------------------------------------------------------
// <copyright file="HDCanvas.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class HDCanvas : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] [HideInInspector] private CanvasScaler canvasScaler;
        [SerializeField] [HideInInspector] private Canvas canvas;
#pragma warning restore 0649

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "We're overriding Unity functionality and this is Unity's naming convention.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Style",
            "IDE1006:Naming Styles",
            Justification = "We're overriding Unity functionality and this is Unity's naming convention.")]
        public new bool enabled
        {
            get
            {
                return base.enabled;
            }

            set
            {
                this.CacheComponents();
                base.enabled = value;
                this.canvas.enabled = value;
                this.canvasScaler.enabled = value;
            }
        }

        private void Awake()
        {
            this.Setup();
        }

        private void OnEnable()
        {
            this.Setup();
        }

        private void Reset()
        {
            this.Setup();
        }

        private void OnValidate()
        {
            this.CacheComponents();
            this.SetupCanvasScaler();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            this.SetupCanvasScaler();
        }

        #endif

        private void CacheComponents()
        {
            this.AssertGetComponent<Canvas>(ref this.canvas);
            this.AssertGetComponent<CanvasScaler>(ref this.canvasScaler);
        }

        private void Setup()
        {
            this.CacheComponents();
            this.SetupCanvasScaler();

            // Finding a valid camera if we don't have one
            if (!this.canvas.worldCamera)
            {
                this.canvas.worldCamera = Camera.main;
            }
        }

        private void SetupCanvasScaler()
        {
            if (this.canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                this.canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            }

            bool isPortrait = Screen.height > Screen.width;

            if (isPortrait)
            {
                if (this.canvasScaler.referenceResolution != new Vector2(1080, 1920))
                {
                    this.canvasScaler.referenceResolution = new Vector2(1080, 1920);
                }
            }
            else
            {
                if (this.canvasScaler.referenceResolution != new Vector2(1920, 1080))
                {
                    this.canvasScaler.referenceResolution = new Vector2(1920, 1080);
                }
            }

            if (this.canvasScaler.screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                this.canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            }

            if (this.canvasScaler.matchWidthOrHeight != 1)
            {
                this.canvasScaler.matchWidthOrHeight = 1;
            }
        }
    }
}

#endif
