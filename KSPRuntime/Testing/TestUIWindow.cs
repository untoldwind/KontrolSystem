using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPUI;

namespace KontrolSystem.KSP.Runtime.Testing {
    public class TestUIWindow<T> : KSPUIModule.IWindow<T> {
        private bool closed;
        private Func<T, bool> isEndState;
        private Action<KSPUIModule.IContainer<T>, T> render;
        private TestUIContainer<T> root;
        
        public T State { get; set; }

        public TestUIWindow(T initialState, Func<T, bool> isEndState, Action<KSPUIModule.IContainer<T>, T> render) {
            State = initialState;
            this.isEndState = isEndState;
            this.render = render;
            closed = isEndState(initialState);
            root = new TestUIContainer<T>();
            render(root, initialState);
        }
        
        public bool Closed => closed;

        public void Close() => closed = true;
    }

    public interface ITestUIElement {
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

        public KSPUIModule.IInput Input(string value, Func<T, string, T> onUpdate) {
            var element = new TestUIInput<T>(value, onUpdate);
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

        public E GetChild<E>(int idx) where E: ITestUIElement {
            return (E)children[idx];
        }
    }

    public class TestUILabel : KSPUIModule.ILabel, ITestUIElement {
        public string Label { get; }

        public TestUILabel(string label) {
            Label = label;
        }
    }

    public class TestUIButton<T> : KSPUIModule.IButton, ITestUIElement {
        private Func<T, T> onClick;
        
        public string Label { get; }

        public TestUIButton(string label, Func<T, T> onClick) {
            Label = label;
            this.onClick = onClick;
        }
    }

    public class TestUIInput<T> : KSPUIModule.IInput, ITestUIElement {
        private Func<T, string, T> onUpdate;
        
        public string Value { get; }

        public TestUIInput(string value, Func<T, string, T> onUpdate) {
            Value = value;
            this.onUpdate = onUpdate;
        }
    }
}
