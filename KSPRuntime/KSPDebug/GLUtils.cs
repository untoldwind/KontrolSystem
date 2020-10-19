using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public static class GLUtils {
        private static Material _additive;

        public static Material Additive {
            get {
                if (_additive == null) {
                    var shader = Shader.Find("Hidden/Internal-Colored");
                    Material mat = new Material(shader);
                    mat.hideFlags = HideFlags.HideAndDontSave;
                    // Set blend mode to show destination alpha channel.
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    // Turn off backface culling, depth writes, depth test.
                    mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    mat.SetInt("_ZWrite", 0);
                    mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                    _additive = mat;
                }

                return _additive;
            }
        }

        //Tests if body occludes worldPosition, from the perspective of the planetarium camera
        // https://cesiumjs.org/2013/04/25/Horizon-culling/
        public static bool IsOccluded(Vector3d worldPosition, KSPOrbitModule.IBody body, Vector3d camPos) {
            Vector3d vc = (body.Position - camPos) / (body.Radius - 100);
            Vector3d vt = (worldPosition - camPos) / (body.Radius - 100);

            double vtVc = Vector3d.Dot(vt, vc);

            // In front of the horizon plane
            if (vtVc < vc.sqrMagnitude - 1) return false;

            return vtVc * vtVc / vt.sqrMagnitude > vc.sqrMagnitude - 1;
        }

        public static void GLTriangle(Vector3d worldVertices1, Vector3d worldVertices2, Vector3d worldVertices3,
            Color c, Material material, bool map) {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadOrtho();
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
                    ? PlanetariumCamera.Camera.WorldToViewportPoint(ScaledSpace.LocalToScaledSpace(worldPosition))
                    : FlightCamera.fetch.mainCamera.WorldToViewportPoint(worldPosition);
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
        public static void DrawPath(CelestialBody mainBody, List<Vector3d> points, KSPOrbitModule.IBody body, Color c,
            Material material, bool map, bool dashed = false) {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            GL.Color(c);

            Vector3d camPos = map
                ? ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position)
                : (Vector3d) FlightCamera.fetch.mainCamera.transform.position;

            int step = (dashed ? 2 : 1);
            for (int i = 0; i < points.Count - 1; i += step) {
                if (!IsOccluded(points[i], body, camPos) && !IsOccluded(points[i + 1], body, camPos)) {
                    GLPixelLine(points[i], points[i + 1], map);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
