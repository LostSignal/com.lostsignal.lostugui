//-----------------------------------------------------------------------
// <copyright file="ActivateAction.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using UnityEngine;

    [AddComponentMenu("")]
    [RequireComponent(typeof(LostButton))]
    public class ActivateAction : UIAction
    {
        #pragma warning disable 0649
        [SerializeField] private GameObject actionObject;
        [SerializeField] private bool activate;
        [SerializeField] private LostButton button;
        #pragma warning restore 0649

        public override string Name
        {
            get { return "Activate"; }
        }

        public override Type ActionObjectType
        {
            get { return typeof(GameObject); }
        }

        public override UnityEngine.Object ActionObject
        {
            get { return this.actionObject; }
            set { this.actionObject = (GameObject)value; }
        }

        public override Type ActionValueType
        {
            get { return typeof(bool); }
        }

        public override object ActionValue
        {
            get { return this.activate; }
            set { this.activate = (bool)value; }
        }

        public override void Apply()
        {
            this.actionObject.SafeSetActive(this.activate);
        }

        public override void Revert()
        {
            this.actionObject.SafeSetActive(!this.activate);
        }

        protected override void Awake()
        {
            base.Awake();

            this.OnValidate();

            if (this.State == this.button.State)
            {
                this.Apply();
            }
            else
            {
                this.Revert();
            }
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.button);
        }
    }
}

#endif
