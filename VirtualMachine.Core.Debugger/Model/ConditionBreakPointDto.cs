using System;

namespace VirtualMachine.Core.Debugger.Model
{
    public class ConditionBreakPointDto
    {
        public uint Address { get; set; }
        public string Name { get; set; }
        public uint Argument { get; set; }
        public bool IsDoubleMemoryAddressed { get; set; }
        public string ComparisonOperator { get; set; }
    }
}