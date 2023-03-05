using BepInEx;
using BepInEx.Logging;
using System.Security.Permissions;
using System.Security;
using System.Collections.Generic;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[module: UnverifiableCode]
#pragma warning restore CS0618 // Type or member is obsolete


namespace ForceShelterFailures
{
    [BepInPlugin(AUTHOR + "." + MOD_ID, MOD_NAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger { get; private set; } = null!;

        public const string VERSION = "1.0.0";
        public const string MOD_NAME = "Force Shelter Failures";
        public const string MOD_ID = "forceshelterfailures";
        public const string AUTHOR = "forthbridge";

        public void OnEnable()
        {
            Logger = base.Logger;

            On.RainWorld.LoadSetupValues += RainWorld_LoadSetupValues;
        }

        private RainWorldGame.SetupValues RainWorld_LoadSetupValues(On.RainWorld.orig_LoadSetupValues orig, bool distributionBuild)
        {
            RainWorldGame.SetupValues setupValues = orig(distributionBuild);
            
            setupValues.forcePrecycles = true;
            return setupValues;
        }
    }
}

