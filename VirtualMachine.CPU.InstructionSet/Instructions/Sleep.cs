using System;
using VirtualMachine.Core;

namespace VirtualMachine.CPU.InstructionSet.Instructions
{
    public class Sleep : InstructionBase
    {
        public Sleep(OperandType first) : base(9, first, OperandType.Ignored, OperandType.Ignored)
        {
        }

        protected override void ExecuteInternal(ICpu cpu, IMemory memory, Operand op0, Operand op1, Operand op2)
        {
            int sleepTime = op0.Value.ToInt();
            cpu.Sleep(sleepTime);
        }
    }
}