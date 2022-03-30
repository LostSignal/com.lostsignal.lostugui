//-----------------------------------------------------------------------
// <copyright file="XRKeyboardData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_LOST_UGUI

namespace Lost.XR
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Lost/XR Keyboard Data")]
    public class XRKeyboardData : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private List<Keyboard> keyboards;
#pragma warning restore 0649

        public Keyboard CurrentKeyboard => this.keyboards.IsNullOrEmpty() == false ? this.keyboards[0] : null;

        [Serializable]
        public class Keyboard
        {
#pragma warning disable 0649, CA2235
            [SerializeField] private string name;
            [SerializeField] [Multiline(4)] private string lowerCaseText;
            [SerializeField] [Multiline(4)] private string upperCaseText;
            [SerializeField] [Multiline(4)] private string numbersText;
            [SerializeField] [Multiline(4)] private string symbolsText;
            [SerializeField] [Multiline(5)] private string keypadText;
            [SerializeField] private List<string> alternates = new List<string>();
#pragma warning restore 0649, CA2235

            public string LowerCaseText => this.lowerCaseText;

            public string UpperCaseText => this.upperCaseText;

            public string NumbersText => this.numbersText;

            public string SymbolsText => this.symbolsText;

            public string KeypadText => this.keypadText;

            public string GetSeconardyKeys(char c)
            {
                if (this.alternates.IsNullOrEmpty())
                {
                    return null;
                }

                for (int i = 0; i < this.alternates.Count; i++)
                {
                    var secondaryKeys = this.alternates[i];
                    if (secondaryKeys[0] == c)
                    {
                        return secondaryKeys.Substring(2);
                    }
                }

                return null;
            }
        }
    }
}

#endif
