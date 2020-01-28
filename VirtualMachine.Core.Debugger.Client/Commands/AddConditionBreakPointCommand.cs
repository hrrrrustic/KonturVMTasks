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
        public IReadOnlyList<string> ParameterNames { get; } = new[] { "name", "address", "condition"};

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
            dto.Address = uint.Parse(parameters[1]);

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
                    IsDoubleMemoryAddressed = true,
                    FirstArgument = firstValue,
                    SecondArgument = secondValue,
                    ComparisonOperator = splitResult[1]
                };
            }

            if (leftPartIsMemoryAddress)
            {
                return new ConditionBreakPointDto
                {
                    FirstArgument = firstValue,
                    SecondArgument = secondValue,
                    IsDoubleMemoryAddressed = false,
                    ComparisonOperator = splitResult[1]
                };
            }

            string reverseStringOperator = reverseOperations[splitResult[1]];

            return new ConditionBreakPointDto
            {
                FirstArgument = secondValue,
                SecondArgument = firstValue,
                IsDoubleMemoryAddressed = false,
                ComparisonOperator = reverseStringOperator
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