using Xunit;
using KontrolSystem.KSP.Runtime.Testing;

namespace KontrolSystem.KSP.Runtime.Test {
    public class MockOrbitTests {
        [Fact]
        public void TestShipOrbit() {
            MockOrbit orbit = new MockOrbit(MockBody.Kerbin, 23.0000000004889, 0.499999999787874, 1999999.99888609,
                12.0000000035085, 45.000000043628, 2248030.00958346, -1.76866346012346);

            AssertEqual(orbit.FrameX, new Vector3d(0.556326074237772, 0.78368736854604, 0.276288630791373));
            AssertEqual(orbit.FrameY, new Vector3d(-0.826983528498581, 0.489655834561346, 0.276288630370612));
            AssertEqual(orbit.FrameZ, new Vector3d(0.0812375696043732, -0.382192715866505, 0.920504853449106));
            Assert.Equal(0.000664417038265578, orbit.meanMotion, 7);
            Assert.Equal(-2.55587262120358, orbit.GetTrueAnomalyAtUT(2248030.02958346), 7);
            AssertEqual(orbit.GetRelativePositionAtUT(2248030.02958346),
                new Vector3d(-16555.4669097162, -2375291.03194007, -984757.441721877));
            AssertEqual(orbit.GetOrbitalVelocityAtUT(2248030.02958346),
                new Vector3d(894.837941056022, 414.309011750899, 93.0483164395469));
        }

        [Fact]
        public void TestKerbinOrbit() {
            Assert.Equal(MockBody.Kerbin.orbit.GetTrueAnomalyAtUT(191234.147219217), -3.01263120009739, 7);
            AssertEqual(MockBody.Kerbin.orbit.GetRelativePositionAtUT(191234.147219217),
                new Vector3d(-13486907047.0309, -1748997796.87317, 0));
            AssertEqual(MockBody.Kerbin.orbit.GetOrbitalVelocityAtUT(191234.147219217),
                new Vector3d(1194.02661869056, -9207.40211722977, 0));
            AssertEqual(MockBody.Kerbin.orbit.GetRelativePositionAtUT(2248727.10958411),
                new Vector3d(-505812409.43674, -13590430780.3387, 0));
            AssertEqual(MockBody.Kerbin.orbit.GetOrbitalVelocityAtUT(2248727.10958411),
                new Vector3d(9278.07693080402, -345.314031847963, 0));
        }

        [Fact]
        public void TestFromPositionMun() {
            double UT = 20000.0;
            Vector3d position = MockBody.Mun.GetPositionAtUT(UT);
            Vector3d velocity = MockBody.Mun.GetOrbitalVelocityAtUT(UT);

            MockOrbit orbit = new MockOrbit(MockBody.Kerbin, position, velocity, UT);

            Assert.Equal(MockBody.Mun.orbit.inclination, orbit.inclination, 7);
            Assert.Equal(MockBody.Mun.orbit.eccentricity, orbit.eccentricity, 7);
            Assert.Equal(MockBody.Mun.orbit.semiMajorAxis, orbit.semiMajorAxis, 7);
            AssertEqual(MockBody.Mun.GetPositionAtUT(25000.0), orbit.GetRelativePositionAtUT(25000.0));
            AssertEqual(MockBody.Mun.GetOrbitalVelocityAtUT(25000.0), orbit.GetOrbitalVelocityAtUT(25000.0));
        }

        [Fact]
        public void TestFromPositionDuna() {
            double UT = 20000.0;
            Vector3d position = MockBody.Duna.GetPositionAtUT(UT);
            Vector3d velocity = MockBody.Duna.GetOrbitalVelocityAtUT(UT);

            MockOrbit orbit = new MockOrbit(MockBody.Kerbol, position, velocity, UT);

            Assert.Equal(MockBody.Duna.orbit.inclination, orbit.inclination, 7);
            Assert.Equal(MockBody.Duna.orbit.eccentricity, orbit.eccentricity, 7);
            Assert.Equal(MockBody.Duna.orbit.semiMajorAxis, orbit.semiMajorAxis, 4);
            Assert.Equal(MockBody.Duna.orbit.LAN, orbit.LAN, 7);
            Assert.Equal(MockBody.Duna.orbit.argumentOfPeriapsis, orbit.argumentOfPeriapsis, 5);
            AssertEqual(MockBody.Duna.GetPositionAtUT(25000.0), orbit.GetRelativePositionAtUT(25000.0));
            AssertEqual(MockBody.Duna.GetOrbitalVelocityAtUT(25000.0), orbit.GetOrbitalVelocityAtUT(25000.0));
        }

        private void AssertEqual(Vector3d expected, Vector3d actual) {
            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
        }
    }
}
