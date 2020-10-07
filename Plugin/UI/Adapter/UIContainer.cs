using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPUI;
using UnityEngine;

namespace KontrolSystem.Plugin.UI.Adapter {
    public abstract class UIContainer<T> : KSPUIModule.IContainer<T> {
        private List<IUIElement<T>> children = new List<IUIElement<T>>();

        public KSPUIModule.ILabel Label(string label) {
            var child = new UILabel<T>(label);
            children.Add(child);
            return child;
        }

        public KSPUIModule.IButton Button(string label, Func<T, T> onClick) {
            var child = new UIButton<T>(label, onClick);
            children.Add(child);
            return child;
        }

        public KSPUIModule.ITextField TextField(string value, Func<T, string, T> onUpdate) {
            var child = new UITextField<T>(value, onUpdate);
            children.Add(child);
            return child;
        }

        public KSPUIModule.IContainer<T> VerticalLayout() {
            var child = new UIVerticalContainer<T>();
            children.Add(child);
            return child;
        }

        public KSPUIModule.IContainer<T> HorizontalLayout() {
            var child = new UIHorizontalContainer<T>();
            children.Add(child);
            return child;
        }

        protected (T state, bool changed) DrawChildren(T state) {
            var changed = false;

            foreach (var child in children) {
                var result = child.Draw(state);
                state = result.state;
                changed = changed || result.changed;
            }

            return (state, changed);
        }
    }

    public class UIVerticalContainer<T> : UIContainer<T>, IUIElement<T> {
        public (T state, bool changed) Draw(T state) {
            GUILayout.BeginVertical();

            var result = DrawChildren(state);

            GUILayout.EndVertical();

            return result;
        }
    }

    public class UIHorizontalContainer<T> : UIContainer<T>, IUIElement<T> {
        public (T state, bool changed) Draw(T state) {
            GUILayout.BeginHorizontal();

            var result = DrawChildren(state);

            GUILayout.EndHorizontal();

            return result;
        }
    }

    public class UIWindow<T> : UIVerticalContainer<T>, KSPUIModule.IWindow<T> {
        public UIWindow() => Title = "KontrolSystem";

        public string Title { get; set; }
    }
}
