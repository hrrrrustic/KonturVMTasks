using System;
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
            return new ConditionBreakPoint(new Word(dto.FirstArgument), new Word(dto.SecondArgument), dto.IsDoubleMemoryAddressed, dto.Condition, dto.Name);
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
		private class ConditionBreakPoint : IBreakPoint
        {
            public Word Address { get; }
            public string Name { get; }

            public Word SecondArgument { get; }

            public Func<Word, Word, bool> Condition { get; }
            public bool IsDoubleAddressed { get; set; }

            public ConditionBreakPoint(Word firstArgument, Word secondArgument, bool isDoubleAddressed, Func<Word, Word, bool> condition, string name)
            {
                Address = firstArgument;
                Name = name;
                IsDoubleAddressed = isDoubleAddressed;
                SecondArgument = secondArgument;
                Condition = condition;

            }
            public bool ShouldStop(IReadOnlyMemory memory)
            {
                Word firstValue = memory.ReadWord(Address);
                Word secondValue = IsDoubleAddressed ? memory.ReadWord(SecondArgument) : SecondArgument;
                
                return Condition.Invoke(firstValue, secondValue);

            }
        }
    }
}