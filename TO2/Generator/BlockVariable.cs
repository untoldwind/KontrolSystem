using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator {
    public delegate IBlockVariable VariableResolver(string name);

    public interface IBlockVariable {
        string Name {
            get;
        }
        RealizedType Type {
            get;
        }

        bool IsConst {
            get;
        }

        void EmitLoad(IBlockContext context);

        void EmitLoadPtr(IBlockContext context);

        void EmitStore(IBlockContext context);
    }

    internal class MethodParameter : IBlockVariable {
        private readonly string name;
        private readonly RealizedType type;
        private readonly int index;

        public string Name => name;

        public RealizedType Type => type;

        public bool IsConst => false;

        public MethodParameter(string _name, RealizedType _type, int _index) {
            name = _name;
            type = _type;
            index = _index;
        }

        public void EmitLoad(IBlockContext context) => EmitLoadArg(context.IL, index);

        public void EmitLoadPtr(IBlockContext context) {
            if (index < 256) context.IL.Emit(OpCodes.Ldarga_S, (byte)index);
            else context.IL.Emit(OpCodes.Ldarga, (short)index);
        }

        public void EmitStore(IBlockContext context) {
            if (index < 256) {
                context.IL.Emit(OpCodes.Starg_S, (byte)index);
            } else {
                context.IL.Emit(OpCodes.Starg, (short)index);
            }
        }

        public static void EmitLoadArg(IILEmitter IL, int index) {
            switch (index) {
            case 0:
                IL.Emit(OpCodes.Ldarg_0);
                return;
            case 1:
                IL.Emit(OpCodes.Ldarg_1);
                return;
            case 2:
                IL.Emit(OpCodes.Ldarg_2);
                return;
            case 3:
                IL.Emit(OpCodes.Ldarg_3);
                return;
            case int n when (n < 256):
                IL.Emit(OpCodes.Ldarg_S, (byte)index);
                return;
            default:
                IL.Emit(OpCodes.Ldarg, (short)index);
                return;
            }
        }
    }

    internal class DeclaredVariable : IBlockVariable {
        private readonly string name;
        private readonly bool isConst;
        private readonly RealizedType type;
        private readonly ILocalRef localRef;

        public DeclaredVariable(string _name, bool _isConst, RealizedType _type, ILocalRef _localRef) {
            name = _name;
            isConst = _isConst;
            type = _type;
            localRef = _localRef;
        }

        public string Name => name;

        public RealizedType Type => type;

        public bool IsConst => isConst;

        public void EmitLoad(IBlockContext context) => localRef.EmitLoad(context);

        public void EmitLoadPtr(IBlockContext context) => localRef.EmitLoadPtr(context);

        public void EmitStore(IBlockContext context) => localRef.EmitStore(context);
    }

    public class TempVariable : IBlockVariable {
        private readonly RealizedType type;
        private readonly ILocalRef localRef;

        public TempVariable(RealizedType _type, ILocalRef _localRef) {
            type = _type;
            localRef = _localRef;
        }

        public string Name => "***temp***";

        public RealizedType Type => type;

        public bool IsConst => false;

        public void EmitLoad(IBlockContext context) => localRef.EmitLoad(context);

        public void EmitLoadPtr(IBlockContext context) => localRef.EmitLoadPtr(context);

        public void EmitStore(IBlockContext context) => localRef.EmitStore(context);
    }

    public class ClonedFieldVariable : IBlockVariable {
        private readonly RealizedType type;
        public readonly FieldInfo valueField;

        public ClonedFieldVariable(RealizedType _type, FieldInfo _valueField) {
            type = _type;
            valueField = _valueField;
        }

        public string Name => valueField.Name;

        public RealizedType Type => type;

        public bool IsConst => true;

        public void EmitLoad(IBlockContext context) {
            context.IL.Emit(OpCodes.Ldarg_0);
            context.IL.Emit(OpCodes.Ldfld, valueField);
        }

        public void EmitLoadPtr(IBlockContext context) {
            context.IL.Emit(OpCodes.Ldarg_0);
            context.IL.Emit(OpCodes.Ldflda, valueField);
        }

        public void EmitStore(IBlockContext context) { }
    }
}
