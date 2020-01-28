using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualMachine.Core;
using VirtualMachine.Core.Debugger.Client;
using VirtualMachine.Core.Debugger.Client.Commands;
using VirtualMachine.Core.Debugger.Model;

namespace ConditionBreakPointTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var test = new AddConditionBreakPointCommand();

            var dtoResult = test.ParseCondition("mem(0x10) == 0xA2", "mem");

            Assert.AreEqual(dtoResult.IsDoubleMemoryAddressed, false);
            Assert.AreEqual(dtoResult.FirstArgument, uint.Parse("10", NumberStyles.HexNumber));
            Assert.AreEqual(dtoResult.SecondArgument, uint.Parse("A2", NumberStyles.HexNumber));
            Assert.AreEqual(dtoResult.Condition.Invoke(new Word(dtoResult.FirstArgument), new Word(dtoResult.SecondArgument)), false);

        }

        [TestMethod]
        public void TestMethod2()
        {
            var test = new AddConditionBreakPointCommand();

            var dtoResult = test.ParseCondition("mem(0x2048) != mem(0x2040)", "mem");

            Assert.AreEqual(dtoResult.IsDoubleMemoryAddressed, true);
            Assert.AreEqual(dtoResult.FirstArgument, uint.Parse("2048", NumberStyles.HexNumber));
            Assert.AreEqual(dtoResult.SecondArgument, uint.Parse("2040", NumberStyles.HexNumber));
            Assert.AreEqual(dtoResult.Condition.Invoke(new Word(dtoResult.FirstArgument), new Word(dtoResult.SecondArgument)), true);

        }

        [TestMethod]
        public void TestMethod3()
        {
            var test = new AddConditionBreakPointCommand();

            var dtoResult = test.ParseCondition("0xA2 > mem(0x10)", "mem");

            Assert.AreEqual(dtoResult.IsDoubleMemoryAddressed, false);
            Assert.AreEqual(dtoResult.FirstArgument, uint.Parse("10", NumberStyles.HexNumber));
            Assert.AreEqual(dtoResult.SecondArgument, uint.Parse("A2", NumberStyles.HexNumber));
            Assert.AreEqual(dtoResult.Condition.Invoke(new Word(dtoResult.FirstArgument), new Word(dtoResult.SecondArgument)), true);

        }
    }
}
