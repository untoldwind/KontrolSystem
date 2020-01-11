using System;
using System.Linq;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using System.Collections.Generic;
using UnityEngine;
using KontrolSystem.KSP.Runtime.Testing;

namespace Expermiments {
    struct Demo {
        public int a;
        public int b;

        public static void func(int _b) { Console.Out.WriteLine(_b); }

        public void test<T>(T value) { }
    }

    class MyFuture : Future<int> {
        public void Abort() {
            throw new NotImplementedException();
        }

        public override FutureResult<int> PollValue() {
            return new FutureResult<int>();
        }

        public override string ToString() {
            return "bal";
        }
    }

    class MainClass {
        public static void Main(string[] args) {
            var context = new KSPTestRunnerContext();
            string a = "abc";

            int c = a.Length;

            foreach (var m in typeof(Demo).GetMethod("test").GetGenericArguments()) {
                System.Console.Out.WriteLine(">>> " + m.Name);
            }
        }
    }
}
