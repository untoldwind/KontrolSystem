using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("DockingNode")]
        public class ModuleDockingNodeAdapter : PartModuleAdapter, IKSPTargetable {
            public readonly ModuleDockingNode dockingNode;

            internal ModuleDockingNodeAdapter(VesselAdapter vesselAdapter, ModuleDockingNode dockingNode) :
                base(vesselAdapter, dockingNode) => this.dockingNode = dockingNode;

            [KSField] public string State => dockingNode.state;

            [KSField] public string NodeType => dockingNode.nodeType;

            [KSField] public Direction PortFacing => new Direction(dockingNode.nodeTransform.rotation);

            [KSField]
            public Vector3d NodePosition => dockingNode.nodeTransform.position -
                                            (FlightGlobals.ActiveVessel?.CoMD ?? vesselAdapter.vessel.CoMD);

            [KSMethod]
            public void ControlFrom() => dockingNode.MakeReferenceTransform();

            [KSMethod]
            public void Undock() {
                if (dockingNode.otherNode != null) {
                    // check to see if either the undock or decouple events are available
                    // and execute accordingly.
                    var evnt1 = dockingNode.Events["Undock"];
                    var evnt2 = dockingNode.Events["Decouple"];
                    if (evnt1 != null && evnt1.guiActive && evnt1.active) {
                        dockingNode.Undock();
                    } else if (evnt2 != null && evnt2.guiActive && evnt2.active) {
                        dockingNode.Decouple();
                    } else {
                        // If you can't do either event on this port, check to see if
                        // you can on the port it's docked too!
                        evnt1 = dockingNode.otherNode.Events["Undock"];
                        evnt2 = dockingNode.otherNode.Events["Decouple"];
                        if (evnt1 != null && evnt1.guiActive && evnt1.active) {
                            dockingNode.otherNode.Undock();
                        } else if (evnt2 != null && evnt2.guiActive && evnt2.active) {
                            dockingNode.otherNode.Decouple();
                        }
                    }
                }
            }

            [KSField] public bool Deployable => dockingNode.deployAnimator != null;

            [KSMethod]
            public void Deploy() {
                var m = dockingNode.deployAnimator;

                if (m != null && m.animSwitch) {
                    m.Toggle();
                }
            }

            [KSMethod]
            public void Undeploy() {
                var m = dockingNode.deployAnimator;

                if (m != null && !m.animSwitch) {
                    m.Toggle();
                }
            }

            public string Name => dockingNode.name;

            public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(dockingNode.GetOrbit());

            public Option<KSPOrbitModule.IBody> AsBody => new Option<KSPOrbitModule.IBody>();

            public Option<VesselAdapter> AsVessel => new Option<VesselAdapter>();

            public Option<ModuleDockingNodeAdapter> AsDockingPort => new Option<ModuleDockingNodeAdapter>(this);

            public ITargetable Underlying => dockingNode;
        }
    }
}
