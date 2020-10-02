using System;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    class OrbitWrapper : KSPOrbitModule.IOrbit {
        private readonly Orbit orbit;

        public OrbitWrapper(Orbit orbit) => this.orbit = orbit;

        public double Apoapsis => orbit.ApA;

        public double Periapsis => orbit.PeA;

        public double ApoapsisRadius => orbit.ApR;

        public double PeriapsisRadius => orbit.PeR;

        public double SemiMajorAxis => orbit.semiMajorAxis;

        public double Inclination => orbit.inclination;

        public double Eccentricity => orbit.eccentricity;

        public double LAN => orbit.LAN;

        public double Epoch => orbit.epoch;

        public double ArgumentOfPeriapsis => orbit.argumentOfPeriapsis;

        public double MeanAnomalyAtEpoch => orbit.meanAnomalyAtEpoch;

        public double Period => orbit.period;

        public KSPOrbitModule.IBody ReferenceBody => new BodyWrapper(orbit.referenceBody);

        public double MeanMotion {
            get {
                if (orbit.eccentricity > 1) {
                    return Math.Sqrt(orbit.referenceBody.gravParameter /
                                     Math.Abs(orbit.semiMajorAxis * orbit.semiMajorAxis * orbit.semiMajorAxis));
                } else {
                    // The above formula is wrong when using the RealSolarSystem mod, which messes with orbital periods.
                    // This simpler formula should be foolproof for elliptical orbits:
                    return 2 * Math.PI / orbit.period;
                }
            }
        }

        public Vector3d OrbitNormal => -orbit.GetOrbitNormal().normalized.SwapYZ();

        public Orbit.PatchTransitionType PatchEndTransition => orbit.patchEndTransition;

        public double PatchEndTime => orbit.EndUT;

        public Vector3d AbsolutePosition(double ut) => orbit.referenceBody.position + RelativePosition(ut);

        public Vector3d OrbitalVelocity(double ut) => orbit.getOrbitalVelocityAtUT(ut).SwapYZ();

        public Vector3d RelativePosition(double ut) => orbit.getRelativePositionAtUT(ut).SwapYZ();

        public Vector3d Prograde(double ut) => OrbitalVelocity(ut).normalized;

        public Vector3d NormalPlus(double ut) => -orbit.GetOrbitNormal().normalized.SwapYZ();

        public Vector3d RadialPlus(double ut) => Vector3d.Exclude(Prograde(ut), Up(ut)).normalized;

        public Vector3d Up(double ut) => RelativePosition(ut).normalized;

        public double Radius(double ut) => RelativePosition(ut).magnitude;

        public Vector3d Horizontal(double ut) => Vector3d.Exclude(Up(ut), Prograde(ut)).normalized;

        public KSPOrbitModule.IOrbit PerturbedOrbit(double ut, Vector3d dV) => new OrbitWrapper(
            OrbitFromStateVectors(AbsolutePosition(ut), OrbitalVelocity(ut) + dV, orbit.referenceBody, ut));

        public double UTAtMeanAnomaly(double meanAnomaly, double ut) {
            double currentMeanAnomaly = MeanAnomalyAtUT(ut);
            double meanDifference = meanAnomaly - currentMeanAnomaly;
            if (orbit.eccentricity < 1) meanDifference = DirectBindingMath.Clamp_Radians_2Pi(meanDifference);
            return ut + meanDifference / MeanMotion;
        }

        public double GetMeanAnomalyAtEccentricAnomaly(double E) {
            double e = orbit.eccentricity;
            if (e < 1) {
                //elliptical orbits
                return DirectBindingMath.Clamp_Radians_2Pi(E - (e * Math.Sin(E)));
            } else {
                //hyperbolic orbits
                return (e * Math.Sinh(E)) - E;
            }
        }

        public double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly) {
            double e = orbit.eccentricity;
            trueAnomaly = DirectBindingMath.Clamp_Radians_2Pi(trueAnomaly);

            if (e < 1) {
                //elliptical orbits
                double cosE = (e + Math.Cos(trueAnomaly)) / (1 + e * Math.Cos(trueAnomaly));
                double sinE = Math.Sqrt(1 - (cosE * cosE));
                if (trueAnomaly > Math.PI) sinE *= -1;

                return DirectBindingMath.Clamp_Radians_2Pi(Math.Atan2(sinE, cosE));
            } else {
                //hyperbolic orbits
                double coshE = (e + Math.Cos(trueAnomaly)) / (1 + e * Math.Cos(trueAnomaly));
                if (coshE < 1)
                    throw new ArgumentException("OrbitExtensions.GetEccentricAnomalyAtTrueAnomaly: True anomaly of " +
                                                trueAnomaly + " radians is not attained by orbit with eccentricity " +
                                                orbit.eccentricity);

                double E = DirectBindingMath.Acosh(coshE);
                if (trueAnomaly > Math.PI) E *= -1;

                return E;
            }
        }

        public double TimeOfTrueAnomaly(double trueAnomaly, double ut) {
            return UTAtMeanAnomaly(GetMeanAnomalyAtEccentricAnomaly(GetEccentricAnomalyAtTrueAnomaly(trueAnomaly)), ut);
        }

        public double MeanAnomalyAtUT(double ut) {
            // We use ObtAtEpoch and not meanAnomalyAtEpoch because somehow meanAnomalyAtEpoch
            // can be wrong when using the RealSolarSystem mod. ObtAtEpoch is always correct.
            double ret = (orbit.ObTAtEpoch + (ut - orbit.epoch)) * MeanMotion;
            if (orbit.eccentricity < 1) ret = DirectBindingMath.Clamp_Radians_2Pi(ret);
            return ret;
        }

        public double NextPeriapsisTime(Option<double> maybeUt = new Option<double>()) {
            double ut = maybeUt.GetValueOrDefault(Planetarium.GetUniversalTime());
            if (orbit.eccentricity < 1) {
                return TimeOfTrueAnomaly(0, ut);
            } else {
                return ut - MeanAnomalyAtUT(ut) / MeanMotion;
            }
        }

        public double NextApoapsisTime(Option<double> maybeUt = new Option<double>()) {
            double ut = maybeUt.GetValueOrDefault(Planetarium.GetUniversalTime());
            if (orbit.eccentricity < 1) {
                return TimeOfTrueAnomaly(Math.PI, ut);
            } else {
                throw new ArgumentException("OrbitExtensions.NextApoapsisTime cannot be called on hyperbolic orbits");
            }
        }

        public double SynodicPeriod(KSPOrbitModule.IOrbit other) {
            int sign = (Vector3d.Dot(OrbitNormal, other.OrbitNormal) > 0 ? 1 : -1); //detect relative retrograde motion
            return Math.Abs(1.0 /
                            (1.0 / Period - sign * 1.0 / other.Period)); //period after which the phase angle repeats
        }

        public double TrueAnomalyAtRadius(double radius) => orbit.TrueAnomalyAtRadius(radius);

        public double NextTimeOfRadius(double ut, double radius) {
            if (radius < orbit.PeR || (orbit.eccentricity < 1 && radius > orbit.ApR))
                throw new ArgumentException("OrbitExtensions.NextTimeOfRadius: given radius of " + radius +
                                            " is never achieved: PeR = " + orbit.PeR + " and ApR = " + orbit.ApR);

            double trueAnomaly1 = orbit.TrueAnomalyAtRadius(radius);
            double trueAnomaly2 = 2 * Math.PI - trueAnomaly1;
            double time1 = TimeOfTrueAnomaly(trueAnomaly1, ut);
            double time2 = TimeOfTrueAnomaly(trueAnomaly2, ut);
            if (time2 < time1 && time2 > ut) return time2;
            else return time1;
        }

        public Vector3d RelativePositionAtPeriapsis {
            get {
                Vector3d vectorToAn = Quaternion.AngleAxis((float) -orbit.LAN, Planetarium.up) * Planetarium.right;
                Vector3d vectorToPe = Quaternion.AngleAxis((float) orbit.argumentOfPeriapsis, OrbitNormal) * vectorToAn;
                return PeriapsisRadius * vectorToPe;
            }
        }

        public double TrueAnomalyFromVector(Vector3d vec) {
            Vector3d oNormal = OrbitNormal;
            Vector3d projected = Vector3d.Exclude(oNormal, vec);
            Vector3d vectorToPe = RelativePositionAtPeriapsis;
            double angleFromPe = Vector3d.Angle(vectorToPe, projected);

            //If the vector points to the infalling part of the orbit then we need to do 360 minus the
            //angle from Pe to get the true anomaly. Test this by taking the the cross product of the
            //orbit normal and vector to the periapsis. This gives a vector that points to center of the
            //outgoing side of the orbit. If vectorToAN is more than 90 degrees from this vector, it occurs
            //during the infalling part of the orbit.
            if (Math.Abs(Vector3d.Angle(projected, Vector3d.Cross(oNormal, vectorToPe))) < 90) {
                return angleFromPe;
            } else {
                return 360 - angleFromPe;
            }
        }

        public double AscendingNodeTrueAnomaly(KSPOrbitModule.IOrbit b) {
            Vector3d vectorToAn = Vector3d.Cross(OrbitNormal, b.OrbitNormal);
            return TrueAnomalyFromVector(vectorToAn);
        }

        public double DescendingNodeTrueAnomaly(KSPOrbitModule.IOrbit b) =>
            DirectBindingMath.Clamp_Degrees_360(AscendingNodeTrueAnomaly(b) + 180);

        public double TimeOfAscendingNode(KSPOrbitModule.IOrbit b, double ut) =>
            TimeOfTrueAnomaly(AscendingNodeTrueAnomaly(b), ut);

        public double TimeOfDescendingNode(KSPOrbitModule.IOrbit b, double ut) =>
            TimeOfTrueAnomaly(DescendingNodeTrueAnomaly(b), ut);

        public static Orbit OrbitFromStateVectors(Vector3d pos, Vector3d vel, CelestialBody body, double ut) {
            Orbit ret = new Orbit();
            ret.UpdateFromStateVectors(Orbit.Swizzle(pos - body.position), Orbit.Swizzle(vel), body, ut);
            if (double.IsNaN(ret.argumentOfPeriapsis)) {
                Vector3d vectorToAn = Quaternion.AngleAxis(-(float) ret.LAN, Planetarium.up) * Planetarium.right;
                Vector3d vectorToPe = Orbit.Swizzle(ret.eccVec);
                double cosArgumentOfPeriapsis =
                    Vector3d.Dot(vectorToAn, vectorToPe) / (vectorToAn.magnitude * vectorToPe.magnitude);
                //Squad's UpdateFromStateVectors is missing these checks, which are needed due to finite precision arithmetic:
                if (cosArgumentOfPeriapsis > 1) {
                    ret.argumentOfPeriapsis = 0;
                } else if (cosArgumentOfPeriapsis < -1) {
                    ret.argumentOfPeriapsis = 180;
                } else {
                    ret.argumentOfPeriapsis = Math.Acos(cosArgumentOfPeriapsis);
                }
            }

            return ret;
        }
    }
}
