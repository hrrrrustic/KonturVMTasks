﻿using System;
using VirtualMachine.Core;

namespace VirtualMachine.CPU.InstructionSet.Instructions
{
    public class Exit : InstructionBase
    {
        public Exit() : base(8, OperandType.Ignored, OperandType.Ignored ,OperandType.Ignored)
        {
        }

        protected override void ExecuteInternal(ICpu cpu, IMemory memory, Operand op0, Operand op1, Operand op2)
        {
            cpu.Stop();
        }
    }
}