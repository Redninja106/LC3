using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Assembly;
internal static class AssemblyCompiler
{
    public static HexFile Compile(AssemblySyntaxTree syntaxTree)
    {
        Dictionary<string, ushort> labels = GetLabels(syntaxTree);
        
        List<ushort> values = [];
        ushort? firstOrigin = null;

        foreach (var statement in syntaxTree.Statements)
        {
            switch (statement)
            {
                case AssemblyDirective directive:
                    var argReader = directive.GetArgumentReader();
                    if (directive.Kind == DirectiveKind.Orig)
                    {
                        if (firstOrigin is null)
                        {
                            firstOrigin = (ushort)directive.GetArgumentReader().NextLiteral(false);
                        }
                        else
                        {
                            int currentLocation = firstOrigin.Value + values.Count;
                            int distanceToNewOrg = directive.GetArgumentReader().NextLiteral(false) - currentLocation;
                            if (distanceToNewOrg < 0)
                                throw new Exception("Invalid origin!");
                            values.AddRange(Enumerable.Repeat((ushort)0, distanceToNewOrg));
                        }
                    }
                    else if (directive.Kind == DirectiveKind.Blkw)
                    {
                        int count = directive.GetArgumentReader().NextLiteral(false);
                        values.AddRange(Enumerable.Repeat((ushort)0, count));
                    }
                    else if (directive.Kind == DirectiveKind.Fill)
                    {
                        ushort value = (ushort)directive.GetArgumentReader().NextLiteral(true);
                        values.Add(value);
                    }
                    else
                    {
                        throw new("Unknown directive!");
                    }
                    break;
                case AssemblyInstruction instruction:
                    var opCode = Enum.Parse<OpCode>(instruction.OpCode, true);
                    var handler = Interpreter.GetOpCodeHandler(opCode);
                    var argReader0 = instruction.GetArgumentReader(labels);
                    var instructionValue = handler.Parse(ref argReader0, (ushort)(firstOrigin!.Value + 1 + values.Count));
                    values.Add(instructionValue.Value);
                    break;
                default:
                    break;
            }
        }

        return new HexFile(firstOrigin ?? 0, [..values]);
    }

    // first pass determines label locations. needs to be a seperate pass for forward label references
    private static Dictionary<string, ushort> GetLabels(AssemblySyntaxTree syntaxTree)
    {
        Dictionary<string, ushort> labels = new();

        ushort location = 0;

        foreach (var statement in syntaxTree.Statements)
        {
            if (statement is AssemblyDirective directive && directive.Kind == DirectiveKind.Orig)
            {
                location = (ushort)directive.GetArgumentReader().NextLiteral(false);
            }

            if (statement is AssemblyLabel label)
            {
                labels.Add(label.Name, location);
            }

            location = (ushort)(location + statement.Size);
        }

        return labels;
    }
}
