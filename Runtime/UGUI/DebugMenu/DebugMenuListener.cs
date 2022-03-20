//--------------------------------------------------------------------s---
// <copyright file="DebugMenuListener.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(DebugMenu))]
    public class DebugMenuListener : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] [HideInInspector] private DebugMenu debugMenu;
#pragma warning restore 0649

#if USING_UNITY_INPUT_SYSTEM
        private UnityEngine.InputSystem.Controls.KeyControl keyboardKey = null;
#endif

        private float fingerHoldTime = 0.0f;
        private float keyHoldTime = 0.0f;

        private void OnValidate()
        {
            this.AssertGetComponent<DebugMenu>(ref this.debugMenu);
        }

        private void Awake()
        {
            this.OnValidate();
        }

        private void Update()
        {
            this.CheckTouch();
            this.CheckKeyboard();
        }

        private void CheckTouch()
        {
            if (UnityEngine.Input.touchCount == this.debugMenu.Settings.FingerDownCount)
            {
                this.fingerHoldTime += Time.unscaledDeltaTime;

                if (this.fingerHoldTime > this.debugMenu.Settings.FingerDownTime)
                {
                    this.fingerHoldTime = 0.0f;
                    this.debugMenu.ShowMenu();
                }
            }
            else
            {
                this.fingerHoldTime = 0.0f;
            }
        }

        private void CheckKeyboard()
        {
#if USING_UNITY_INPUT_SYSTEM

            if (this.keyboardKey == null)
            {
                var keyboard = UnityEngine.InputSystem.Keyboard.current;

                if (keyboard != null)
                {
                    this.keyboardKey = keyboard.FindKeyOnCurrentKeyboardLayout(this.debugMenu.Settings.Key.ToString());
                }
            }

            if (keyboardKey.wasPressedThisFrame)
#else
            if (UnityEngine.Input.GetKey(this.debugMenu.Settings.Key))
#endif
            {
                this.keyHoldTime += Time.unscaledDeltaTime;

                if (this.keyHoldTime > this.debugMenu.Settings.KeyHoldTime)
                {
                    this.keyHoldTime = 0.0f;
                    this.debugMenu.ShowMenu();
                }
            }
            else
            {
                this.keyHoldTime = 0.0f;
            }
        }
    }
}

#endif
