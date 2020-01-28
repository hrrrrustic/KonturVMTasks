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
            return new ConditionBreakPoint(new Word(dto.Address), dto.Name, new Word(dto.FirstArgument),
                new Word(dto.SecondArgument), dto.ComparisonOperator, dto.IsDoubleMemoryAddressed);
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
            private readonly Dictionary<string, Func<Word, Word, bool>> supportedConditions = new Dictionary<string, Func<Word, Word, bool>>
            {
                [">"] = (x, y) => x.ToUInt() > y.ToUInt(),
                ["<"] = (x, y) => x.ToUInt() < y.ToUInt(),
                [">="] = (x, y) => x.ToUInt() >= y.ToUInt(),
                ["<="] = (x, y) => x.ToUInt() <= y.ToUInt(),
                ["=="] = (x, y) => x.ToUInt() == y.ToUInt(),
                ["!="] = (x, y) => x.ToUInt() != y.ToUInt(),
            };
            public Word Address { get; }
            public string Name { get; }

            public Word FirstArgument { get; }
            public Word SecondArgument { get; }

            public string ComparisonOperator { get; }

            public bool IsDoubleAddressed { get;}

            public ConditionBreakPoint(Word address, string name, Word firstArgument, Word secondArgument, string comparisonOperator, bool isDoubleAddressed)
            {
                Address = address;
                Name = name;
                FirstArgument = firstArgument;
                SecondArgument = secondArgument;
                ComparisonOperator = comparisonOperator;
                IsDoubleAddressed = isDoubleAddressed;
            }
            public bool ShouldStop(IReadOnlyMemory memory)
            {
                Word firstValue = memory.ReadWord(FirstArgument);
                Word secondValue = IsDoubleAddressed ? memory.ReadWord(SecondArgument) : SecondArgument;

                return ConditionResult(firstValue, secondValue, ComparisonOperator);
            }

            private bool ConditionResult(Word first, Word second, string operation)
            {
                return supportedConditions[operation].Invoke(first, second);
            }
        }
    }
}