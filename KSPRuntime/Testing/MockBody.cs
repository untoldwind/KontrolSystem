using System;
using KontrolSystem.KSP.Runtime.KSPOrbit;

namespace KontrolSystem.KSP.Runtime.Testing {
    public class MockBody : KSPOrbitModule.IBody {
        public static MockBody Kerbol = new MockBody("Kerbol", 261600000, 1.17233279483249E+18);
        public static MockBody Kerbin = new MockBody("Kerbin", 600000, Kerbol, 0, 0, 13599840256, 0, 0, 0, 3.14000010490417, 3531600000000, 84159286.4796305, true, 70000);
        public static MockBody Mun = new MockBody("Mun", 200000, Kerbin, 0, 0, 12000000, 0, 0, 0, 1.70000004768372, 65138397520.7807, 2429559.11656475, false, 0);
        public static MockBody Minmus = new MockBody("Minmus", 60000, Kerbin, 6, 0, 47000000, 78, 38, 0, 0.899999976158142, 1765800026.31247, 2247428.3879023, false, 0);
        public static MockBody Duna = new MockBody("Duna", 320000, Kerbol, 0.06, 0.051, 20726155264, 135.5, 0, 0, 3.14000010490417, 301363211975.098, 47921949.369738, true, 50000);
        public static MockBody Ike = new MockBody("Ike", 130000, Duna, 0.2, 0.03, 3200000, 0, 0, 0, 1.7, 18568368573.144, 1049598.93931162, false, 0);
        public static MockBody Eve = new MockBody("Eve", 700000, Kerbol, 2.09999990463257, 0.00999999977648258, 9832684544, 15, 0, 0, 3.14000010490417, 8171730229210.87, 85109364.7382441, true, 90000);
        public static MockBody Gilly = new MockBody("Gilly", 13000, Eve, 12, 0.550000011920929, 31500000, 80, 10, 0, 0.899999976158142, 8289449.81471635, 126123.271704568, false, 0);
        public static MockBody Jool = new MockBody("Jool", 6000000, Kerbol, 1.30400002002716, 0.0500000007450581, 68773560320, 52, 0, 0, 0.100000001490116, 282528004209995, 2455985185.42347, true, 200000);
        public static MockBody Tylo = new MockBody("Tylo", 600000, Jool, 0.025000000372529, 0, 68500000, 0, 0, 0, 3.14000010490417, 2825280042099.95, 10856518.3683586, false, 0);
        public static MockBody Laythe = new MockBody("Laythe", 500000, Jool, 0, 0, 27184000, 0, 0, 0, 3.14000010490417, 1962000029236.08, 3723645.81113302, true, 50000);
        public static MockBody Bop = new MockBody("Bop", 65000, Jool, 15, 0.234999999403954, 128500000, 10, 25, 0, 0.899999976158142, 2486834944.41491, 1221060.86284253, false, 0);
        public static MockBody Pol = new MockBody("Pol", 44000, Jool, 4.25, 0.17085, 179890000, 2, 15, 0, 0.899999976158142, 721702080, 1042138.89230178, false, 0);
        public static MockBody Vall = new MockBody("Vall", 300000, Jool, 0, 0, 43152000, 0, 0, 0, 0.899999976158142, 207481499473.751, 2406401.44479404, false, 0);

        public readonly MockBody parent;
        public readonly string name;
        public readonly MockOrbit orbit;
        public readonly double mu;
        public readonly double soiRadius;
        public readonly double radius;
        public readonly bool hasAtmosphere;
        public readonly double atmosphereDepth;

        public MockBody(string name, double radius, double mu) {
            this.name = name;
            this.mu = mu;
            this.parent = null;
            this.orbit = null;
            this.soiRadius = double.PositiveInfinity;
            this.radius = radius;
            this.hasAtmosphere = false;
            this.atmosphereDepth = 0;
        }

        public MockBody(string name,
                        double radius,
                        MockBody parent,
                        double inclination,
                        double eccentricity,
                        double semiMajorAxis,
                        double LAN,
                        double argumentOfPeriapsis,
                        double epoch,
                        double meanAnomalyAtEpoch,
                        double mu,
                        double soiRadius,
                        bool hasAtmosphere,
                        double atmosphereDepth) {
            this.name = name;
            this.parent = parent;
            this.mu = mu;
            this.soiRadius = soiRadius;
            this.orbit = new MockOrbit(parent, inclination, eccentricity, semiMajorAxis, LAN, argumentOfPeriapsis, epoch, meanAnomalyAtEpoch);
            this.radius = radius;
            this.hasAtmosphere = hasAtmosphere;
            this.atmosphereDepth = atmosphereDepth;
        }

        public string Name => name;

        public double GravParameter => mu;

        public double SOIRadius => soiRadius;

        public bool HasAtmosphere => hasAtmosphere;

        public double AtmosphereDepth => atmosphereDepth;

        public KSPOrbitModule.IOrbit Orbit => orbit;

        public double Radius => radius;

        public Vector3d Position => Vector3d.zero;

        public Vector3d Up => Vector3d.up;

        public Vector3d GetPositionAtUT(double UT) {
            return orbit?.GetRelativePositionAtUT(UT) ?? Vector3d.zero;
        }

        public Vector3d GetOrbitalVelocityAtUT(double UT) {
            return orbit?.GetOrbitalVelocityAtUT(UT) ?? Vector3d.zero;
        }

        public Vector3d GetSurfaceNormal(double lat, double lon) {
            lat *= Math.PI / 180.0;
            lon *= Math.PI / 180.0;
            double phi = Math.Cos(lat);
            double z = Math.Sin(lat);

            return new Vector3d(phi * Math.Cos(lon), z, phi * Math.Sin(lon));
        }

        public double GetSurfaceHeight(double lat, double lon) => radius;

        public double GetLatitude(Vector3d position) {
            Vector3d normalized = position.normalized.xzy;
            return Math.Asin(normalized.z) * 180.0 / Math.PI;
        }

        public double GetLongitude(Vector3d position) {
            Vector3d normalized = position.normalized.xzy;
            return Math.Atan2(normalized.y, normalized.x) * 180.0 / Math.PI;
        }

        public KSPOrbitModule.IOrbit CreateOrbit(Vector3d relPos, Vector3d vel, double UT) {
            return new MockOrbit(this, relPos.SwapYZ(), vel.SwapYZ(), UT);
        }
    }

}
