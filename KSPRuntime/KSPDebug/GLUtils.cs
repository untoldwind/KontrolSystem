using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public static class GLUtils {
        private static Material _colored;

        public static Material Colored {
            get {
                if (_colored == null) {
                    _colored = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
                }

                return _colored;
            }
        }

        //Tests if body occludes worldPosition, from the perspective of the planetarium camera
        // https://cesiumjs.org/2013/04/25/Horizon-culling/
        public static bool IsOccluded(Vector3d worldPosition, Vector3d bodyPosition, double bodyRadius, Vector3d camPos) {
            Vector3d vc = (bodyPosition - camPos) / (bodyRadius - 100);
            Vector3d vt = (worldPosition - camPos) / (bodyRadius - 100);

            double vtVc = Vector3d.Dot(vt, vc);

            // In front of the horizon plane
            if (vtVc < vc.sqrMagnitude - 1) return false;

            return vtVc * vtVc / vt.sqrMagnitude > vc.sqrMagnitude - 1;
        }

        public static void GLTriangle(Vector3d worldVertices1, Vector3d worldVertices2, Vector3d worldVertices3,
            Color c, Material material, bool map) {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.TRIANGLES);
            GL.Color(c);
            GLVertex(worldVertices1, map);
            GLVertex(worldVertices2, map);
            GLVertex(worldVertices3, map);
            GL.End();
            GL.PopMatrix();
        }

        public static void GLVertex(Vector3d worldPosition, bool map) {
            Vector3 screenPoint =
                map
                    ? PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition))
                    : FlightCamera.fetch.mainCamera.WorldToScreenPoint(worldPosition);
            GL.Vertex3(screenPoint.x, screenPoint.y, 0);
        }

        public static void GLPixelLine(Vector3d worldPosition1, Vector3d worldPosition2, bool map) {
            Vector3 screenPoint1, screenPoint2;
            if (map) {
                screenPoint1 =
                    PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition1));
                screenPoint2 =
                    PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition2));
            } else {
                screenPoint1 = FlightCamera.fetch.mainCamera.WorldToScreenPoint(worldPosition1);
                screenPoint2 = FlightCamera.fetch.mainCamera.WorldToScreenPoint(worldPosition2);
            }

            if (screenPoint1.z > 0 && screenPoint2.z > 0) {
                GL.Vertex3(screenPoint1.x, screenPoint1.y, 0);
                GL.Vertex3(screenPoint2.x, screenPoint2.y, 0);
            }
        }

        //If dashed = false, draws 0-1-2-3-4-5...
        //If dashed = true, draws 0-1 2-3 4-5...
        public static void DrawPath(Vector3d[] points, Vector3d bodyPosition, double bodyRadius, Color c,
            Material material, bool dashed, bool map) {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            GL.Color(c);

            Vector3d camPos = map
                ? ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position)
                : (Vector3d)FlightCamera.fetch.mainCamera.transform.position;

            int step = (dashed ? 2 : 1);
            for (int i = 0; i < points.Length - 1; i += step) {
                if (!IsOccluded(points[i], bodyPosition, bodyRadius, camPos) && !IsOccluded(points[i + 1], bodyPosition, bodyRadius, camPos)) {
                    GLPixelLine(points[i], points[i + 1], map);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
