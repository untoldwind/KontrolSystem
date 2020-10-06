using System;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    [KSModule("ksp::game",
        Description = "Collection to game and runtime related functions."
    )]
    public class KSPGameModule {
        [KSFunction(Description =
            @"Get the current game scene.

              Results may be:
              * `SPACECENTER`: Game is currently showing the outside of the space center.
              * `EDITOR`: Game is currently showing the VAB or SPH.
              * `FLIGHT`: Game is currently in flight of a vessel.
              * `TRACKINGSTATION`: Game is currently showing the tracking station.
             "
        )]
        public static string CurrentScene() => KSPContext.CurrentContext.CurrentScene.ToString();

        [KSFunction(
            Description = "Get the current universal time (UT) in seconds from start."
        )]
        public static double CurrentTime() => Planetarium.GetUniversalTime();

        [KSFunction(
            Description = "Yield execution to allow Unity to do some other stuff inbetween."
        )]
        public static Future<object> Yield() {
            KSPContext.CurrentContext.NextYield = new WaitForFixedUpdate();
            return new Future.Success<object>(null);
        }

        [KSFunction(
            Description = "Stop execution of given number of seconds (factions of a seconds are supported as well)."
        )]
        public static Future<object> Sleep(double seconds) {
            KSPContext.CurrentContext.NextYield = new WaitForSeconds((float) seconds);
            return new Future.Success<object>(null);
        }

        [KSFunction(
            Description = "Stop execution until a given condition is met."
        )]
        public static Future<object> WaitUntil(Func<bool> predicate) {
            IKSPContext context = KSPContext.CurrentContext;
            KSPContext.CurrentContext.NextYield = new WaitUntil(() => {
                try {
                    ContextHolder.CurrentContext.Value = context;
                    context.ResetTimeout();
                    return predicate();
                } finally {
                    ContextHolder.CurrentContext.Value = null;
                }
            });
            return new Future.Success<object>(null);
        }

        [KSFunction(
            Description = "Stop execution as long as a given condition is met."
        )]
        public static Future<object> WaitWhile(Func<bool> predicate) {
            IKSPContext context = KSPContext.CurrentContext;
            KSPContext.CurrentContext.NextYield = new WaitWhile(() => {
                try {
                    ContextHolder.CurrentContext.Value = context;
                    context.ResetTimeout();
                    return predicate();
                } finally {
                    ContextHolder.CurrentContext.Value = null;
                }
            });
            return new Future.Success<object>(null);
        }
    }
}
