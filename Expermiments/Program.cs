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

        public void func(int _b, Demo other) {
            Console.Out.WriteLine(_b);
            other.a = 12;
            this.a = other.b * 2;
        }

        public void test<T>(T value) {
            this.a = this.b * 2;
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
            ValueTuple<bool, int> tuple1;
            ValueTuple<bool, int> tuple2;
            var longArr = new[] { 123L, 234L };
            ValueTuple<bool, int> tuple3 = (false, 56);

            longArr[1] = 345L;

            tuple1.Item1 = false;
            tuple1.Item2 = 12;
            tuple2.Item1 = true;
            tuple2.Item2 = 23;

            tuple1.Item1 = tuple2.Item1;

            var arrOfTuple = new[] { tuple1, tuple2 };

            arrOfTuple[1].Item1 = true;
            arrOfTuple[0] = tuple3;

            var other = (ValueTuple<bool, int>[])arrOfTuple.Clone();

            double a = -300;
            double b = 123.67;
            double c = a % b;

            Console.Out.WriteLine(c);

            Demo d = new Demo();

            d.Prop = 123;
            d.sub.s.e = 0;

            int i = 0;

            if ((++i) > 10000) {
                i = 0;
                Console.Out.WriteLine(">>> ");
            }

            foreach (var m in typeof(Demo).GetMethod("test").GetGenericArguments()) {
                Console.Out.WriteLine(">>> " + m.Name);
            }
        }
    }
}
