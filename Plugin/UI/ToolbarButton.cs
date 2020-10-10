using System;
using UnityEngine;
using KSP.UI.Screens;
using KramaxReloadExtensions;
using KontrolSystem.Plugin.Config;
using KontrolSystem.Plugin.Core;

namespace KontrolSystem.Plugin.UI {
    /// <summary>
    /// The main entry point of the plugin.
    /// Registers a button for the toolbar with an attached popup window.
    /// </summary>
    public abstract class ToolbarButton : ReloadableMonoBehaviour {
        private static ToolbarButton _instance;
        private ApplicationLauncherButton launcherButton;
        private IButton blizzyButton;

        private Texture2D launcherButtonTexture;

        private bool clickedOn;
        private bool hoverStay;
        private bool isOpen;

        private CommonStyles commonStyles;
        private ToolbarWindow toolbarWindow;
        private ConsoleWindow consoleWindow;
        private ModuleManagerWindow moduleManagerWindow;
        private VolumeInspectWindow volumeInspect;

        public static ToolbarButton Instance => _instance;

        public VolumeInspectWindow VolumeInspect => volumeInspect;

        // --------------------- MonoBehaviour callbacks ------------------------

        public void Awake() {
            _instance = this;
            PluginLogger.Instance.Debug("Awake ToolbarButton");

            consoleWindow = AddComponent(typeof(ConsoleWindow)) as ConsoleWindow;
            moduleManagerWindow = AddComponent(typeof(ModuleManagerWindow)) as ModuleManagerWindow;
            volumeInspect = AddComponent(typeof(VolumeInspectWindow)) as VolumeInspectWindow;
        }

        public void Start() {
            switch (HighLogic.LoadedScene) {
                case GameScenes.SPACECENTER:
                case GameScenes.TRACKSTATION:
                case GameScenes.FLIGHT:
                case GameScenes.EDITOR:
                    break;
                default:
                    return;
            }
            PluginLogger.Instance.Info("Starting!!!!!! " + HighLogic.LoadedScene);
            
            launcherButtonTexture = GameDatabase.Instance.GetTexture("KontrolSystem/GFX/dds_launcher_button", false);

            ApplicationLauncher launcher = ApplicationLauncher.Instance;

            if (launcher == null) {
                PluginLogger.Instance.Error("Launcher not ready on start");
                return;
            }

            var useBlizzyOnly = ToolbarManager.ToolbarAvailable &&
                                KontrolSystemParameters.Instance != null &&
                                KontrolSystemParameters.Instance.useBlizzyToolbarOnly;

            if (!useBlizzyOnly && launcherButton == null) {
                launcherButton = launcher.AddModApplication(
                    CallbackOnTrue,
                    CallbackOnFalse,
                    CallbackOnHover,
                    CallbackOnHoverOut,
                    CallbackOnEnable,
                    CallbackOnDisable,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    launcherButtonTexture);

                launcher.AddOnShowCallback(CallbackOnShow);
                launcher.AddOnHideCallback(CallbackOnHide);
                launcher.EnableMutuallyExclusive(launcherButton);
            }

            if (blizzyButton == null && ToolbarManager.ToolbarAvailable) {
                blizzyButton = ToolbarManager.Instance.Add("KontrolSystem", "ksButton");
                blizzyButton.TexturePath = "KontrolSystem/GFX/dds_launcher_button-blizzy";
                blizzyButton.ToolTip = "KontrolSystem";
                blizzyButton.OnClick += CallbackOnClickBlizzy;
            }

            commonStyles ??= new CommonStyles(Instantiate(HighLogic.Skin));

            toolbarWindow ??= new ToolbarWindow(GetInstanceID(), commonStyles, consoleWindow, moduleManagerWindow);

            Mainframe.Instance.Reboot(KontrolSystemConfig.Instance);

            PluginLogger.Instance.Info("Start success");
        }

        public void OnDestroy() {
            try {
                ApplicationLauncher launcher = ApplicationLauncher.Instance;
                if (launcherButton != null && launcher != null) {
                    launcher.DisableMutuallyExclusive(launcherButton);
                    launcher.RemoveOnRepositionCallback(CallbackOnShow);
                    launcher.RemoveOnHideCallback(CallbackOnHide);
                    launcher.RemoveOnShowCallback(CallbackOnShow);
                    launcher.RemoveModApplication(launcherButton);
                    launcherButton = null;
                }
            } catch (Exception e) {
                PluginLogger.Instance.Error("Failed unregistering AppLauncher handlers," + e.Message);
            }

            PluginLogger.Instance.Info("Destroy");
            _instance = null;
        }

        public void OnGUI() {
            if (!isOpen && !hoverStay) return;
            
            toolbarWindow?.DrawUI();

            if (hoverStay) {
                Vector3 mousePos = Input.mousePosition;
                hoverStay = toolbarWindow?.WindowRect.Contains(new Vector2(mousePos.x, Screen.height - mousePos.y)) ??
                            false;
            }
        }

        // ------------------------ Launcher button callbacks --------------------------------

        /// <summary>Callback for when the button is toggled on</summary>
        void CallbackOnTrue() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnTrue()");
            clickedOn = true;
            Open();
        }

        /// <summary>Callback for when the button is toggled off</summary>
        void CallbackOnFalse() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnFalse()");
            clickedOn = false;
            Close();
        }

        /// <summary>Callback for when the mouse is hovering over the button</summary>
        void CallbackOnHover() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnHover()");
            if (!clickedOn) {
                Open();
            }
        }

        /// <summary>Callback for when the mouse is hover is off the button</summary>
        void CallbackOnHoverOut() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnHoverOut()");
            if (!clickedOn) {
                Close();
                Vector3 mousePos = Input.mousePosition;
                hoverStay = toolbarWindow?.WindowRect.Contains(new Vector2(mousePos.x, Screen.height - mousePos.y)) ??
                            false;
            }
        }

        /// <summary>Callback for when the application launcher shows itself</summary>
        void CallbackOnShow() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnShow()");
            if (clickedOn)
                Open();
        }

        /// <summary>Callback for when the application launcher hides itself</summary>
        void CallbackOnHide() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnHide()");
            Close();
        }

        void CallbackOnEnable() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnEnable()");
        }

        /// <summary>Callback for when the button is hidden or disabled by the application launcher</summary>
        void CallbackOnDisable() {
            PluginLogger.Instance.Debug("KontrolSystem: PROOF: CallbackOnDisable()");
        }

        void CallbackOnClickBlizzy(ClickEvent e) {
            if (!isOpen)
                Open();
            else
                Close();
        }

        void Open() {
            if (launcherButton == null) {
                toolbarWindow?.SetPosition(ApplicationLauncher.Instance.IsPositionedAtTop);
            } else {
                toolbarWindow?.SetPosition(ApplicationLauncher.Instance.IsPositionedAtTop,
                    launcherButton.GetAnchorUL());
            }

            isOpen = true;
            hoverStay = false;
        }

        void Close() {
            isOpen = false;
        }
    }

    [KSPAddon(KSPAddon.Startup.FlightEditorAndKSC, false)]
    public class FlightEditorAndKSCToolbarButton : ToolbarButton {
        
    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class TrackingStationToolbarButton : ToolbarButton {
        
    }
}
