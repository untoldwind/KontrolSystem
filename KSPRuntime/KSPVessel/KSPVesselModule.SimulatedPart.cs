using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        public class SimulatedPart {
            protected DragCubeList cubes = new DragCubeList();

            public double totalMass = 0;
            public bool shieldedFromAirstream;
            public bool noDrag;
            public bool hasLiftModule;
            private double bodyLiftMultiplier;

            private SimCurves simCurves;

            private QuaternionD vesselToPart;
            private QuaternionD partToVessel;

            public SimulatedPart(Part p, SimCurves simCurves) {
                Rigidbody rigidbody = p.rb;

                totalMass = rigidbody == null
                    ? 0
                    : rigidbody.mass; // TODO : check if we need to use this or the one without the childMass
                shieldedFromAirstream = p.ShieldedFromAirstream;

                noDrag = rigidbody == null && !PhysicsGlobals.ApplyDragToNonPhysicsParts;
                hasLiftModule = p.hasLiftModule;
                bodyLiftMultiplier = p.bodyLiftMultiplier * PhysicsGlobals.BodyLiftMultiplier;

                this.simCurves = simCurves;

                CopyDragCubesList(p.DragCubes, cubes);

                // Rotation to convert the vessel space vesselVelocity to the part space vesselVelocity
                // QuaternionD.LookRotation is not working...
                partToVessel =
                    Quaternion.LookRotation(p.vessel.GetTransform().InverseTransformDirection(p.transform.forward),
                        p.vessel.GetTransform().InverseTransformDirection(p.transform.up));
                vesselToPart = Quaternion.Inverse(partToVessel);
            }

            public virtual Vector3d Drag(Vector3d vesselVelocity, double dragFactor, float mach) {
                if (shieldedFromAirstream || noDrag)
                    return Vector3d.zero;

                Vector3d dragVectorDirLocal = -(vesselToPart * vesselVelocity).normalized;

                cubes.SetDrag(dragVectorDirLocal, mach);

                Vector3d drag = -vesselVelocity.normalized * cubes.AreaDrag * dragFactor;

                return drag;
            }

            public virtual Vector3d Lift(Vector3d vesselVelocity, double liftFactor) {
                if (shieldedFromAirstream || hasLiftModule)
                    return Vector3d.zero;

                // direction of the lift in a vessel centric reference
                Vector3d liftV = partToVessel * ((Vector3d) cubes.LiftForce * bodyLiftMultiplier * liftFactor);

                Vector3d liftVector = ProjectOnPlane(liftV, vesselVelocity);

                return liftVector;
            }

            public virtual bool SimulateAndRollback(double altATGL, double altASL, double endASL, double pressure, double shockTemp, double time, double semiDeployMultiplier) {
                return false;
            }

            public virtual bool Simulate(double altATGL, double altASL, double endASL, double pressure, double shockTemp, double time, double semiDeployMultiplier) {
                return false;
            }
            
            protected void CopyDragCubesList(DragCubeList source, DragCubeList dest) {
                dest.ClearCubes();

                dest.SetPart(source.Part);

                dest.None = source.None;

                // Procedural need access to part so things gets bad quick.
                dest.Procedural = false;

                for (int i = 0; i < source.Cubes.Count; i++) {
                    DragCube c = new DragCube();
                    CopyDragCube(source.Cubes[i], c);
                    dest.Cubes.Add(c);
                }

                dest.SetDragWeights();

                for (int i = 0; i < 6; i++) {
                    dest.WeightedArea[i] = source.WeightedArea[i];
                    dest.WeightedDrag[i] = source.WeightedDrag[i];
                    dest.AreaOccluded[i] = source.AreaOccluded[i];
                    dest.WeightedDepth[i] = source.WeightedDepth[i];
                }

                dest.SetDragWeights();

                dest.DragCurveCd = simCurves.DragCurveCd;
                dest.DragCurveCdPower = simCurves.DragCurveCdPower;
                dest.DragCurveMultiplier = simCurves.DragCurveMultiplier;

                dest.BodyLiftCurve.liftCurve = simCurves.LiftCurve;
                dest.BodyLiftCurve.dragCurve = simCurves.DragCurve;
                dest.BodyLiftCurve.dragMachCurve = simCurves.DragMachCurve;
                dest.BodyLiftCurve.liftMachCurve = simCurves.LiftMachCurve;

                dest.SurfaceCurves.dragCurveMultiplier = simCurves.DragCurveMultiplier;
                dest.SurfaceCurves.dragCurveSurface = simCurves.DragCurveSurface;
                dest.SurfaceCurves.dragCurveTail = simCurves.DragCurveTail;
                dest.SurfaceCurves.dragCurveTip = simCurves.DragCurveTip;
            }
            
            protected static void CopyDragCube(DragCube source, DragCube dest) {
                dest.Name = source.Name;
                dest.Weight = source.Weight;
                dest.Center = source.Center;
                dest.Size = source.Size;
                for (int i = 0; i < source.Drag.Length; i++) {
                    dest.Drag[i] = source.Drag[i];
                    dest.Area[i] = source.Area[i];
                    dest.Depth[i] = source.Depth[i];
                    dest.DragModifiers[i] = source.DragModifiers[i];
                }
            }
        

            protected static Vector3d ProjectOnPlane(Vector3d vector, Vector3d planeNormal) {
                return vector - Vector3d.Project(vector, planeNormal);
            }

            protected void SetCubeWeight(string name, float newWeight) {
                int count = cubes.Cubes.Count;
                if (count == 0) {
                    return;
                }

                bool noChange = true;
                for (int i = count - 1; i >= 0; i--) {
                    if (cubes.Cubes[i].Name == name && cubes.Cubes[i].Weight != newWeight) {
                        cubes.Cubes[i].Weight = newWeight;
                        noChange = false;
                    }
                }

                if (noChange)
                    return;

                cubes.SetDragWeights();
            }
        }

    }
}
