//-----------------------------------------------------------------------
// <copyright file="XRDialogSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.XR
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Lost/XR Dialog Settings")]
    public class XRDialogSettings : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private Settings[] settings;
#pragma warning restore 0649

        public Settings CurrentSettings => this.settings[0];

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            [Tooltip("The forwards distance from the camera that this object should be placed.")]
            [SerializeField] private float canvasDistanceForwards = 1.5f;

            [Tooltip("The upwards distance from the camera that this object should be placed.")]
            [SerializeField] private float canvasDistanceUpwards = 0.0f;

            [Tooltip("The speed at which this object changes its position.")]
            [SerializeField] private float positionLerpSpeed = 2.0f;

            [Tooltip("The speed at which this object changes its rotation.")]
            [SerializeField] private float rotationLerpSpeed = 10.0f;
#pragma warning restore 0649

            public float CanvasDistanceForwards => this.canvasDistanceForwards;

            public float CanvasDistanceUpwards => this.canvasDistanceUpwards;

            public float PositionLerpSpeed => this.positionLerpSpeed;

            public float RotationLerpSpeed => this.rotationLerpSpeed;
        }
    }
}
