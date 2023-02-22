using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("DebugVector",
            Description = "Represents a debugging vector in the current scene."
        )]
        public class VectorRenderer : IMarker {
            [KSField(Description = "The color of the debugging vector")]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            [KSField] public double Scale { get; set; }

            [KSField(Description = "The width of the debugging vector")]
            public double Width { get; set; }

            [KSField(Description = "Controls if an arrow should be drawn at the end.")]
            public bool Pointy { get; set; }

            private Func<Vector3d> startProvider;
            private Func<Vector3d> endProvider;
            private bool enable;
            private LineRenderer line;
            private LineRenderer hat;
            private GameObject lineObj;
            private GameObject hatObj;
            private GameObject labelObj;
#pragma warning disable CS0618 // ^^^ see above comment about why this is disabled.
            private Text label;
#pragma warning restore CS0618
            private string labelStr;
            private Vector3 labelLocation;

            private Vector3 shipCenterCoords;

            private Vector3 camPos; // camera coordinates.
            private Vector3 camLookVec; // vector from camera to ship position.
            private Vector3 prevCamLookVec;
            private Quaternion camRot;
            private Quaternion prevCamRot;
            private bool isOnMap; // true = Map view, false = Flight view.
            private bool prevIsOnMap;
            private const int MapLayer = 10; // found through trial-and-error
            private const int FlightLayer = 15; // Supposedly the layer for UI effects in flight camera.

            public VectorRenderer(Func<Vector3d> startProvider, Func<Vector3d> endProvider,
                KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) {
                this.startProvider = startProvider;
                this.endProvider = endProvider;
                Color = color;
                Scale = 1.0;
                Width = width;
                Pointy = pointy;
                labelStr = label;
            }

            [KSField(Description = "Controls if the debug-vector is currently visible (initially `true`)")]
            public bool Visible {
                get { return enable; }
                set {
                    if (value) {
                        if (line == null || hat == null) {
                            lineObj = new GameObject("vecdrawLine");
                            hatObj = new GameObject("vecdrawHat");

                            line = lineObj.AddComponent<LineRenderer>();
                            hat = hatObj.AddComponent<LineRenderer>();
#pragma warning disable CS0618 // ^^^ see above comment about why this is disabled.
                            labelObj = new GameObject("vecdrawLabel", typeof(Text));
                            label = labelObj.GetComponent<Text>();
#pragma warning restore CS0618

                            line.useWorldSpace = false;
                            hat.useWorldSpace = false;

                            UpdateShipCenterCoords();

                            // Note the Shader name string below comes from Kerbal's packaged shaders the
                            // game ships with - there's many to choose from but they're not documented what
                            // they are.  This was settled upon via trial and error:
                            // Additionally, Note that in KSP 1.8 because of the Unity update, some of these
                            // shaders Unity previously supplied were removed from Unity's DLLs.  SQUAD packaged them
                            // inside its own DLLs in 1.8 for modders who had been using them.  But because of that,
                            // mods have to use this different path to get to them:
                            Shader vecShader = Shader.Find("Particles/Alpha Blended"); // for when KSP version is < 1.8
                            if (vecShader == null)
                                vecShader = Shader.Find(
                                    "Legacy Shaders/Particles/Alpha Blended"); // for when KSP version is >= 1.8

                            line.material = new Material(vecShader);
                            hat.material = new Material(vecShader);

                            // This is how font loading would work if other fonts were available in KSP:
                            // Font lblFont = (Font)Resources.Load( "Arial", typeof(Font) );
                            // SafeHouse.Logger.Log( "lblFont is " + (lblFont == null ? "null" : "not null") );
                            // _label.font = lblFont;

                            label.text = labelStr;
                            label.alignment = TextAnchor.MiddleCenter;

                            PutAtShipRelativeCoords();
                            RenderValues();
                        }

                        line.enabled = true;
                        hat.enabled = Pointy;
                        label.enabled = true;
                    } else {
                        if (label != null) {
                            label.enabled = false;
                            label = null;
                        }

                        if (hat != null) {
                            hat.enabled = false;
                            hat = null;
                        }

                        if (line != null) {
                            line.enabled = false;
                            line = null;
                        }

                        labelObj = null;
                        hatObj = null;
                        lineObj = null;
                    }

                    enable = value;
                }
            }

            [KSField(Description = "The current starting position of the debugging vector.")]
            public Vector3d Start {
                get => startProvider();
                set => startProvider = () => value;
            }


            [KSField(Description = "The current end position of the debugging vector.")]
            public Vector3d End {
                get => endProvider();
                set => endProvider = () => value;
            }

            /// <summary>
            /// Update _shipCenterCoords, abstracting the different ways to do
            /// it depending on view mode:
            /// </summary>
            private void UpdateShipCenterCoords() {
                if (isOnMap)
                    shipCenterCoords = ScaledSpace.LocalToScaledSpace(
                        FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);
                else
                    shipCenterCoords = FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero;
            }

            /// <summary>
            /// Update camera data, abstracting the different ways KSP does it
            /// depending on view mode:
            /// </summary>
            private void UpdateCamData() {
                prevIsOnMap = isOnMap;
                prevCamLookVec = camLookVec;
                prevCamRot = camRot;

                isOnMap = MapView.MapIsEnabled;

                var cam = Utils.GetCurrentCamera();
                var transform = cam.transform;
                camPos = transform.localPosition;

                // the Distance coming from MapView.MapCamera.Distance
                // doesn't seem to work - calculating it myself below:
                // _camdist = pc.Distance();
                // camRot = cam.GetCameraTransform().rotation;
                camRot = transform.rotation;

                camLookVec = camPos - shipCenterCoords;
            }

            /// <summary>
            /// Get the position in screen coordinates that the 3d world coordinates
            /// project onto, abstracting the two different ways KSP has to access
            /// the camera depending on view mode.
            /// Returned coords are in a system where the screen viewport goes from
            /// (0,0) to (1,1) and the Z coord is how far from the screen it is
            /// (-Z means behind you).
            /// </summary>
            private Vector3 GetViewportPosFor(Vector3 v) {
                var cam = Utils.GetCurrentCamera();
                return cam.WorldToViewportPoint(v);
            }

            /// <summary>
            /// Position the origins of the objects that make up the arrow
            /// such that they anchor relative to current ship position.
            /// </summary>
            private void PutAtShipRelativeCoords() {
                line.transform.localPosition = shipCenterCoords;
                hat.transform.localPosition = shipCenterCoords;
            }

            /// <summary>
            /// Move the origin point of the vector drawings to move with the
            /// current ship, whichever ship that happens to be at the moment,
            /// and move to wherever that ship is within its local XYZ world (which
            /// isn't always at (0,0,0), as it turns out.):
            /// </summary>
            public void Update() {
                if (line == null || hat == null) return;
                if (!enable) return;


                UpdateCamData();
                UpdateShipCenterCoords();
                PutAtShipRelativeCoords();

                SetLayer(isOnMap ? MapLayer : FlightLayer);

                var mapChange = isOnMap != prevIsOnMap;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                var magnitudeChange = prevCamLookVec.magnitude != camLookVec.magnitude;
                if (mapChange || magnitudeChange) {
                    RenderPointCoords();
                    LabelPlacement();
                } else if (prevCamRot != camRot) {
                    LabelPlacement();
                }
            }

            [KSMethod]
            public void Remove() => KSPContext.CurrentContext.RemoveMarker(this);

            public void OnRender() {
            }

            public void SetLayer(int newVal) {
                if (lineObj != null) lineObj.layer = newVal;
                if (hatObj != null) hatObj.layer = newVal;
                if (labelObj != null) labelObj.layer = newVal;
            }

            public void SetLabel(string newVal) {
                labelStr = newVal;
                if (label != null) label.text = labelStr;
                RenderPointCoords();
            }

            public void RenderValues() {
                RenderPointCoords();
                RenderColor();
                UpdateCamData();
                LabelPlacement();
            }

            /// <summary>
            /// Assign the arrow and label's positions in space.  Call
            /// whenever :VEC, :START, or :SCALE change, or when the
            /// game switches between flight view and map view, as they
            /// don't use the same size scale.
            /// </summary>
            public void RenderPointCoords() {
                if (line != null && hat != null) {
                    double mapLengthMult = 1.0; // for scaling when on map view.
                    double mapWidthMult = 1.0; // for scaling when on map view.
                    float useWidth;

                    if (isOnMap) {
                        mapLengthMult = ScaledSpace.InverseScaleFactor;
                        mapWidthMult = Math.Max(camLookVec.magnitude, 100.0f) / 100.0f;
                    }

                    // From point1 to point3 is the vector.
                    // point2 is the spot just short of point3 to start drawing
                    // the pointy hat, if Pointy is enabled:
                    Vector3d start = startProvider();
                    Vector3d vector = endProvider() - start;
                    Vector3d point1 = mapLengthMult * start;
                    Vector3d point2 = mapLengthMult * (start + (Scale * 0.95 * vector));
                    Vector3d point3 = mapLengthMult * (start + (Scale * vector));

                    label.fontSize = (int) (12.0 * (Width / 0.2) * Scale);

                    useWidth = (float) (Width * Scale * mapWidthMult);

                    // Position the arrow line:
                    line.positionCount = 2;
                    line.startWidth = useWidth;
                    line.endWidth = useWidth;
                    line.SetPosition(0, point1);
                    line.SetPosition(1, Pointy ? point2 : point3);

                    // Position the arrow hat.  Note, if Pointy = false, this will be invisible.
                    hat.positionCount = 2;
                    hat.startWidth = useWidth * 3.5f;
                    hat.endWidth = 0.0f;
                    hat.SetPosition(0, point2);
                    hat.SetPosition(1, point3);

                    // Put the label at the midpoint of the arrow:
                    labelLocation = (point1 + point3) / 2;

                    PutAtShipRelativeCoords();
                }
            }

            /// <summary>
            /// Calculates colors and applies transparency fade effect.
            /// Only needs to be called when the color changes.
            /// </summary>
            public void RenderColor() {
                Color c1 = Color.Color;
                Color c2 = Color.Color;
                c1.a = c1.a * (float) 0.25;
                Color lCol =
                    UnityEngine.Color.Lerp(c2, UnityEngine.Color.white, 0.7f); // "whiten" the label color a lot.

                if (line != null && hat != null) {
                    // If Wiping, then the line has the fade effect from color c1 to color c2,
                    // else it stays at c2 the whole way:
                    line.startColor = c1;
                    line.endColor = c2;
                    // The hat does not have the fade effect, staying at color c2 the whole way:
                    hat.startColor = c2;
                    hat.endColor = c2;
                    label.color = lCol; // The label does not have the fade effect.
                }
            }

            /// <summary>
            /// Place the 2D label at the correct projected spot on
            /// the screen from its location in 3D space:
            /// </summary>
            private void LabelPlacement() {
                Vector3 screenPos = GetViewportPosFor(shipCenterCoords + labelLocation);

                // If the projected location is on-screen:
                if (screenPos.z > 0
                    && screenPos.x >= 0 && screenPos.x <= 1
                    && screenPos.y >= 0 && screenPos.y <= 1) {
                    label.enabled = true;
                    label.transform.position = screenPos;
                } else {
                    label.enabled = false;
                }
            }
        }
    }
}
