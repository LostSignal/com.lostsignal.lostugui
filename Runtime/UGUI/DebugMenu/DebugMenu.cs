//--------------------------------------------------------------------s---
// <copyright file="DebugMenu.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    public enum Corner
    {
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight,
    }

    public class DebugMenu : DialogLogic
    {
        private readonly int fpsUpdateTicks = 10;

#pragma warning disable 0649
        [Header("Menu Items")]
        [SerializeField] private GameObject debugMenu;
        [SerializeField] private DebugMenuItem debugMenuItemPrefab;
        [SerializeField] private RectTransform debugMenuItemsHolder;

        [Header("Text Overlay")]
        [SerializeField] private TMP_Text upperLeftText;
        [SerializeField] private TMP_Text upperRightText;
        [SerializeField] private TMP_Text lowerLeftText;
        [SerializeField] private TMP_Text lowerRightText;
#pragma warning restore 0649

        // Fps related variables
        private int fpsCurrentTickCount = 0;
        private float fpsDeltaTime = 0.0f;
        private Corner fpsCorner = Corner.UpperLeft;
        private bool showFps = false;

        private Camera cameraCache = null;

        public DebugMenuSettings Settings { get; private set; } = new DebugMenuSettings();

        public void ShowMenu()
        {
            this.debugMenu.SetActive(true);
        }

        public void HideMenu()
        {
            this.debugMenu.SetActive(false);
        }

        public void AddItem(string name, UnityAction customAction)
        {
            var newItem = GameObject.Instantiate(this.debugMenuItemPrefab);
            newItem.transform.SetParent(this.debugMenuItemsHolder);
            newItem.transform.Reset();
            newItem.Initialize(name, customAction, this.HideMenu);
        }

        public void RemoveItem(string name)
        {
            foreach (var menuItem in this.debugMenuItemsHolder.GetComponentsInChildren<DebugMenuItem>())
            {
                if (menuItem.Name == name)
                {
                    GameObject.Destroy(menuItem.gameObject);
                }
            }
        }

        public void ShowFPS()
        {
            this.showFps = true;
        }

        public void HideFPS()
        {
            this.showFps = false;
            this.SetText(this.fpsCorner, string.Empty);
        }

        public void ToggleFPS()
        {
            if (this.showFps)
            {
                this.HideFPS();
            }
            else
            {
                this.ShowFPS();
            }
        }

        public void SetFpsCornerAndColor(Corner fpsCorner, Color fpsColor)
        {
            this.fpsCorner = fpsCorner;
            this.SetCornerColor(fpsCorner, fpsColor);
        }

        public void SetText(Corner corner, string text)
        {
            switch (corner)
            {
                case Corner.UpperLeft:
                    this.upperLeftText.text = text;
                    break;

                case Corner.UpperRight:
                    this.upperRightText.text = text;
                    break;

                case Corner.LowerLeft:
                    this.lowerLeftText.text = text;
                    break;

                case Corner.LowerRight:
                    this.lowerRightText.text = text;
                    break;

                default:
                    Debug.LogErrorFormat(this, "TextOverlay found unknown Corner type '{0}'", corner.ToString());
                    break;
            }
        }

        public void SetText(Corner corner, string text, Color color)
        {
            this.SetText(corner, text);
            this.SetCornerColor(corner, color);
        }

        public void SetCornerColor(Corner corner, Color color)
        {
            switch (corner)
            {
                case Corner.UpperLeft:
                    this.upperLeftText.color = color;
                    break;

                case Corner.UpperRight:
                    this.upperRightText.color = color;
                    break;

                case Corner.LowerLeft:
                    this.lowerLeftText.color = color;
                    break;

                case Corner.LowerRight:
                    this.lowerRightText.color = color;
                    break;

                default:
                    Debug.LogErrorFormat(this, "TextOverlay found unknown Corner type '{0}'", corner.ToString());
                    break;
            }
        }

        public void SetSettings(DebugMenuSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            this.Settings = settings;
        }

        public void SetCamera(Camera camera)
        {
            this.cameraCache = camera;
            this.Dialog.SetCamera(camera);
        }

        protected override void Awake()
        {
            base.Awake();
            this.HideMenu();
        }

        protected override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.HideMenu();
        }

        private void Update()
        {
            if (!this.cameraCache)
            {
                if (this.Dialog.Canvas.worldCamera)
                {
                    this.cameraCache = this.Dialog.Canvas.worldCamera;
                }
                else
                {
                    this.cameraCache = Camera.main;
                    this.Dialog.Canvas.worldCamera = this.cameraCache;
                }
            }

            if (this.showFps == false)
            {
                return;
            }

            this.fpsDeltaTime += Time.deltaTime;

            this.fpsCurrentTickCount++;

            if (this.fpsCurrentTickCount == this.fpsUpdateTicks)
            {
                float msec = (this.fpsDeltaTime / (float)this.fpsUpdateTicks) * 1000.0f;
                float fps = 1.0f / (this.fpsDeltaTime / (float)this.fpsUpdateTicks);

                // building the new string
                BetterStringBuilder.New()
                    .Append((int)msec)
                    .Append(".")
                    .Append(this.GetDecimalPoint(msec))
                    .Append(" ms (")
                    .Append((int)fps)
                    .Append(".")
                    .Append(this.GetDecimalPoint(fps))
                    .Append(" fps)")
                    .Set(this.GetCornerText(this.fpsCorner));

                this.fpsCurrentTickCount = 0;
                this.fpsDeltaTime = 0.0f;
            }
        }

        private TMP_Text GetCornerText(Corner corner)
        {
            return corner switch
            {
                Corner.LowerLeft => this.lowerLeftText,
                Corner.LowerRight => this.lowerRightText,
                Corner.UpperLeft => this.upperLeftText,
                Corner.UpperRight => this.upperRightText,
                _ => null,
            };
        }

        private int GetDecimalPoint(float num)
        {
            return (int)((num - (int)num) * 10.0f);
        }

        [System.Serializable]
        public class DebugMenuSettings
        {
#pragma warning disable 0649
            [Header("Keyboard")]
            [SerializeField] private KeyCode key = KeyCode.F2;
            [SerializeField] private float keyHoldTime = 0.01f;

            [Header("Touch")]
            [SerializeField] private int fingerDownCount = 3;
            [SerializeField] private float fingerDownTime = 2.0f;
#pragma warning restore 0649

            public KeyCode Key => this.key;

            public float KeyHoldTime => this.keyHoldTime;

            public int FingerDownCount => this.fingerDownCount;

            public float FingerDownTime => this.fingerDownTime;
        }
    }
}

#endif
