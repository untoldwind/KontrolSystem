using System;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.Testing;

namespace Expermiments {
    struct Sub1 {
        public int e;
    }

    struct Sub {
        public int d;
        public Sub1 s;
    }

    struct Demo {
        public int a;
        public int b;
        public Sub sub;
        public int Prop { get; set; }

        public static void func(int _b) {
            Console.Out.WriteLine(_b);
        }

        public void test<T>(T value) {
        }
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
            ValueTuple<Boolean> tuple1;
            ValueTuple<Boolean> tuple2;
            var longArr = new[] {123L, 234L};

            longArr[1] = 345L;

            tuple1.Item1 = false;
            tuple2.Item1 = true;

            tuple1.Item1 = tuple2.Item1;

            var arrOfTuple = new[] {tuple1, tuple2};

            arrOfTuple[1].Item1 = true;

            double a = -300;
            double b = 123.67;
            double c = a % b;

            Console.Out.WriteLine(c);

            Demo d = new Demo();

            d.Prop = 123;
            d.sub.s.e = 0;

            foreach (var m in typeof(Demo).GetMethod("test").GetGenericArguments()) {
                Console.Out.WriteLine(">>> " + m.Name);
            }
        }
    }
}
