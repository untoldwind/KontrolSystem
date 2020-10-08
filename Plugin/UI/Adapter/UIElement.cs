using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPUI;
using UnityEngine;

namespace KontrolSystem.Plugin.UI.Adapter {
    public interface IUIElement<T> {
        (T state, bool changed) Draw(T state);
    }

    public class UILabel<T> : IUIElement<T>, KSPUIModule.ILabel {
        private readonly string label;
        private readonly List<GUILayoutOption> options = new List<GUILayoutOption>();

        public UILabel(string label) => this.label = label;

        public (T state, bool changed) Draw(T state) {
            GUILayout.Label(label, options.ToArray());
            return (state, changed: false);
        }

        public KSPUIModule.ILabel Width(double width) {
            options.Add(GUILayout.Width((float) width));
            return this;
        }

        public KSPUIModule.ILabel MinWidth(double width) {
            options.Add(GUILayout.MinWidth((float) width));
            return this;
        }

        public KSPUIModule.ILabel MaxWidth(double width) {
            options.Add(GUILayout.MaxWidth((float) width));
            return this;
        }

        public KSPUIModule.ILabel ExpandWidth() {
            options.Add(GUILayout.ExpandWidth(true));
            return this;
        }

        public KSPUIModule.ILabel Height(double height) {
            options.Add(GUILayout.Height((float) height));
            return this;
        }

        public KSPUIModule.ILabel MinHeight(double height) {
            options.Add(GUILayout.MinHeight((float) height));
            return this;
        }

        public KSPUIModule.ILabel MaxHeight(double height) {
            options.Add(GUILayout.MaxHeight((float) height));
            return this;
        }

        public KSPUIModule.ILabel ExpandHeight() {
            options.Add(GUILayout.ExpandHeight(true));
            return this;
        }
    }

    public class UIButton<T> : IUIElement<T>, KSPUIModule.IButton {
        private readonly string label;
        private readonly Func<T, T> onClick;
        private readonly List<GUILayoutOption> options = new List<GUILayoutOption>();

        public UIButton(string label, Func<T, T> onClick) {
            this.label = label;
            this.onClick = onClick;
        }

        public (T state, bool changed) Draw(T state) {
            if (GUILayout.Button(label, options.ToArray())) {
                return (state: onClick(state), changed: true);
            }

            return (state, changed: false);
        }

        public KSPUIModule.IButton Width(double width) {
            options.Add(GUILayout.Width((float) width));
            return this;
        }

        public KSPUIModule.IButton MinWidth(double width) {
            options.Add(GUILayout.MinWidth((float) width));
            return this;
        }

        public KSPUIModule.IButton MaxWidth(double width) {
            options.Add(GUILayout.MaxWidth((float) width));
            return this;
        }

        public KSPUIModule.IButton ExpandWidth() {
            options.Add(GUILayout.ExpandWidth(true));
            return this;
        }

        public KSPUIModule.IButton Height(double height) {
            options.Add(GUILayout.Height((float) height));
            return this;
        }

        public KSPUIModule.IButton MinHeight(double height) {
            options.Add(GUILayout.MinHeight((float) height));
            return this;
        }

        public KSPUIModule.IButton MaxHeight(double height) {
            options.Add(GUILayout.MaxHeight((float) height));
            return this;
        }

        public KSPUIModule.IButton ExpandHeight() {
            options.Add(GUILayout.ExpandHeight(true));
            return this;
        }
    }

    public class UITextField<T> : IUIElement<T>, KSPUIModule.ITextField {
        private readonly string value;
        private readonly Func<T, string, T> onUpdate;
        private readonly List<GUILayoutOption> options = new List<GUILayoutOption>();
        private int maxLength;

        public UITextField(string value, Func<T, string, T> onUpdate) {
            this.value = value;
            this.onUpdate = onUpdate;
            maxLength = -1;
        }

        public (T state, bool changed) Draw(T state) {
            string newValue = GUILayout.TextField(value, maxLength, options.ToArray());

            if (!String.Equals(newValue, value)) {
                return (state: onUpdate(state, newValue), changed: true);
            }

            return (state, changed: false);
        }

        public KSPUIModule.ITextField MaxLength(long newMaxLength) {
            maxLength = (int) newMaxLength;
            return this;
        }

        public KSPUIModule.ITextField Width(double width) {
            options.Add(GUILayout.Width((float) width));
            return this;
        }

        public KSPUIModule.ITextField MinWidth(double width) {
            options.Add(GUILayout.MinWidth((float) width));
            return this;
        }

        public KSPUIModule.ITextField MaxWidth(double width) {
            options.Add(GUILayout.MaxWidth((float) width));
            return this;
        }

        public KSPUIModule.ITextField ExpandWidth() {
            options.Add(GUILayout.ExpandWidth(true));
            return this;
        }

        public KSPUIModule.ITextField Height(double height) {
            options.Add(GUILayout.Height((float) height));
            return this;
        }

        public KSPUIModule.ITextField MinHeight(double height) {
            options.Add(GUILayout.MinHeight((float) height));
            return this;
        }

        public KSPUIModule.ITextField MaxHeight(double height) {
            options.Add(GUILayout.MaxHeight((float) height));
            return this;
        }

        public KSPUIModule.ITextField ExpandHeight() {
            options.Add(GUILayout.ExpandHeight(true));
            return this;
        }
    }
}
