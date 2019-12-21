using NUnit.Framework;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class ILEmitterTests {
        [Test]
        public void TestStackCount() {
            Assert.AreEqual(StackBehaviour.Pop0, OpCodes.Ldarg_0.StackBehaviourPop);
            Assert.AreEqual(StackBehaviour.Push1, OpCodes.Ldarg_0.StackBehaviourPush);

            Assert.AreEqual(StackBehaviour.Pop1, OpCodes.Starg.StackBehaviourPop);
            Assert.AreEqual(StackBehaviour.Push0, OpCodes.Starg.StackBehaviourPush);

            Assert.AreEqual(StackBehaviour.Varpop, OpCodes.Call.StackBehaviourPop);
            Assert.AreEqual(StackBehaviour.Varpush, OpCodes.Call.StackBehaviourPush);
        }
    }
}
