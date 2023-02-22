using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPUI;

namespace KontrolSystem.KSP.Runtime.Testing {
    public class TestUIWindowHandle<T> : KSPUIModule.IWindowHandle<T> {
        private bool closed;
        private readonly Func<T, bool> isEndState;
        private Action<KSPUIModule.IWindow<T>, T> render;
        private readonly TestUIWindow<T> window;

        public T State { get; set; }

        public TestUIWindowHandle(T initialState, Func<T, bool> isEndState, Action<KSPUIModule.IWindow<T>, T> render) {
            State = initialState;
            this.isEndState = isEndState;
            this.render = render;
            closed = isEndState(initialState);
            window = new TestUIWindow<T>();
            render(window, initialState);
        }

        public bool Closed => closed;

        public void Close() => closed = true;

        public void SimulateClick(long[] path) {
            ITestUIElement element = window;
            foreach (var idx in path) {
                element = element.GetChild((int)idx);
                if (element == null) return;
            }

            switch (element) {
            case TestUIButton<T> button:
                State = button.Click(State);
                break;
            }

            closed = isEndState(State);
        }
    }

    public interface ITestUIElement {
        public ITestUIElement GetChild(int idx);
    }

    public class TestUIContainer<T> : KSPUIModule.IContainer<T>, ITestUIElement {
        private readonly List<ITestUIElement> children = new List<ITestUIElement>();

        public KSPUIModule.ILabel Label(string label) {
            var element = new TestUILabel(label);
            children.Add(element);
            return element;
        }

        public KSPUIModule.IButton Button(string label, Func<T, T> onClick) {
            var element = new TestUIButton<T>(label, onClick);
            children.Add(element);
            return element;
        }

        public KSPUIModule.ITextField TextField(string value, Func<T, string, T> onUpdate) {
            var element = new TestUITextField<T>(value, onUpdate);
            children.Add(element);
            return element;
        }

        public KSPUIModule.IContainer<T> VerticalLayout() {
            var container = new TestUIContainer<T>();
            children.Add(container);
            return container;
        }

        public KSPUIModule.IContainer<T> HorizontalLayout() {
            var container = new TestUIContainer<T>();
            children.Add(container);
            return container;
        }

        public ITestUIElement GetChild(int idx) => idx >= 0 && idx < children.Count ? children[idx] : null;

        public KSPUIModule.IContainer<T> Width(double width) => this;

        public KSPUIModule.IContainer<T> MinWidth(double width) => this;

        public KSPUIModule.IContainer<T> MaxWidth(double width) => this;

        public KSPUIModule.IContainer<T> ExpandWidth() => this;

        public KSPUIModule.IContainer<T> Height(double height) => this;

        public KSPUIModule.IContainer<T> MinHeight(double height) => this;

        public KSPUIModule.IContainer<T> MaxHeight(double height) => this;

        public KSPUIModule.IContainer<T> ExpandHeight() => this;
    }

    public class TestUIWindow<T> : TestUIContainer<T>, KSPUIModule.IWindow<T> {
        public string Title { get; set; }
    }

    public class TestUILabel : KSPUIModule.ILabel, ITestUIElement {
        public string Label { get; }

        public TestUILabel(string label) {
            Label = label;
        }

        public ITestUIElement GetChild(int idx) => null;

        public KSPUIModule.ILabel Width(double width) => this;

        public KSPUIModule.ILabel MinWidth(double width) => this;

        public KSPUIModule.ILabel MaxWidth(double width) => this;

        public KSPUIModule.ILabel ExpandWidth() => this;

        public KSPUIModule.ILabel Height(double height) => this;

        public KSPUIModule.ILabel MinHeight(double height) => this;

        public KSPUIModule.ILabel MaxHeight(double height) => this;

        public KSPUIModule.ILabel ExpandHeight() => this;
    }

    public class TestUIButton<T> : KSPUIModule.IButton, ITestUIElement {
        private Func<T, T> onClick;

        public string Label { get; }

        public TestUIButton(string label, Func<T, T> onClick) {
            Label = label;
            this.onClick = onClick;
        }

        public ITestUIElement GetChild(int idx) => null;

        public T Click(T state) => onClick(state);

        public KSPUIModule.IButton Width(double width) => this;

        public KSPUIModule.IButton MinWidth(double width) => this;

        public KSPUIModule.IButton MaxWidth(double width) => this;

        public KSPUIModule.IButton ExpandWidth() => this;

        public KSPUIModule.IButton Height(double height) => this;

        public KSPUIModule.IButton MinHeight(double height) => this;

        public KSPUIModule.IButton MaxHeight(double height) => this;

        public KSPUIModule.IButton ExpandHeight() => this;
    }

    public class TestUITextField<T> : KSPUIModule.ITextField, ITestUIElement {
        private Func<T, string, T> onUpdate;

        public string Value { get; }

        public TestUITextField(string value, Func<T, string, T> onUpdate) {
            Value = value;
            this.onUpdate = onUpdate;
        }

        public ITestUIElement GetChild(int idx) => null;

        public KSPUIModule.ITextField MaxLength(long maxLength) => this;

        public KSPUIModule.ITextField Width(double width) => this;

        public KSPUIModule.ITextField MinWidth(double width) => this;

        public KSPUIModule.ITextField MaxWidth(double width) => this;

        public KSPUIModule.ITextField ExpandWidth() => this;

        public KSPUIModule.ITextField Height(double height) => this;

        public KSPUIModule.ITextField MinHeight(double height) => this;

        public KSPUIModule.ITextField MaxHeight(double height) => this;

        public KSPUIModule.ITextField ExpandHeight() => this;
    }
}
