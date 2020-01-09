using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class GenericParameter : RealizedType {
        private readonly string name;

        public GenericParameter(string _name) => name = _name;

        public override string Name => name;

        public override Type GeneratedType(ModuleContext context) => throw new NotImplementedException();

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override RealizedType FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (!typeArguments.ContainsKey(name)) throw new ArgumentException($"Generic parameter {name} not found");
            return typeArguments[name];
        }

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context, RealizedType concreteType) => (name, concreteType).Yield();
    }
}
