namespace KontrolSystem.Plugin.Config {
    public class KontrolSystemParameters : GameParameters.CustomParameterNode {
        private static KontrolSystemParameters _instance;

        public static KontrolSystemParameters Instance {
            get {
                if (_instance == null) {
                    if (HighLogic.CurrentGame != null) {
                        _instance = HighLogic.CurrentGame.Parameters.CustomParams<KontrolSystemParameters>();
                    }
                }

                return _instance;
            }
        }

        [GameParameters.CustomParameterUI("Only use Blizzy toolbar",
            toolTip = "If you have the \"Blizzy Toolbar\" mod installed, only put the KontrolSystem\n" +
                      "button on it instead of both it and the stock toolbar.")]
        public bool useBlizzyToolbarOnly = false;

        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        public override bool HasPresets => false;

        public override string DisplaySection => "KontrolSystem";

        public override string Section => "KontrolSystem";

        public override int SectionOrder => 0;

        public override string Title => "CONFIG";

        public override void OnLoad(ConfigNode node) {
            base.OnLoad(node);
            _instance = null;
        }
    }
}
