using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualMachine.Core;
using VirtualMachine.Core.Debugger.Client.Commands;
using static VirtualMachine.Core.Debugger.Server.BreakPoints.BreakPointsConverter;

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

            uint firstValue = uint.Parse("10", NumberStyles.HexNumber);
            uint secondValue = uint.Parse("A2", NumberStyles.HexNumber);

            Assert.AreEqual(firstValue, dtoResult.FirstArgument);
            Assert.AreEqual("==", dtoResult.ComparisonOperator);
            Assert.AreEqual(false, dtoResult.IsDoubleMemoryAddressed);
            Assert.AreEqual(secondValue, dtoResult.SecondArgument);
            Ram ram = new Ram(0, 10000);
            
            ram.WriteWord(new Word(firstValue), new Word(secondValue));

            var breakPoint = new ConditionBreakPoint(new Word(dtoResult.Address), dtoResult.Name,
                new Word(dtoResult.FirstArgument), new Word(dtoResult.SecondArgument), dtoResult.ComparisonOperator,
                dtoResult.IsDoubleMemoryAddressed);

            Assert.IsTrue(breakPoint.ShouldStop(ram));
        }

        [TestMethod]
        public void TestMethod2()
        {
            var test = new AddConditionBreakPointCommand();

            var dtoResult = test.ParseCondition("mem(0x2048) != mem(0x2040)", "mem");

            uint firstValue = uint.Parse("2048", NumberStyles.HexNumber);
            uint secondValue = uint.Parse("2040", NumberStyles.HexNumber);

            Assert.AreEqual(firstValue, dtoResult.FirstArgument);
            Assert.AreEqual("!=", dtoResult.ComparisonOperator);
            Assert.AreEqual(true, dtoResult.IsDoubleMemoryAddressed);
            Assert.AreEqual(secondValue, dtoResult.SecondArgument);

            Ram ram = new Ram(0, 10000);

            ram.WriteWord(new Word(firstValue), new Word(1));
            ram.WriteWord(new Word(secondValue), new Word(1));


            var breakPoint = new ConditionBreakPoint(new Word(dtoResult.Address), dtoResult.Name, new Word(dtoResult.FirstArgument),
               new Word(dtoResult.SecondArgument), dtoResult.ComparisonOperator, dtoResult.IsDoubleMemoryAddressed);

            Assert.IsTrue(!breakPoint.ShouldStop(ram));
        }

        [TestMethod]
        public void TestMethod3()
        {
            var test = new AddConditionBreakPointCommand();

            var dtoResult = test.ParseCondition("0xA2 > mem(0x10)", "mem");

            uint firstValue = uint.Parse("10", NumberStyles.HexNumber);
            uint secondValue = uint.Parse("A2", NumberStyles.HexNumber);

            Assert.AreEqual(firstValue, dtoResult.FirstArgument);
            Assert.AreEqual("<", dtoResult.ComparisonOperator);
            Assert.AreEqual(false, dtoResult.IsDoubleMemoryAddressed);
            Assert.AreEqual(secondValue, dtoResult.SecondArgument);

            Ram ram = new Ram(0, 10000);

            ram.WriteWord(new Word(firstValue), new Word(0));

            var breakPoint = new ConditionBreakPoint(new Word(dtoResult.Address), dtoResult.Name, new Word(dtoResult.FirstArgument),
               new Word(dtoResult.SecondArgument), dtoResult.ComparisonOperator, dtoResult.IsDoubleMemoryAddressed);

            Assert.IsTrue(breakPoint.ShouldStop(ram));
        }
    }
}
