using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("GroundMarker",
            Description = "Represents a ground marker on a given celestial body."
        )]
        public class GroundMarker : IMarker {
            [KSField(Description = "Controls if the ground marker is currently visible (initially `true`)")]
            public bool Visible { get; set; }

            [KSField] public double Rotation { get; set; }


            [KSField] public KSPOrbitModule.GeoCoordinates GeoCoordinates { get; set; }

            [KSField(Description = "The color of the ground marker vector")]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            public GroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates, KSPConsoleModule.RgbaColor color,
                double rotation) {
                Color = color;
                GeoCoordinates = geoCoordinates;
                Rotation = rotation;
                Visible = true;
            }

            [KSMethod]
            public void Remove() => KSPContext.CurrentContext.RemoveMarker(this);
            
            public void Update() {
            }

            public void OnRender() {
                bool map = MapView.MapIsEnabled;
                Color color = Color.Color;
                Vector3d up = GeoCoordinates.SurfaceNormal;
                double height = GeoCoordinates.Body.Radius + Math.Max(GeoCoordinates.TerrainHeight, 0);
                Vector3d position = GeoCoordinates.Body.Position + (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);
                Vector3d center = position + height * up;
                Vector3d camPos = map
                    ? ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position)
                    : (Vector3d) FlightCamera.fetch.mainCamera.transform.position;

                if (GLUtils.IsOccluded(center, position, GeoCoordinates.Body.Radius, camPos)) return;

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
                    color, GLUtils.Colored, map);

                GLUtils.GLTriangle(
                    center,
                    center + radius * (QuaternionD.AngleAxis(Rotation + 110, up) * north),
                    center + radius * (QuaternionD.AngleAxis(Rotation + 130, up) * north),
                    color, GLUtils.Colored, map);

                GLUtils.GLTriangle(
                    center,
                    center + radius * (QuaternionD.AngleAxis(Rotation - 110, up) * north),
                    center + radius * (QuaternionD.AngleAxis(Rotation - 130, up) * north),
                    color, GLUtils.Colored, map);
            }
        }
    }
}
