using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using KontrolSystem.Plugin.Config;
using KontrolSystem.Parsing;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.Plugin.Utils;
using UnityEngine;

namespace KontrolSystem.Plugin.Core {
    internal class State {
        internal KontrolRegistry registry;

        internal TimeSpan bootTime;

        internal List<MainframeError> errors;

        internal State(KontrolRegistry _registry, TimeSpan _bootTime, List<MainframeError> _errors) {
            registry = _registry;
            bootTime = _bootTime;
            errors = _errors;
        }
    }
}
