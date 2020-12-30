using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using FinePrint;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    [KSModule("ksp::orbit")]
    public partial class KSPOrbitModule {
        private static Dictionary<string, int> _greekMap;
        
        [KSFunction]
        public static Result<IBody, string> FindBody(string name) {
            IBody body = KSPContext.CurrentContext.Bodies.FirstOrDefault(b => b.Name == name);
            return body != null ? Result.Ok<IBody, string>(body) : Result.Err<IBody, string>($"No such body '{name}'");
        }

        [KSFunction]
        public static WaypointAdapter[] AllWaypoints() {
            WaypointManager wpm = WaypointManager.Instance();

            if (wpm == null) return new WaypointAdapter[0];

            return wpm.Waypoints.Select(waypoint => new WaypointAdapter(waypoint)).ToArray();
        }

        [KSFunction]
        public static Result<WaypointAdapter, string> FindWaypoint(string name) {
            WaypointManager wpm = WaypointManager.Instance();


            if (wpm != null) {
                var (baseName, index) = GreekToInteger(name);

                if (index >= 0) {
                    var waypoint = wpm.Waypoints.FirstOrDefault(p =>
                        string.Equals(p.name, baseName, StringComparison.InvariantCultureIgnoreCase) &&
                        p.index == index);

                    if (waypoint != null) return Result.Ok<WaypointAdapter, string>(new WaypointAdapter(waypoint));
                } else {
                    var waypoint = wpm.Waypoints.FirstOrDefault(p =>
                        string.Equals(p.name, name, StringComparison.InvariantCultureIgnoreCase));

                    if (waypoint != null) return Result.Ok<WaypointAdapter, string>(new WaypointAdapter(waypoint));
                }
            }

            return Result.Err<WaypointAdapter, string>($"No waypoint '{name}' found");
        }

        private static (string, int) GreekToInteger(string greekLetterName) {
            int lastSpace = greekLetterName.LastIndexOf(' ');
            if (lastSpace >= 0 && lastSpace < greekLetterName.Length - 1) {
                string lastTerm = greekLetterName.Substring(lastSpace + 1).ToLowerInvariant();
                string baseName = greekLetterName.Substring(0, lastSpace);
                int index;

                if (GreekMap.TryGetValue(lastTerm, out index)) {
                    return (baseName, index);
                }
            }

            return (greekLetterName, -1);
        }

        private static Dictionary<string, int> GreekMap {
            get  {
                if (_greekMap != null) return _greekMap;
                _greekMap = new Dictionary<string, int>();
                for (int i = 0 ; i < 20 ; ++i)
                    _greekMap.Add(FinePrint.Utilities.StringUtilities.IntegerToGreek(i).ToLower(), i);
                return _greekMap;
            }
        }
    }
}
