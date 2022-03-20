//-----------------------------------------------------------------------
// <copyright file="DebugMenuManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System;
    using System.Collections;

    #if USING_PLAYFAB
    using global::PlayFab.ClientModels;
    using global::PlayFab.Internal;
    #endif

    using Lost.CloudBuild;
    using UnityEngine;

    public sealed class DebugMenuManager : Manager<DebugMenuManager>
    {
#pragma warning disable 0649
        [SerializeField] private bool developmentBuildsOnly = true;

        [Header("Settings")]
        [SerializeField] private DebugMenu.DebugMenuSettings settings = new DebugMenu.DebugMenuSettings();

        [Header("Overlay Options")]
        [SerializeField] private bool showAppVersionInLowerLeftKey = true;
        [SerializeField] private bool showPlayFabIdInLowerRight = true;

        [Header("Debug Menu Options")]
        [SerializeField] private bool showTestAd = true;
        [SerializeField] private bool showToggleFps = true;
        [SerializeField] private bool showPrintAdsInfo = true;
        [SerializeField] private bool addRebootButton = true;
#pragma warning restore 0649

        private string versionAndCommitId;

        public override void Initialize()
        {
            if (this.developmentBuildsOnly == false || Application.isEditor || Debug.isDebugBuild)
            {
                this.StartCoroutine(InitializeSettings());
            }
            else
            {
                this.SetInstance(this);
            }

            IEnumerator InitializeSettings()
            {
                yield return DialogManager.WaitForInitialization();

                var debugMenu = DialogManager.GetDialog<DebugMenu>();

                debugMenu.SetSettings(this.settings);

                if (this.showAppVersionInLowerLeftKey)
                {
                    if (this.versionAndCommitId == null)
                    {
                        var version = Application.version;
                        var commitId = CloudBuildManifest.Find()?.ScmCommitId;
                        this.versionAndCommitId = commitId == null ? version : string.Format($"{version} ({commitId})");
                    }

                    debugMenu.SetText(Corner.LowerLeft, this.versionAndCommitId);
                }

                if (this.showPlayFabIdInLowerRight)
                {
                    #if USING_PLAYFAB

                    PlayFab.PlayFabManager.OnInitialized += () =>
                    {
                        if (PlayFab.PlayFabManager.Instance.Login.IsLoggedIn)
                        {
                            var debugMenu = DialogManager.GetDialog<DebugMenu>();
                            var playfabId = PlayFab.PlayFabManager.Instance.Login.IsLoggedIn ? PlayFab.PlayFabManager.Instance.User.PlayFabId : "Login Error!";
                            debugMenu.SetText(Corner.LowerRight, playfabId);
                        }
                    };

                    #endif
                }

                if (this.showTestAd)
                {
                    throw new NotImplementedException();
                    //// debugMenu.AddItem("Show Test Ad", ShowTestAd);
                }

                if (this.showToggleFps)
                {
                    debugMenu.AddItem("Toggle FPS", ToggleFps);
                }

                if (this.showPrintAdsInfo)
                {
                    throw new NotImplementedException();
                    //// debugMenu.AddItem("Print Ads Info", PrintAdsInfo);
                }

                if (this.addRebootButton)
                {
                    throw new NotImplementedException();

                    //// Not sure where bootloader will live so commenting out for now
                    //// debugMenu.AddItem("Reboot", Bootloader.Reboot);
                }

                debugMenu.Dialog.Show();

                this.SetInstance(this);
            }
        }

        private static void ToggleFps()
        {
            DialogManager.GetDialog<DebugMenu>().ToggleFPS();
        }
    }
}

#endif
