using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Body",
            Description = "Represents an in-game celestrial body."
        )]
        public interface IBody {
            [KSField(Description = "Name of the celestrial body.")]
            string Name {
                get;
            }

            [KSField(Description = "Standard gravitation parameter of the body.")]
            double GravParameter {
                get;
            }

            [KSField("SOI_radius", Description = "Radius of the sphere of influence of the body")]
            double SOIRadius {
                get;
            }

            [KSField(Description = "The orbit of the celestrial body itself (around the parent body)")]
            IOrbit Orbit {
                get;
            }

            [KSField(Description = "`true` if the celestrial body has an atmosphere to deal with.")]
            bool HasAtmosphere {
                get;
            }

            [KSField(Description = "Depth/height of the atmosphere if present.")]
            double AtmosphereDepth {
                get;
            }

            [KSField]
            Vector3d Position { get; }

            [KSMethod(Description = "Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `UT`")]
            IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double UT);
        }
    }
}
