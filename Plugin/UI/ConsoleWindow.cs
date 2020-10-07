using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public class ConsoleWindow : ResizableWindow {
        private static readonly Color Color = new Color(1f, 1f, 1f, 1.1f); // opaque window color when focused
        private static readonly Color BgColor = new Color(0.0f, 0.0f, 0.0f, 1.0f); // black background of terminal
        private static readonly Color TextColor = new Color(0.5f, 1.0f, 0.5f, 1.0f); // font color on terminal

        private GUIStyle terminalImageStyle;
        private GUIStyle terminalFrameStyle;
        private GUIStyle terminalFrameActiveStyle;
        private GUISkin terminalLetterSkin;
        private KSPConsoleBuffer consoleBuffer;
        private int fontCharWidth;
        private int fontCharHeight;

        public void Toggle() {
            if (!isOpen) Open();
            else Close();
        }

        public void AttachTo(KSPConsoleBuffer consoleBuffer) {
            this.consoleBuffer = consoleBuffer;
            if (this.consoleBuffer == null) return;

            windowRect = new Rect(windowRect.xMin, windowRect.yMin, this.consoleBuffer.VisibleCols * fontCharWidth + 65,
                this.consoleBuffer.VisibleRows * fontCharHeight + 108);
        }

        // --------------------- MonoBehaviour callbacks ------------------------

        public void Awake() {
            Initialize("KontrolSystem: Console", new Rect(60, 60, 100, 100), true);

            Texture2D terminalImage = GameDatabase.Instance.GetTexture("KontrolSystem/GFX/dds_monitor_minimal", false);
            Texture2D terminalFrameImage =
                GameDatabase.Instance.GetTexture("KontrolSystem/GFX/dds_monitor_minimal_frame", false);
            Texture2D terminalFrameActiveImage =
                GameDatabase.Instance.GetTexture("KontrolSystem/GFX/dds_monitor_minimal_frame_active", false);

            terminalImageStyle = Create9SliceStyle(terminalImage);
            terminalFrameStyle = Create9SliceStyle(terminalFrameImage);
            terminalFrameActiveStyle = Create9SliceStyle(terminalFrameActiveImage);

            terminalLetterSkin = BuildPanelSkin();
            terminalLetterSkin.label.fontSize = 12;
            terminalLetterSkin.label.font =
                FontManager.Instance.GetSystemFontByNameAndSize(FontManager.DefaultConsoleFonts,
                    terminalLetterSkin.label.fontSize, true);

            CharacterInfo chInfo;
            terminalLetterSkin.label.font
                .RequestCharactersInTexture("X"); // Make sure the char in the font is lazy-loaded by Unity.
            terminalLetterSkin.label.font.GetCharacterInfo('X', out chInfo);
            fontCharWidth = chInfo.advance;
            fontCharHeight = terminalLetterSkin.label.fontSize;
        }

        protected override void DrawWindow(int windowId) {
            GUI.color = Color;

            if (GUI.Button(new Rect(windowRect.width - 75, windowRect.height - 30, 50, 25), "Close")) Close();

            GUI.Label(new Rect(15, 28, windowRect.width - 30, windowRect.height - 63), "", terminalImageStyle);

            GUI.BeginGroup(new Rect(28, 48, (consoleBuffer?.VisibleCols ?? 1) * fontCharWidth + 2,
                (consoleBuffer?.VisibleRows ?? 1) * fontCharHeight +
                4)); // +4 so descenders and underscores visible on bottom row.

            List<ConsoleLine> visibleLines = consoleBuffer?.VisibleLines ?? new List<ConsoleLine>();

            terminalLetterSkin.label.normal.textColor = TextColor;

            for (int row = 0; row < visibleLines.Count; row++) {
                string lineString = visibleLines[row].ToString().Replace('\0', ' ');

                GUI.Label(new Rect(0, (row * fontCharHeight), windowRect.width - 10, fontCharHeight), lineString,
                    terminalLetterSkin.label);
            }

            GUI.EndGroup();

            GUI.Label(new Rect(15, 28, windowRect.width - 30, windowRect.height - 63), "", terminalFrameStyle);

            GUI.Label(new Rect(windowRect.width / 2 - 40, windowRect.height - 16, 100, 10),
                (consoleBuffer?.VisibleCols ?? 1) + "x" + (consoleBuffer?.VisibleRows ?? 1));
        }

        protected override void OnResize(Rect windowRect) {
            consoleBuffer?.Resize((int) ((windowRect.height - 108) / fontCharHeight),
                (int) ((windowRect.width - 65) / fontCharWidth));
        }


        private Color AdjustColor(Color baseColor, double brightness) {
            Color newColor = baseColor;
            newColor.a = Convert.ToSingle(brightness); // represent dimness by making it fade into the backround.
            return newColor;
        }

        /// <summary>
        /// Unity lacks gui styles for GUI.DrawTexture(), so to make it do
        /// 9-slice stretching, we have to draw the 9slice image as a GUI.Label.
        /// But GUI.Labels that render a Texture2D instead of text, won't stretch
        /// larger than the size of the image file no matter what you do (only smaller).
        /// So to make it stretch the image in a label, the image has to be implemented
        /// as part of the label's background defined in the GUIStyle instead of as a
        /// normal image element.  This sets up that style, which you can then render
        /// by making a GUILabel use this style and have dummy empty string content.
        /// </summary>
        /// <returns>The slice style.</returns>
        /// <param name="fromTexture">From texture.</param>
        private GUIStyle Create9SliceStyle(Texture2D fromTexture) {
            GUIStyle style = new GUIStyle();
            style.normal.background = fromTexture;
            style.border = new RectOffset(20, 20, 20, 20);
            return style;
        }

        private static GUISkin BuildPanelSkin() {
            GUISkin theSkin = Instantiate(HighLogic.Skin); // Use Instantiate to make a copy of the Skin Object

            theSkin.label.fontSize = 10;
            theSkin.label.normal.textColor = Color.white;
            theSkin.label.padding = new RectOffset(0, 0, 0, 0);
            theSkin.label.margin = new RectOffset(1, 1, 1, 1);

            theSkin.button.fontSize = 10;
            theSkin.button.padding = new RectOffset(0, 0, 0, 0);
            theSkin.button.margin = new RectOffset(0, 0, 0, 0);

            return theSkin;
        }
    }
}
