//-----------------------------------------------------------------------
// <copyright file="LostButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class LostButton : Button
    {
        private readonly List<UIAction> actions = new List<UIAction>();

        private SelectionState selectionState = SelectionState.Normal;
        private bool isFirstStateChange = true;
        private RectTransform rectTransform;
        private bool isInitialized = false;

        public UIActionState State
        {
            get
            {
                switch (this.selectionState)
                {
                    case SelectionState.Normal: return UIActionState.Normal;
                    case SelectionState.Highlighted: return UIActionState.Highlighted;
                    case SelectionState.Pressed: return UIActionState.Pressed;
                    case SelectionState.Selected: return UIActionState.Selected;
                    case SelectionState.Disabled: return UIActionState.Disabled;
                    default:
                        Debug.LogError($"Found Unknown Button Selection State {this.selectionState}");
                        return UIActionState.Normal;
                }
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if (!this.rectTransform)
                {
                    this.rectTransform = this.GetComponent<RectTransform>();
                }

                return this.rectTransform;
            }
        }

        public bool IsPressedDown { get; private set; }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            this.IsPressedDown = true;
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            this.IsPressedDown = false;
        }

        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            this.IsPressedDown = false;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return;
            }
            #endif

            this.Initialize();

            if (this.selectionState != state)
            {
                this.UpdateButtonActions(this.selectionState, state);
                this.selectionState = state;
            }
        }

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;
            this.actions.AddRange(this.GetComponentsInChildren<UIAction>());
            this.actions.Sort((x, y) => { return x.Order.CompareTo(y.Order); });
        }

        private void UpdateButtonActions(SelectionState oldState, SelectionState newState)
        {
            // There's nothing to revert on the first state change
            if (this.isFirstStateChange == false)
            {
                // Revert the old button actions
                foreach (var action in this.actions.Where(x => (int)x.State == (int)oldState))
                {
                    action.Revert();
                }
            }

            // Apply the new button actions
            foreach (var action in this.actions.Where(x => (int)x.State == (int)newState))
            {
                action.Apply();
            }

            this.isFirstStateChange = false;
        }
    }
}

#endif
