namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public class SimCurves {
        public FloatCurve LiftCurve { get; }
        public FloatCurve LiftMachCurve { get; }
        public FloatCurve DragCurve { get; }
        public FloatCurve DragMachCurve { get; }
        public FloatCurve DragCurveTail { get; }
        public FloatCurve DragCurveSurface { get; }
        public FloatCurve DragCurveTip { get; }
        public FloatCurve DragCurveCd { get; }
        public FloatCurve DragCurveCdPower { get; }
        public FloatCurve DragCurveMultiplier { get; }
        public FloatCurve AtmospherePressureCurve { get; }
        public FloatCurve AtmosphereTemperatureSunMultCurve { get; }
        public FloatCurve LatitudeTemperatureBiasCurve { get; }
        public FloatCurve LatitudeTemperatureSunMultCurve { get; }
        public FloatCurve AxialTemperatureSunMultCurve { get; }
        public FloatCurve AtmosphereTemperatureCurve { get; }
        public FloatCurve DragCurvePseudoReynolds { get; }
        public double SpaceTemperature { get; }

        public SimCurves(CelestialBody body) {
            DragCurveCd = new FloatCurve(PhysicsGlobals.DragCurveCd.Curve.keys);
            DragCurveCdPower = new FloatCurve(PhysicsGlobals.DragCurveCdPower.Curve.keys);
            DragCurveMultiplier = new FloatCurve(PhysicsGlobals.DragCurveMultiplier.Curve.keys);

            DragCurveSurface = new FloatCurve(PhysicsGlobals.SurfaceCurves.dragCurveSurface.Curve.keys);
            DragCurveTail = new FloatCurve(PhysicsGlobals.SurfaceCurves.dragCurveTail.Curve.keys);
            DragCurveTip = new FloatCurve(PhysicsGlobals.SurfaceCurves.dragCurveTip.Curve.keys);

            LiftCurve = new FloatCurve(PhysicsGlobals.BodyLiftCurve.liftCurve.Curve.keys);
            LiftMachCurve = new FloatCurve(PhysicsGlobals.BodyLiftCurve.liftMachCurve.Curve.keys);
            DragCurve = new FloatCurve(PhysicsGlobals.BodyLiftCurve.dragCurve.Curve.keys);
            DragMachCurve = new FloatCurve(PhysicsGlobals.BodyLiftCurve.dragMachCurve.Curve.keys);

            DragCurvePseudoReynolds = new FloatCurve(PhysicsGlobals.DragCurvePseudoReynolds.Curve.keys);

            SpaceTemperature = PhysicsGlobals.SpaceTemperature;

            AtmospherePressureCurve = new FloatCurve(body.atmospherePressureCurve.Curve.keys);
            AtmosphereTemperatureSunMultCurve = new FloatCurve(body.atmosphereTemperatureSunMultCurve.Curve.keys);
            LatitudeTemperatureBiasCurve = new FloatCurve(body.latitudeTemperatureBiasCurve.Curve.keys);
            LatitudeTemperatureSunMultCurve = new FloatCurve(body.latitudeTemperatureSunMultCurve.Curve.keys);
            AtmosphereTemperatureCurve = new FloatCurve(body.atmosphereTemperatureCurve.Curve.keys);
            AxialTemperatureSunMultCurve = new FloatCurve(body.axialTemperatureSunMultCurve.Curve.keys);
        }
    }
}
