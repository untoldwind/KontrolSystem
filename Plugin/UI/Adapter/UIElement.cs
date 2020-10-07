using System;
using KontrolSystem.KSP.Runtime.KSPUI;
using UnityEngine;

namespace KontrolSystem.Plugin.UI.Adapter {
    public interface IUIElement<T> {
        (T state, bool changed) Draw(T state);
    }

    public class UILabel<T> : IUIElement<T>, KSPUIModule.ILabel {
        private readonly string label;

        public UILabel(string label) => this.label = label;

        public (T state, bool changed) Draw(T state) {
            GUILayout.Label(label);
            return (state, changed: false);
        }
    }

    public class UIButton<T> : IUIElement<T>, KSPUIModule.IButton {
        private readonly string label;
        private readonly Func<T, T> onClick;

        public UIButton(string label, Func<T, T> onClick) {
            this.label = label;
            this.onClick = onClick;
        }

        public (T state, bool changed) Draw(T state) {
            if (GUILayout.Button(label)) {
                return (state: onClick(state), changed: true);
            }

            return (state, changed: false);
        }
    }

    public class UITextField<T> : IUIElement<T>, KSPUIModule.ITextField {
        private readonly string value;
        private readonly Func<T, string, T> onUpdate;

        public UITextField(string value, Func<T, string, T> onUpdate) {
            this.value = value;
            this.onUpdate = onUpdate;
        }

        public (T state, bool changed) Draw(T state) {
            string newValue = GUILayout.TextField(value);

            if (!String.Equals(newValue, value)) {
                return (state: onUpdate(state, newValue), changed: true);
            }

            return (state, changed: false);
        }
    }
}
