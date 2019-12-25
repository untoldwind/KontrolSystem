namespace KontrolSystem.TO2.AST {
    public enum IndexSpecType {
        Single
    }

    public class IndexSpec {
        public readonly IndexSpecType indexType;
        public readonly Expression start;

        public IndexSpec(Expression index) {
            indexType = IndexSpecType.Single;
            start = index;
        }

        public void SetVariableContainer(IVariableContainer container) => start.SetVariableContainer(container);
    }
}
