using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPUI;
using UnityEngine;

namespace KontrolSystem.Plugin.UI.Adapter {
    public abstract class UIContainer<T> : KSPUIModule.IContainer<T> {
        private readonly List<IUIElement<T>> children = new List<IUIElement<T>>();
        protected readonly List<GUILayoutOption> options = new List<GUILayoutOption>();

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

        public KSPUIModule.IContainer<T> Width(double width) {
            options.Add(GUILayout.Width((float)width));
            return this;
        }

        public KSPUIModule.IContainer<T> MinWidth(double width) {
            options.Add(GUILayout.MinWidth((float)width));
            return this;
        }

        public KSPUIModule.IContainer<T> MaxWidth(double width) {
            options.Add(GUILayout.MaxWidth((float)width));
            return this;
        }

        public KSPUIModule.IContainer<T> ExpandWidth() {
            options.Add(GUILayout.ExpandWidth(true));
            return this;
        }

        public KSPUIModule.IContainer<T> Height(double height) {
            options.Add(GUILayout.Height((float)height));
            return this;
        }

        public KSPUIModule.IContainer<T> MinHeight(double height) {
            options.Add(GUILayout.MinHeight((float)height));
            return this;
        }

        public KSPUIModule.IContainer<T> MaxHeight(double height) {
            options.Add(GUILayout.MaxHeight((float)height));
            return this;
        }

        public KSPUIModule.IContainer<T> ExpandHeight() {
            options.Add(GUILayout.ExpandHeight(true));
            return this;
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
            GUILayout.BeginVertical(options.ToArray());

            var result = DrawChildren(state);

            GUILayout.EndVertical();

            return result;
        }
    }

    public class UIHorizontalContainer<T> : UIContainer<T>, IUIElement<T> {
        public (T state, bool changed) Draw(T state) {
            GUILayout.BeginHorizontal(options.ToArray());

            var result = DrawChildren(state);

            GUILayout.EndHorizontal();

            return result;
        }
    }

    public class UIWindow<T> : UIContainer<T>, KSPUIModule.IWindow<T> {
        private readonly GUIStyle style = new GUIStyle() {
            padding = new RectOffset(5, 5, 5, 5),
        };
        
        public UIWindow() => Title = "KontrolSystem";

        public string Title { get; set; }
        
        public (T state, bool changed) Draw(T state) {
            GUILayout.BeginVertical(style, options.ToArray());

            var result = DrawChildren(state);

            GUILayout.EndVertical();

            return result;
        }
    }
}
