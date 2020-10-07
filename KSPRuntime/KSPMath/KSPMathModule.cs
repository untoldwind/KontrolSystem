using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.AST;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class KSPMathModule {
        private const string ModuleName = "ksp::math";

        private static KSPMathModule _instance;

        public static KSPMathModule Instance {
            get {
                if (_instance == null) _instance = new KSPMathModule();
                return _instance;
            }
        }

        public IKontrolModule module;

        KSPMathModule() {
            List<RealizedType> types = new List<RealizedType> {
                Vector2Binding.Vector2Type,
                Vector3Binding.Vector3Type,
                DirectionBinding.DirectionType
            };

            BindingGenerator.RegisterTypeMapping(typeof(Vector2d), Vector2Binding.Vector2Type);
            BindingGenerator.RegisterTypeMapping(typeof(Vector3d), Vector3Binding.Vector3Type);
            BindingGenerator.RegisterTypeMapping(typeof(Direction), DirectionBinding.DirectionType);

            List<CompiledKontrolConstant> constants = new List<CompiledKontrolConstant>();

            List<CompiledKontrolFunction> functions = new List<CompiledKontrolFunction> {
                Direct.BindFunction(typeof(Vector2Binding), "vec2", "Create a new 2-dimensional vector", typeof(double),
                    typeof(double)),
                Direct.BindFunction(typeof(Vector3Binding), "vec3", "Create a new 3-dimensional vector", typeof(double),
                    typeof(double), typeof(double)),
                Direct.BindFunction(typeof(DirectionBinding), "euler", "Create a Direction from euler angles in degree",
                    typeof(double), typeof(double), typeof(double)),
                Direct.BindFunction(typeof(DirectionBinding), "angle_axis",
                    "Create a Direction from a given axis with rotation angle in degree", typeof(double),
                    typeof(Vector3d)),
                Direct.BindFunction(typeof(DirectionBinding), "look_dir_up",
                    "Create a Direction from a fore-vector and an up-vector", typeof(Vector3d), typeof(Vector3d)),
                Direct.BindFunction(typeof(ExtraMath), "AngleDelta",
                    "Calculate the difference between to angles in degree (-180 .. 180)", typeof(double),
                    typeof(double))
            };

            module = Direct.BindModule(ModuleName, "Collection of KSP/Unity related mathematical functions.", types,
                constants, functions);
        }
    }
}
