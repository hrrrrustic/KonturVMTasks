using System;
using System.Collections.Generic;
using VirtualMachine.Core;
using VirtualMachine.Core.Debugger.Model;

namespace VirtualMachine.Core.Debugger.Server.BreakPoints
{
	public class BreakPointsConverter
	{
		public IBreakPoint FromDto(BreakPointDto dto)
		{
			return new BreakPoint(new Word(dto.Address), dto.Name);
		}

        public IBreakPoint FromDto(ConditionBreakPointDto dto)
        {
            return new ConditionBreakPoint(new Word(dto.Address),  new Word(dto.Argument),
                dto.IsDoubleMemoryAddressed, dto.Name, dto.ComparisonOperator);
        }

		public BreakPointDto ToDto(IBreakPoint bp)
		{
			return new BreakPointDto
			{
				Name = bp.Name,
				Address = bp.Address.ToUInt()
			};
		}

		private class BreakPoint : IBreakPoint
		{
			public BreakPoint(Word address, string name)
			{
				Address = address;
				Name = name;
			}

			public Word Address { get; }
			public string Name { get; }
			public bool ShouldStop(IReadOnlyMemory memory) => true;
		}
		public class ConditionBreakPoint : IBreakPoint
        {
            public Word Address { get; }
            public string Name { get; }

            public Word Argument { get; }

			public string ComparisonOperator { get; }

            public bool IsDoubleAddressed { get;}

            public ConditionBreakPoint(Word address, Word argument, bool isDoubleAddressed, string name, string comparisonOperator)
            {
                Address = address;
                Argument= argument;
                Name = name;
                ComparisonOperator = comparisonOperator;
                IsDoubleAddressed = isDoubleAddressed;

            }
            public bool ShouldStop(IReadOnlyMemory memory)
            {
                Word firstValue = memory.ReadWord(Address);
                Word secondValue = IsDoubleAddressed ? memory.ReadWord(Argument) : Argument;

                return ConditionResult(firstValue, secondValue, ComparisonOperator);
            }

            private bool ConditionResult(Word first, Word second, string operation)
            {
                return ConditionResult(first.ToUInt(), second.ToUInt(), operation);
            }

            private bool ConditionResult(uint first, uint second, string operation)
            {
                switch (operation)
                {
                    case ">":
                        return first > second;
                    case ">=":
                        return first >= second;
                    case "<":
                        return first < second;
                    case "<=":
                        return first <= second;
                    case "==":
                        return first == second;
                    case "!=":
                        return first != second;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}