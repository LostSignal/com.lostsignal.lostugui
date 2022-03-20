//-----------------------------------------------------------------------
// <copyright file="DialogManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class DialogManager : Manager<DialogManager>
    {
        private readonly Dictionary<System.Type, DialogLogic> dialogTypes = new Dictionary<System.Type, DialogLogic>();
        private readonly LinkedList<Dialog> dialogs = new LinkedList<Dialog>();
        private readonly List<DialogLogic> instantiatedDialogs = new List<DialogLogic>();

        //// TODO [bgish]: Update GetDialog to look out for these types and create them on the fly if needed
#pragma warning disable 0649
        [SerializeField] private DialogLogic[] onDemandDialogs;
#pragma warning restore 0649

        public static void RegisterDialog(DialogLogic dialogLogic)
        {
            if (DialogManager.IsInitialized)
            {
                DialogManager.Instance.dialogTypes.Add(dialogLogic.GetType(), dialogLogic);
            }
            else
            {
                DialogManager.OnInitialized += () =>
                {
                    DialogManager.Instance.dialogTypes.Add(dialogLogic.GetType(), dialogLogic);
                };
            }
        }

        public static void UnregisterDialog(DialogLogic dialogLogic)
        {
            if (DialogManager.Instance)
            {
                DialogManager.Instance.dialogTypes.Remove(dialogLogic.GetType());
            }
        }

        public static void ForceUpdateDialogCameras(Camera newCamera)
        {
            foreach (var dialogLogic in DialogManager.Instance.dialogTypes.Values)
            {
                dialogLogic.Dialog.ForceUpdateCamera(newCamera);
            }
        }

        public static T GetDialog<T>()
            where T : DialogLogic
        {
            if (DialogManager.Instance.dialogTypes.TryGetValue(typeof(T), out DialogLogic dialogLogic))
            {
                return (T)dialogLogic;
            }

            if (DialogManager.Instance.onDemandDialogs == null)
            {
                return null;
            }

            for (int i = 0; i < DialogManager.Instance.onDemandDialogs.Length; i++)
            {
                var prefab = DialogManager.Instance.onDemandDialogs[i];
                var dailogLogicComponent = prefab.GetComponent<T>();

                if (dailogLogicComponent)
                {
                    var newDialog = GameObject.Instantiate(prefab);
                    newDialog.gameObject.name = newDialog.gameObject.name.Substring(0, newDialog.gameObject.name.Length - "(Clone)".Length);
                    DialogManager.Instance.instantiatedDialogs.Add(newDialog);
                    GameObject.DontDestroyOnLoad(newDialog.gameObject);
                    return newDialog.GetComponent<T>();
                }
            }

            return null;
        }

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        public bool IsTopMostDialog(Dialog dialog)
        {
            return this.dialogs.Last != null && this.dialogs.Last.Value == dialog;
        }

        public void AddDialog(Dialog dialog)
        {
            if (dialog != null && dialog.RegisterForBackButton && this.dialogs.Contains(dialog) == false)
            {
                this.dialogs.AddLast(dialog);
            }
        }

        public void RemoveDialog(Dialog dialog)
        {
            if (dialog != null && dialog.RegisterForBackButton && this.dialogs.Contains(dialog))
            {
                this.dialogs.Remove(dialog);
            }
        }

        public void BackButtonPressed()
        {
            if (this.dialogs.Count > 0)
            {
                this.dialogs.Last.Value.BackButtonPressed();
            }
        }

#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
        private void Update()
        {
            // NOTE [bgish]: this catches the Android Back Button
#if USING_UNITY_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
#else
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
#endif
            {
                this.BackButtonPressed();
            }
        }

        private void OnDestroy()
        {
            // Making sure we destory all dialogs we created
            foreach (var dialog in this.instantiatedDialogs)
            {
                if (dialog)
                {
                    GameObject.Destroy(dialog.gameObject);
                }
            }

            this.instantiatedDialogs.Clear();
        }

#endif
    }
}

#endif
