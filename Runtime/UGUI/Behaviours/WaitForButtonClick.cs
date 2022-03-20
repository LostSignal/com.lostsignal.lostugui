//-----------------------------------------------------------------------
// <copyright file="WaitForButtonClick.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    public class WaitForButtonClick : CustomYieldInstruction
    {
        private readonly Button button;
        private bool isDone;

        public WaitForButtonClick(Button button)
        {
            this.isDone = false;
            this.button = button;
            this.button.onClick.AddListener(this.OnClick);
        }

        public override bool keepWaiting
        {
            get { return this.isDone == false; }
        }

        private void OnClick()
        {
            this.isDone = true;
            this.button.onClick.RemoveListener(this.OnClick);
        }
    }
}

#endif
