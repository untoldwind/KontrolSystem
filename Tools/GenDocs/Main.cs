using System;
using System.IO;
using System.Text;
using System.Linq;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.KSP.Runtime;

namespace KontrolSystem.GenDocs {
    class MainClass {
        public static void Main(string[] args) {
            var registry = KontrolSystemKSPRegistry.CreateKSP();

            registry.AddDirectory(Path.Combine(Directory.GetCurrentDirectory(), "GameData", "KontrolSystem", "to2"));

            GenerateDocs(registry);
        }

        public static void GenerateDocs(KontrolRegistry registry) {
            foreach (IKontrolModule module in registry.modules.Values) {
                if (IsModuleEmpty(module)) continue;
                using (StreamWriter fs = File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), "docs", "content", "reference", module.Name.Replace("::", "_") + ".md"))) {
                    GenerateDocs(module, fs);
                    Console.Out.WriteLine($"Generated: {module.Name}");
                }
            }
        }

        public static bool IsModuleEmpty(IKontrolModule module) => !module.AllConstantNames.Any() && !module.AllFunctionNames.Any() && !module.AllTypeNames.Any();

        public static void GenerateDocs(IKontrolModule module, TextWriter output) {
            output.WriteLine("---");
            output.WriteLine($"title: \"{module.Name}\"");
            output.WriteLine("---");
            output.WriteLine();
            output.WriteLine(module.Description);

            if (module.AllTypeNames.Any()) {
                output.WriteLine();
                output.WriteLine("# Types");
                output.WriteLine();

                foreach (string typeName in module.AllTypeNames.OrderBy(name => name)) {
                    RealizedType type = module.FindType(typeName);

                    output.WriteLine();
                    output.WriteLine($"## {typeName}");
                    output.WriteLine();
                    output.WriteLine(type.Description);

                    if (type.DeclaredFields.Count > 0) {
                        output.WriteLine();
                        output.WriteLine("### Fields");
                        output.WriteLine();

                        output.WriteLine("Name | Type | Description");
                        output.WriteLine("--- | --- | ---");

                        foreach (var kv in type.DeclaredFields.OrderBy(kv => kv.Key)) {
                            output.WriteLine($"{kv.Key} | {kv.Value.DeclaredType} | {kv.Value.Description?.Replace("\n", " ")}");
                        }
                    }
                    if (type.DeclaredMethods.Count > 0) {
                        output.WriteLine();
                        output.WriteLine("### Methods");

                        foreach (var kv in type.DeclaredMethods.OrderBy(kv => kv.Key)) {
                            output.WriteLine();
                            output.WriteLine($"#### {kv.Key}");
                            output.WriteLine();
                            output.WriteLine("```rust");
                            output.WriteLine(MethodSignature(type.LocalName, kv.Key, kv.Value));
                            output.WriteLine("```");
                            output.WriteLine();
                            output.WriteLine(kv.Value.Description);
                        }
                    }
                }
            }

            if (module.AllConstantNames.Any()) {
                output.WriteLine();
                output.WriteLine("# Constants");
                output.WriteLine();

                output.WriteLine("Name | Type | Description");
                output.WriteLine("--- | --- | ---");
                foreach (string constantName in module.AllConstantNames.OrderBy(name => name)) {
                    IKontrolConstant constant = module.FindConstant(constantName);

                    output.WriteLine($"{constantName} | {constant.Type} | {constant.Description}");
                }

                output.WriteLine();
            }

            if (module.AllFunctionNames.Any()) {
                output.WriteLine();
                output.WriteLine("# Functions");
                output.WriteLine();

                foreach (string functionName in module.AllFunctionNames.OrderBy(name => name)) {
                    IKontrolFunction function = module.FindFunction(functionName);

                    output.WriteLine();
                    output.WriteLine($"## {functionName}");
                    output.WriteLine();
                    output.WriteLine("```rust");
                    output.WriteLine(FunctionSignature(function));
                    output.WriteLine("```");
                    output.WriteLine();
                    output.WriteLine(function.Description);
                }
            }
        }

        public static string FunctionSignature(IKontrolFunction function) {
            StringBuilder sb = new StringBuilder();

            sb.Append("pub ");
            if (!function.IsAsync) sb.Append("sync ");
            sb.Append("fn ");
            sb.Append(function.Name);

            if (function.Parameters.Count == 0) {
                sb.Append(" (");
            } else {
                sb.Append(" ( ");

                string offset = new String(' ', sb.Length);

                sb.Append(FunctionParameterSignature(function.Parameters.First()));
                foreach (RealizedParameter parameter in function.Parameters.Skip(1)) {
                    sb.Append(",\n");
                    sb.Append(offset);
                    sb.Append(FunctionParameterSignature(parameter));
                }
            }
            sb.Append(" ) -> ");
            sb.Append(function.ReturnType);

            return sb.ToString();
        }

        public static string FunctionParameterSignature(RealizedParameter parameter) {
            StringBuilder sb = new StringBuilder();

            sb.Append(parameter.name);
            sb.Append(" : ");
            sb.Append(parameter.type);

            return sb.ToString();
        }

        public static string MethodSignature(string type, string name, IMethodInvokeFactory method) {
            StringBuilder sb = new StringBuilder();

            sb.Append(type.ToLowerInvariant());
            sb.Append(".");
            sb.Append(name);

            if (method.DeclaredParameters.Count == 0) {
                sb.Append(" (");
            } else {
                sb.Append(" ( ");

                string offset = new String(' ', sb.Length);

                sb.Append(MethodParameterSignature(method.DeclaredParameters.First()));
                foreach (FunctionParameter parameter in method.DeclaredParameters.Skip(1)) {
                    sb.Append(",\n");
                    sb.Append(offset);
                    sb.Append(MethodParameterSignature(parameter));
                }
            }
            sb.Append(" ) -> ");
            sb.Append(method.DeclaredReturn);

            return sb.ToString();
        }

        public static string MethodParameterSignature(FunctionParameter parameter) {
            StringBuilder sb = new StringBuilder();

            sb.Append(parameter.name);
            sb.Append(" : ");
            sb.Append(parameter.type);

            return sb.ToString();
        }
    }
}
