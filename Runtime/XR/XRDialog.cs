//-----------------------------------------------------------------------
// <copyright file="XRDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.XR
{
    using UnityEngine;

#if USING_UNITY_XR_INTERACTION_TOOLKIT
    using UnityEngine.XR.Interaction.Toolkit;
    using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

    //// Right now the PancakeDevice.Awake (line 47) sets "Cursor.lockState = CursorLockMode.Locked;"
    //// Should this be a part of the XRManager
    //// Whenever a Dialog is shown that has "RequireMouseInPancakeMode", it will turn the mouse back on?????

    //// XR Manager
    ////   Cursor / Mouse Icon

    //// XR Settings
    ////     bool DoesFollowUser
    ////         float canvasDistanceForwards;
    ////         float canvasDistanceUpwards = 0.0f;
    ////         float positionLerpSpeed = 2.0f;
    ////         float rotationLerpSpeed = 10.0f;
    ////
    ////     bool isManipulableByUser;
    ////         float min/max distance;
    ////
    ////     bool requiresMouseInPancakeMode;
    ////
    ////     bool staticWorldSpace;

    [RequireComponent(typeof(Dialog))]
    public class XRDialog : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private XRDialogSettings settings;

#if USING_UNITY_XR_INTERACTION_TOOLKIT
        [SerializeField] private bool isGrabable;
#endif

        [SerializeField] [HideInInspector] private Dialog dialog;
        [SerializeField] [HideInInspector] private Canvas dialogCanvas;
#pragma warning restore 0649

        private bool disableHeadTracking;
        private float originalPlaneDistance;
        private bool isPancakeMode;
        private bool hasBeenGrabbed;

#if USING_UNITY_XR_INTERACTION_TOOLKIT
        private TrackedDeviceGraphicRaycaster trackedDeviceGraphicRaycaster;
        private XRGrabInteractable xrGrabInteractable;
#endif

        public bool DisableHeadTracking
        {
            get => this.disableHeadTracking;
            set => this.disableHeadTracking = value;
        }

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.dialog);
            this.AssertGetComponent(ref this.dialogCanvas);
        }

        private void Awake()
        {
            this.OnValidate();
            this.enabled = this.dialog.ShowOnAwake;
            this.dialog.OnShow.AddListener(this.OnShow);
            this.dialog.OnHide.AddListener(this.OnHide);
            this.originalPlaneDistance = this.dialogCanvas.planeDistance;
        }

        private void Update()
        {
            if (this.disableHeadTracking || this.hasBeenGrabbed)
            {
                return;
            }
            else
            {
                this.UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            var dialogCamera = this.dialogCanvas.worldCamera;

            if (dialogCamera == null)
            {
                return;
            }

            var canvasSettings = this.settings.CurrentSettings;
            var dialogCameraTransform = dialogCamera.transform;

            // Move the object CanvasDistance units in front of the camera
            float positionSpeed = Time.deltaTime * canvasSettings.PositionLerpSpeed;

            Vector3 positionTo =
                dialogCameraTransform.position +
                (dialogCameraTransform.forward * canvasSettings.CanvasDistanceForwards) +
                (dialogCameraTransform.up * canvasSettings.CanvasDistanceUpwards);

            this.transform.position = Vector3.SlerpUnclamped(this.transform.position, positionTo, positionSpeed);

            // Rotate the object to face the camera
            float rotattionSpeed = Time.deltaTime * canvasSettings.RotationLerpSpeed;
            Vector3 diff = this.transform.position - dialogCameraTransform.position;

            if (diff != Vector3.zero)
            {
                Quaternion rotationTo = Quaternion.LookRotation(diff);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationTo, rotattionSpeed);
            }
        }

        private void OnShow()
        {
            //// TODO [bgish]: Do some calculations to spawn this dialog in front of the user

#if USING_UNITY_XR_INTERACTION_TOOLKIT
            this.isPancakeMode = XRManager.Instance != null && XRManager.Instance.IsFlatMode;
#else
            this.isPancakeMode = true;
#endif

            if (this.isPancakeMode == false)
            {
                this.enabled = true;

#if USING_UNITY_XR_INTERACTION_TOOLKIT
                if (this.trackedDeviceGraphicRaycaster == null)
                {
                    this.trackedDeviceGraphicRaycaster = this.GetOrAddComponent<TrackedDeviceGraphicRaycaster>();
                }

                if (this.isGrabable)
                {
                    if (this.xrGrabInteractable == null)
                    {
                        this.xrGrabInteractable = this.GetOrAddComponent<XRGrabInteractable>();
                        this.xrGrabInteractable.selectEntered.AddListener((interactor) => this.hasBeenGrabbed = true);

                        this.xrGrabInteractable.attachEaseInTime = -1.0f;
                        this.xrGrabInteractable.movementType = XRBaseInteractable.MovementType.Instantaneous;
                        this.xrGrabInteractable.throwOnDetach = false;
                        this.xrGrabInteractable.forceGravityOnDetach = false;

                        this.xrGrabInteractable.trackPosition = true;
                        this.xrGrabInteractable.smoothPosition = true;
                        this.xrGrabInteractable.smoothPositionAmount = 3.0f;
                        this.xrGrabInteractable.tightenPosition = 0.0f;

                        this.xrGrabInteractable.trackRotation = true;
                        this.xrGrabInteractable.smoothRotation = true;
                        this.xrGrabInteractable.smoothRotationAmount = 0.5f;
                        this.xrGrabInteractable.tightenRotation = 0.0f;

                        var rigidBody = this.GetComponent<Rigidbody>();
                        rigidBody.useGravity = false;
                        rigidBody.isKinematic = true;
                    }
                }
#endif

                if (this.dialogCanvas.renderMode != RenderMode.WorldSpace)
                {
                    this.dialogCanvas.planeDistance = this.settings.CurrentSettings.CanvasDistanceForwards;

                    this.ExecuteAtEndOfFrame(() =>
                    {
                        this.dialogCanvas.renderMode = RenderMode.WorldSpace;
                        this.dialogCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

                        // Making sure the dialog is directly in front of the camera at the settings canvas distance
                        this.dialogCanvas.transform.position =
                            this.dialogCanvas.worldCamera.transform.position +
                            (this.dialogCanvas.worldCamera.transform.forward.normalized * this.settings.CurrentSettings.CanvasDistanceForwards);
                    });
                }
            }
            else
            {
                this.enabled = false;

                if (this.dialogCanvas.renderMode == RenderMode.WorldSpace)
                {
                    this.dialogCanvas.renderMode = this.dialog.IsOverlayCamera ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
                    this.dialogCanvas.planeDistance = this.originalPlaneDistance;
                }
            }
        }

        private void OnHide()
        {
            this.hasBeenGrabbed = false;
        }
    }
}
