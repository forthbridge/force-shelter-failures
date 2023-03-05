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

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

        private static bool isInit = false;

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            if (isInit) return;
            isInit = true;

            MachineConnector.SetRegisteredOI(MOD_ID, Options.instance);

            try
            {
                IL.RainCycle.ctor += RainCycle_ctor;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void RainCycle_ctor(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            ILLabel dest = null!;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(1),
                x => x.MatchCallOrCallvirt<World>("get_game"),
                x => x.MatchCallOrCallvirt<RainWorldGame>("get_GetStorySession"),
                x => x.MatchLdfld<StoryGameSession>("saveState"),
                x => x.MatchLdfld<SaveState>("cycleNumber"),
                x => x.MatchLdcI4(0),
                x => x.MatchBgt(out dest));

            c.RemoveRange(7);
            c.Emit(OpCodes.Br, dest);


            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld<MoreSlugcats.MoreSlugcats>("cfgDisablePrecycles"));

            c.Index += 2;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_1);



            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.0f));

            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<RainCycle, float>>(rainCycle => Options.shelterFailureChance.Value - 1);
        }
    }
}

