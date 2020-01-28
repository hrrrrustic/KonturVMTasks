using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using VirtualMachine.Core.Debugger.Model;

namespace VirtualMachine.Core.Debugger.Client.Commands
{
    public class AddConditionBreakPointCommand : ICommand
    {
        public string Name { get; } = "bp-add-c";
        public string Info { get; } = "Add break point with condition";
        public IReadOnlyList<string> ParameterNames { get; } = new[] { "name","condition"};

        private readonly Dictionary<string, Func<Word, Word, bool>> supportedConditions = new Dictionary<string, Func<Word, Word, bool>>
        {
            [">"] = (x, y) => x.ToUInt() > y.ToUInt(),
            ["<"] = (x, y) => x.ToUInt() < y.ToUInt(),
            [">="] = (x, y) => x.ToUInt() >= y.ToUInt(),
            ["<="] = (x, y) => x.ToUInt() <= y.ToUInt(),
            ["=="] = (x, y) => x.ToUInt() == y.ToUInt(),
            ["!="] = (x, y) => x.ToUInt() != y.ToUInt(),
        };

        private readonly Dictionary<string, string> reverseOperations = new Dictionary<string, string>
        {
            [">"] = "<",
            ["<"] = ">",
            [">="] = "<=",
            ["<="] = ">=",
            ["=="] = "==",
            ["!="] = "!=",
        };
        public Task ExecuteAsync(DebuggerModel model, string[] parameters)
        {
            ConditionBreakPointDto dto = ParseCondition(parameters[2], "mem");
            dto.Name = parameters[0];

            return model.Client.AddConditionBreakPointAsync(dto);
        }

        public ConditionBreakPointDto ParseCondition(string stringCondition, string memorySignature)
        {
            string[] splitResult = stringCondition.Split(' ').Select(k => k.Trim()).ToArray();

            uint firstValue = ParseConditionPart(splitResult[0], memorySignature, out bool leftPartIsMemoryAddress);
            uint secondValue = ParseConditionPart(splitResult[2], memorySignature, out bool rightPartIsMemoryAddress);

            bool doubleMemoryAddressed = leftPartIsMemoryAddress && rightPartIsMemoryAddress;

            if (doubleMemoryAddressed)
            {
                return new ConditionBreakPointDto
                { 
                    FirstArgument = firstValue,
                    IsDoubleMemoryAddressed = true,
                    SecondArgument = secondValue,
                    Condition = supportedConditions[splitResult[1]]
                };
            }

            if (leftPartIsMemoryAddress)
            {
                return new ConditionBreakPointDto
                {
                    FirstArgument = firstValue,
                    SecondArgument = secondValue,
                    IsDoubleMemoryAddressed = false,
                    Condition = supportedConditions[splitResult[1]]
                };
            }

            string reverseStringOperator = reverseOperations[splitResult[1]];

            return new ConditionBreakPointDto
            {
                Condition = supportedConditions[reverseStringOperator],
                FirstArgument = secondValue,
                SecondArgument = firstValue,
                IsDoubleMemoryAddressed = false,
            };
        }

        private uint ParseConditionPart(string source, string memorySignature, out bool isMemoryAddress)
        {
            isMemoryAddress = false;

            if (source.StartsWith(memorySignature))
            {
                isMemoryAddress = true;
                return ParseMemoryAddressString(source, memorySignature);
            }

            return uint.Parse(source.Substring(2), NumberStyles.HexNumber);
            
        }
        private uint ParseMemoryAddressString(string source, string memorySignature)
        {
            int startIndex = memorySignature.Length + 1;
            int length = source.Length - 1 - startIndex;
            string hexValue = source.Substring(startIndex, length);
            return uint.Parse(hexValue.Substring(2), NumberStyles.HexNumber);
        }
    }
}