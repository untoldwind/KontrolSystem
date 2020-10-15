using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("CrewMember")]
        public class CrewMemberAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly ProtoCrewMember crewMember;

            public CrewMemberAdapter(VesselAdapter vesselAdapter, ProtoCrewMember crewMember) {
                this.crewMember = crewMember;
                this.vesselAdapter = vesselAdapter;
            }

            [KSField] public string Name => crewMember.name;

            [KSField] public string Gender => crewMember.gender.ToString();

            [KSField] public long Experience => crewMember.experienceLevel;

            [KSField] public string Trait => crewMember.experienceTrait.Title;
            
            [KSField] public PartAdapter Part => new PartAdapter(vesselAdapter, crewMember.seat.part);
        }
    }
}
