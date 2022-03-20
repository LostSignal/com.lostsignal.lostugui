﻿//-----------------------------------------------------------------------
// <copyright file="MessageBox.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Text = TMPro.TMP_Text;

    public enum OkResult
    {
        Ok,
    }

    public enum YesNoResult
    {
        Yes,
        No,
    }

    public enum LeftRightResult
    {
        Left,
        Right,
    }

    public class MessageBox : DialogLogic
    {
        #pragma warning disable 0649
        [Header("MessageBox")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button okButton;
        [SerializeField] private Text title;
        [SerializeField] private Text body;
        [SerializeField] private Text leftButtonText;
        [SerializeField] private Text rightButtonText;
        [SerializeField] private Text okButtonText;
        #pragma warning restore 0649

        private LeftRightResult result;

        public static MessageBox Instance
        {
            get => DialogManager.GetDialog<MessageBox>();
        }

        public UnityTask<OkResult> ShowOk(string title, string body)
        {
            return UnityTask<OkResult>.Run(this.ShowOkInternal(title, body));
        }

        public UnityTask<YesNoResult> ShowYesNo(string title, string body)
        {
            return UnityTask<YesNoResult>.Run(this.ShowYesNoInternal(title, body));
        }

        public UnityTask<LeftRightResult> Show(string title, string body, string leftButtonText, string rightButtonText)
        {
            return UnityTask<LeftRightResult>.Run(this.ShowInternal(title, body, leftButtonText, rightButtonText));
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return;
            }
#endif

            Debug.Assert(this.leftButton != null, "MessageBox didn't define left button", this);
            Debug.Assert(this.rightButton != null, "MessageBox didn't define right button", this);
            Debug.Assert(this.okButton != null, "MessageBox didn't define an OK button", this);
            Debug.Assert(this.title != null, "MessageBox didn't define title", this);
            Debug.Assert(this.body != null, "MessageBox didn't define body", this);

            this.leftButton.onClick.AddListener(this.LeftButtonClicked);
            this.rightButton.onClick.AddListener(this.RightButtonClicked);
            this.okButton.onClick.AddListener(this.OkButtonClicked);
        }

        protected override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.LeftButtonClicked();
        }

        private IEnumerator<OkResult> ShowOkInternal(string title, string body)
        {
            //// TODO [bgish]: If "in use", then wait till it becomes available

            this.SetOkButtonText(title, body);
            this.Dialog.Show();

            // Waiting for it to start showing
            while (this.Dialog.IsShowing == false)
            {
                yield return default;
            }

            // Waiting for it to return to the hidden state
            while (this.Dialog.IsHidden == false)
            {
                yield return default;
            }
        }

        private IEnumerator<YesNoResult> ShowYesNoInternal(string title, string body)
        {
            // TODO [bgish]: If "in use", then wait till it becomes available
            var language = Lost.Localization.Localization.CurrentLanguage;

            this.SetLeftRightText(title, body, language.No, language.Yes);
            this.Dialog.Show();

            // Waiting for it to start showing
            while (this.Dialog.IsShowing == false)
            {
                yield return default;
            }

            // Waiting for it to return to the hidden state
            while (this.Dialog.IsHidden == false)
            {
                yield return default;
            }

            if (this.result == LeftRightResult.Left)
            {
                yield return YesNoResult.No;
            }
            else if (this.result == LeftRightResult.Right)
            {
                yield return YesNoResult.Yes;
            }
            else
            {
                throw new NotImplementedException("MessageBox.ShowYesNo does not handle result " + this.result.ToString());
            }
        }

        private IEnumerator<LeftRightResult> ShowInternal(string title, string body, string leftButtonText, string rightButtonText)
        {
            //// TODO [bgish]: If "in use", then wait till it becomes available

            if (string.IsNullOrEmpty(leftButtonText) == false && this.leftButtonText == null)
            {
                Debug.LogErrorFormat(this, "Unable to set MessageBox left button to {0} besause LeftButtonText object is null.", leftButtonText);
            }

            if (string.IsNullOrEmpty(rightButtonText) == false && this.rightButtonText == null)
            {
                Debug.LogErrorFormat(this, "Unable to set MessageBox right button to {0} besause RightButtonText object is null.", rightButtonText);
            }

            this.SetLeftRightText(title, body, leftButtonText, rightButtonText);
            this.Dialog.Show();

            // waiting for it to start showing
            while (this.Dialog.IsShowing == false)
            {
                yield return default;
            }

            // waiting for it to return to the hidden state
            while (this.Dialog.IsHidden == false)
            {
                yield return default;
            }

            yield return this.result;
        }

        private void LeftButtonClicked()
        {
            this.result = LeftRightResult.Left;
            this.Dialog.Hide();
        }

        private void RightButtonClicked()
        {
            this.result = LeftRightResult.Right;
            this.Dialog.Hide();
        }

        private void OkButtonClicked()
        {
            this.Dialog.Hide();
        }

        private void SetLeftRightText(string title, string body, string leftButton, string rightButton)
        {
            this.leftButton.gameObject.SetActive(true);
            this.rightButton.gameObject.SetActive(true);
            this.okButton.gameObject.SetActive(false);

            this.title.text = title;
            this.body.text = body;

            if (this.leftButtonText != null)
            {
                this.leftButtonText.text = leftButton;
            }

            if (this.rightButtonText != null)
            {
                this.rightButtonText.text = rightButton;
            }
        }

        private void SetOkButtonText(string title, string body)
        {
            this.leftButton.gameObject.SetActive(false);
            this.rightButton.gameObject.SetActive(false);
            this.okButton.gameObject.SetActive(true);

            this.title.text = title;
            this.body.text = body;

            if (this.okButtonText != null)
            {
                this.okButtonText.text = "OK";
            }
        }
    }
}

#endif
