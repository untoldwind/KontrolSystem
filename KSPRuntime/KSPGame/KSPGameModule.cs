using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    [KSModule("ksp::game",
        Description = "Collection to game and runtime related functions."
    )]
    public partial class KSPGameModule {
        IKSPContext context;

        public KSPGameModule(IContext _context, Dictionary<string, object> modules) {
            context = _context as IKSPContext;
            if (context == null) throw new ArgumentException($"{_context} is not an IKSPContext");
        }

        [KSFunction(Description =
            @"Get the current game scene.

              Results may be:
              * `SPACECENTER`: Game is currently showing the outside of the space center.
              * `EDITOR`: Game is currently showing the VAB or SPH.
              * `FLIGHT`: Game is currently in flight of a vessel.
              * `TRACKINGSTATION`: Game is currently showing the tracking station.
             "
        )]
        public string CurrentScene() => context.CurrentScene.ToString();

        [KSFunction(
            Description = "Get the current universal time (UT) in seconds from start."
        )]
        public double CurrentTime() => Planetarium.GetUniversalTime();

        [KSFunction(
            Description = "Yield execution to allow Unity to do some other stuff inbetween."
        )]
        public Future<object> Yield() {
            context.NextYield = new WaitForFixedUpdate();
            return new Future.Success<object>(null);
        }

        [KSFunction(
            Description = "Stop execution of given number of seconds (factions of a seconds are supported as well)."
        )]
        public Future<object> Sleep(double seconds) {
            context.NextYield = new WaitForSeconds((float)seconds);
            return new Future.Success<object>(null);
        }

        [KSFunction(
            Description = "Stop execution until a given condition is met."
        )]
        public Future<object> WaitUntil(Func<bool> predicate) {
            context.NextYield = new WaitUntil(() => {
                context?.ResetTimeout();
                return predicate();
            });
            return new Future.Success<object>(null);
        }

        [KSFunction(
            Description = "Stop execution as long as a given condition is met."
        )]
        public Future<object> WaitWhile(Func<bool> predicate) {
            context.NextYield = new WaitWhile(() => {
                context?.ResetTimeout();
                return predicate();
            });
            return new Future.Success<object>(null);
        }
    }
}
