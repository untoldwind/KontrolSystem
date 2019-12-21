using System;
using NUnit.Framework;
using KontrolSystem.KSP.Runtime.Testing;

namespace KontrolSystem.KSP.Runtime.Test {
    [TestFixture]
    public class MockOrbitTests {
        [Test]
        public void TestShipOrbit() {
            MockOrbit orbit = new MockOrbit(MockBody.Kerbin, 23.0000000004889, 0.499999999787874, 1999999.99888609, 12.0000000035085, 45.000000043628, 2248030.00958346, -1.76866346012346);

            AssertEqual(orbit.FrameX, new Vector3d(0.556326074237772, 0.78368736854604, 0.276288630791373), "Orbit FrameX");
            AssertEqual(orbit.FrameY, new Vector3d(-0.826983528498581, 0.489655834561346, 0.276288630370612), "Orbit FrameY");
            AssertEqual(orbit.FrameZ, new Vector3d(0.0812375696043732, -0.382192715866505, 0.920504853449106), "Orbit FrameZ");
            Assert.AreEqual(0.000664417038265578, orbit.meanMotion, 1e-7, "Orbit mean motion");
            Assert.AreEqual(-2.55587262120358, orbit.GetTrueAnomalyAtUT(2248030.02958346), 1e-7, "Orbit true anomaly");
            AssertEqual(orbit.GetRelativePositionAtUT(2248030.02958346), new Vector3d(-16555.4669097162, -2375291.03194007, -984757.441721877), "Orbit Pos");
            AssertEqual(orbit.GetOrbitalVelocityAtUT(2248030.02958346), new Vector3d(894.837941056022, 414.309011750899, 93.0483164395469), "Orbit Vel");
        }

        [Test]
        public void TestKerbinOrbit() {
            Assert.AreEqual(MockBody.Kerbin.orbit.GetTrueAnomalyAtUT(191234.147219217), -3.01263120009739, 1e-7);
            AssertEqual(MockBody.Kerbin.orbit.GetRelativePositionAtUT(191234.147219217), new Vector3d(-13486907047.0309, -1748997796.87317, 0), "Orbit Pos 1");
            AssertEqual(MockBody.Kerbin.orbit.GetOrbitalVelocityAtUT(191234.147219217), new Vector3d(1194.02661869056, -9207.40211722977, 0), "Orbit Vel 1");
            AssertEqual(MockBody.Kerbin.orbit.GetRelativePositionAtUT(2248727.10958411), new Vector3d(-505812409.43674, -13590430780.3387, 0), "Orbit Pos 2");
            AssertEqual(MockBody.Kerbin.orbit.GetOrbitalVelocityAtUT(2248727.10958411), new Vector3d(9278.07693080402, -345.314031847963, 0), "Orbit Vel 2");
        }

        [Test]
        public void TestFromPositionMun() {
            double UT = 20000.0;
            Vector3d position = MockBody.Mun.GetPositionAtUT(UT);
            Vector3d velocity = MockBody.Mun.GetOrbitalVelocityAtUT(UT);

            MockOrbit orbit = new MockOrbit(MockBody.Kerbin, position, velocity, UT);

            Assert.AreEqual(MockBody.Mun.orbit.inclination, orbit.inclination, 1e-7);
            Assert.AreEqual(MockBody.Mun.orbit.eccentricity, orbit.eccentricity, 1e-7);
            Assert.AreEqual(MockBody.Mun.orbit.semiMajorAxis, orbit.semiMajorAxis, 1e-7);
            AssertEqual(MockBody.Mun.GetPositionAtUT(25000.0), orbit.GetRelativePositionAtUT(25000.0), "Position");
            AssertEqual(MockBody.Mun.GetOrbitalVelocityAtUT(25000.0), orbit.GetOrbitalVelocityAtUT(25000.0), "Position");
        }

        [Test]
        public void TestFromPositionDuna() {
            double UT = 20000.0;
            Vector3d position = MockBody.Duna.GetPositionAtUT(UT);
            Vector3d velocity = MockBody.Duna.GetOrbitalVelocityAtUT(UT);

            MockOrbit orbit = new MockOrbit(MockBody.Kerbol, position, velocity, UT);

            Assert.AreEqual(MockBody.Duna.orbit.inclination, orbit.inclination, 1e-7);
            Assert.AreEqual(MockBody.Duna.orbit.eccentricity, orbit.eccentricity, 1e-7);
            Assert.AreEqual(MockBody.Duna.orbit.semiMajorAxis, orbit.semiMajorAxis, 1e-4);
            Assert.AreEqual(MockBody.Duna.orbit.LAN, orbit.LAN, 1e-7);
            Assert.AreEqual(MockBody.Duna.orbit.argumentOfPeriapsis, orbit.argumentOfPeriapsis, 1e-5);
            AssertEqual(MockBody.Duna.GetPositionAtUT(25000.0), orbit.GetRelativePositionAtUT(25000.0), "Position");
            AssertEqual(MockBody.Duna.GetOrbitalVelocityAtUT(25000.0), orbit.GetOrbitalVelocityAtUT(25000.0), "Position");
        }

        private void AssertEqual(Vector3d expected, Vector3d actual, string message) {
            Assert.AreEqual(expected.x, actual.x, 1e-4, message + ".x");
            Assert.AreEqual(expected.y, actual.y, 1e-4, message + ".y");
            Assert.AreEqual(expected.z, actual.z, 1e-4, message + ".z");
        }
    }

}
