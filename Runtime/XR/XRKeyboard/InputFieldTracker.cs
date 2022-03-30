//-----------------------------------------------------------------------
// <copyright file="InputFieldTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_LOST_UGUI

namespace Lost.XR
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public static class InputFieldTracker
    {
        private static int currentSelectedGameObjectInstanceId = int.MinValue;
        private static InputField currentInputField;
        private static TMP_InputField currentTMPInputField;
        private static int lastKnownSelectionAnchorPosition;
        private static int lastKnownSelectionFocusPosition;

        public static bool IsInputFieldSelected
        {
            get
            {
                Update();
                return currentInputField || currentTMPInputField;
            }
        }

        public static InputField GetCurrentInputField() => currentInputField;

        public static TMP_InputField GetCurrentTMPInputField() => currentTMPInputField;

        public static int GetLastKnownSelectionAnchorPosition() => lastKnownSelectionAnchorPosition;

        public static int GetLastKnownSelectionFocusPosition() => lastKnownSelectionFocusPosition;

        public static void ReselectLastKnownInputField()
        {
            if (currentInputField)
            {
                EventSystem.current.SetSelectedGameObject(currentInputField.gameObject);
                currentInputField.selectionAnchorPosition = GetLastKnownSelectionAnchorPosition();
                currentInputField.selectionFocusPosition = GetLastKnownSelectionFocusPosition();
            }
            else if (currentTMPInputField)
            {
                EventSystem.current.SetSelectedGameObject(currentTMPInputField.gameObject);
                currentTMPInputField.selectionAnchorPosition = GetLastKnownSelectionAnchorPosition();
                currentTMPInputField.selectionFocusPosition = GetLastKnownSelectionFocusPosition();
            }
        }

        public static void Update()
        {
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (selected)
            {
                var instanceId = selected.GetInstanceID();

                if (instanceId != currentSelectedGameObjectInstanceId)
                {
                    currentSelectedGameObjectInstanceId = instanceId;
                    currentInputField = selected.GetComponent<InputField>();
                    currentTMPInputField = selected.GetComponent<TMP_InputField>();
                }

                if (currentInputField)
                {
                    lastKnownSelectionAnchorPosition = currentInputField.selectionAnchorPosition;
                    lastKnownSelectionFocusPosition = currentInputField.selectionFocusPosition;
                }
                else if (currentTMPInputField)
                {
                    lastKnownSelectionAnchorPosition = currentTMPInputField.selectionAnchorPosition;
                    lastKnownSelectionFocusPosition = currentTMPInputField.selectionFocusPosition;
                }
            }
            else
            {
                currentSelectedGameObjectInstanceId = int.MinValue;
                currentInputField = null;
                currentTMPInputField = null;

                lastKnownSelectionAnchorPosition = -1;
                lastKnownSelectionFocusPosition = -1;
            }
        }
    }
}

#endif
