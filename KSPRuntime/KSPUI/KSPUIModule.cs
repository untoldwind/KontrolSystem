using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    [KSModule("ksp::ui", Description =
        @"Provides functions to create base UI windows and dialogs."
    )]
    public partial class KSPUIModule {
        [KSFunction(
            "show_window"
        )]
        public static IWindow<T> ShowWindow<T>(T initialState, Func<T, bool> isEndState,
            Action<IContainer<T>, T> render) {
            return KSPContext.CurrentContext.ShowWindow(initialState, isEndState, render);
        }
    }
}
