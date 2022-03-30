//-----------------------------------------------------------------------
// <copyright file="XRKeyboard.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_LOST_UGUI

namespace Lost.XR
{
    using UnityEngine;

    public class XRKeyboard : DialogLogic
    {
#pragma warning disable 0649
        [SerializeField] private XRKeyboardData keyboardData;
        [SerializeField] private XRKeyboardKey keyPrefab;

        [Header("Layouts")]
        [SerializeField] private Transform lowerCaseLayout;
        [SerializeField] private Transform upperCaseLayout;
        [SerializeField] private Transform numbersLayout;
        [SerializeField] private Transform symbolsLayout;

        [Header("Rows")]
        [SerializeField] private Transform[] lowerCaseRows;
        [SerializeField] private Transform[] upperCaseRows;
        [SerializeField] private Transform[] numbersRows;
        [SerializeField] private Transform[] symbolsRows;
        [SerializeField] private Transform[] keypadRows;
#pragma warning restore 0649

        private System.Action<char, string> keyPressed;
        private State state;
        private bool isUpperCase;

        public event System.Action<char, string> KeyPressed
        {
            add => this.keyPressed += value;
            remove => this.keyPressed -= value;
        }

        private enum State
        {
            Letters,
            Numbers,
            Symbols,
        }

        public XRKeyboardData.Keyboard CurrentKeyboard
        {
            get => this.keyboardData.CurrentKeyboard;
        }

        public void ToggleUppercase()
        {
            this.isUpperCase = !this.isUpperCase;
            this.UpdateKeyboardVisuals();
        }

        public void SetLetters()
        {
            this.state = State.Letters;
            this.UpdateKeyboardVisuals();
        }

        public void SetNumbers()
        {
            this.state = State.Numbers;
            this.isUpperCase = false;
            this.UpdateKeyboardVisuals();
        }

        public void SetSymbols()
        {
            this.state = State.Symbols;
            this.isUpperCase = false;
            this.UpdateKeyboardVisuals();
        }

        protected override void Awake()
        {
            base.Awake();
            this.PopulateKeyboard();
            this.UpdateKeyboardVisuals();
        }

        private void OnKeyPressed(char keyChar, string keyString)
        {
            this.keyPressed?.Invoke(keyChar, keyString);

            var inputField = InputFieldTracker.GetCurrentInputField();
            var tmpInputField = InputFieldTracker.GetCurrentTMPInputField();
            string currentText = null;

            if (inputField)
            {
                currentText = inputField.text;
            }
            else if (tmpInputField)
            {
                currentText = inputField.text;
            }

            if (currentText != null)
            {
                int caretStartIndex = Mathf.Min(InputFieldTracker.GetLastKnownSelectionAnchorPosition(), InputFieldTracker.GetLastKnownSelectionFocusPosition());
                int caretEndIndex = Mathf.Max(InputFieldTracker.GetLastKnownSelectionAnchorPosition(), InputFieldTracker.GetLastKnownSelectionFocusPosition());

                //// TODO [bgish]: Still need to handle special cases like delete/backspace

                string newText = currentText;

                if (caretStartIndex != caretEndIndex)
                {
                    newText = newText.Remove(caretStartIndex, caretEndIndex - caretStartIndex);
                }

                newText = newText.Insert(caretStartIndex, keyString);

                if (inputField)
                {
                    inputField.text = newText;
                    inputField.caretPosition = caretStartIndex + 1;
                }
                else if (tmpInputField)
                {
                    tmpInputField.text = newText;
                    tmpInputField.caretPosition = caretStartIndex + 1;
                }
            }

            InputFieldTracker.Update();
        }

        private void PopulateKeyboard()
        {
            var keyboard = this.CurrentKeyboard;

            Populate(nameof(this.lowerCaseRows), this.lowerCaseRows, keyboard?.LowerCaseText);
            Populate(nameof(this.upperCaseRows), this.upperCaseRows, keyboard?.UpperCaseText);
            Populate(nameof(this.numbersRows), this.numbersRows, keyboard?.NumbersText);
            Populate(nameof(this.symbolsRows), this.symbolsRows, keyboard?.SymbolsText);
            Populate(nameof(this.keypadRows), this.keypadRows, keyboard?.KeypadText);

            void Populate(string rowName, Transform[] rows, string text)
            {
                var lines = text.IsNullOrWhitespace() == false ? text.Split('\n') : null;

                if (rows.Length != lines.Length)
                {
                    Debug.LogError($"{rowName} row/teext doesn't match!");
                    return;
                }

                for (int i = 0; i < rows.Length; i++)
                {
                    var row = rows[i];
                    var line = lines[i];

                    // Destorying all the children in this row before repopulating
                    for (int childIndex = row.childCount - 1; childIndex >= 0; childIndex--)
                    {
                        this.SendToPool(row.GetChild(childIndex).gameObject);
                    }

                    for (int j = 0; j < line.Length; j++)
                    {
                        var keyboardKey = this.GetFromPool(row);
                        keyboardKey.SetData(this, line[j], line.Substring(j, 1), this.OnKeyPressed);
                    }
                }
            }
        }

        private void SendToPool(GameObject gameObject)
        {
            // TODO [bgish]: Actually send this to an internal pool
            GameObject.Destroy(gameObject);
        }

        private XRKeyboardKey GetFromPool(Transform parent)
        {
            // TODO [bgish]: Actually get this from the pool
            return GameObject.Instantiate(this.keyPrefab, parent);
        }

        private void UpdateKeyboardVisuals()
        {
            this.lowerCaseLayout.SafeSetActive(this.state == State.Letters && this.isUpperCase == false);
            this.upperCaseLayout.SafeSetActive(this.state == State.Letters && this.isUpperCase);
            this.numbersLayout.SafeSetActive(this.state == State.Numbers);
            this.symbolsLayout.SafeSetActive(this.state == State.Symbols);
        }
    }
}

#endif
