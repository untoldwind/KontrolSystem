using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("GroundMarker",
            Description = "Represents a ground marker on a given celestial body."
        )]
        public class GroundMarker : IMarker {
            [KSField(IncludeSetter = true,
                Description = "Controls if the ground marker is currently visible (initially `true`)"
            )]
            public bool Visible { get; set; }

            [KSField(IncludeSetter = true)] public double Rotation { get; set; }


            [KSField(IncludeSetter = true)] public KSPOrbitModule.GeoCoordinates GeoCoordinates { get; set; }

            [KSField(IncludeSetter = true,
                Description = "The color of the debugging vector"
            )]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            public GroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates, KSPConsoleModule.RgbaColor color,
                double rotation) {
                Color = color;
                GeoCoordinates = geoCoordinates;
                Rotation = rotation;
                Visible = true;
            }

            public void Update() {
            }

            public void OnRender() {
                bool map = MapView.MapIsEnabled;
                Color color = Color.Color;
                Vector3d up = GeoCoordinates.SurfaceNormal;
                double height = Math.Max(GeoCoordinates.SurfaceHeight, GeoCoordinates.Body.Radius);
                Vector3d position = GeoCoordinates.Body.Position + (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);
                Vector3d center = GeoCoordinates.Body.Position + height * up;
                Vector3d camPos = map
                    ? ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position)
                    : (Vector3d) FlightCamera.fetch.mainCamera.transform.position;

                if (GLUtils.IsOccluded(center, GeoCoordinates.Body, camPos)) return;

                Vector3d north = Vector3d.Exclude(up, GeoCoordinates.Body.Up).normalized;
                double radius = map ? GeoCoordinates.Body.Radius / 50 : 5;

                if (!map) {
                    Vector3 centerPoint = FlightCamera.fetch.mainCamera.WorldToViewportPoint(center);
                    if (centerPoint.z < 0)
                        return;
                }

                GLUtils.GLTriangle(
                    center,
                    center + radius * (QuaternionD.AngleAxis(Rotation - 10, up) * north),
                    center + radius * (QuaternionD.AngleAxis(Rotation + 10, up) * north),
                    color, GLUtils.Additive, map);

                GLUtils.GLTriangle(
                    center,
                    center + radius * (QuaternionD.AngleAxis(Rotation + 110, up) * north),
                    center + radius * (QuaternionD.AngleAxis(Rotation + 130, up) * north),
                    color, GLUtils.Additive, map);

                GLUtils.GLTriangle(
                    center,
                    center + radius * (QuaternionD.AngleAxis(Rotation - 110, up) * north),
                    center + radius * (QuaternionD.AngleAxis(Rotation - 130, up) * north),
                    color, GLUtils.Additive, map);
            }
        }
    }
}
