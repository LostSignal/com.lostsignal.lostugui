//-----------------------------------------------------------------------
// <copyright file="MaskSlider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class MaskSlider : MonoBehaviour
    {
        private RectTransform rectTransform;
        private float width;

        public float Value
        {
            set { this.rectTransform.SetWidth(this.width * value); }
        }

        private void Start()
        {
            this.rectTransform = this.GetComponent<RectTransform>();
            this.width = this.rectTransform.rect.width;
        }
    }
}

#endif
