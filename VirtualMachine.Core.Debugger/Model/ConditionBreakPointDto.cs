using System;

namespace VirtualMachine.Core.Debugger.Model
{
    public class ConditionBreakPointDto
    {
        public uint Address { get; set; }
        public string Name { get; set; }
        public uint FirstArgument { get; set; }
        public uint SecondArgument { get; set; }
        public bool IsDoubleMemoryAddressed { get; set; }
        public string ComparisonOperator { get; set; }
    }
}