using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Assembly;
internal class AssemblySyntaxTree
{
    public AssemblyStatement[] Statements { get; }

    public AssemblySyntaxTree(AssemblyStatement[] statements)
    {
        this.Statements = statements;
    }

    public static AssemblySyntaxTree FromSource(string source)
    {
        List<AssemblyStatement> statements = [];
        var lines = source.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            int semicolonIndex = line.IndexOf(';');
            if (semicolonIndex == -1)
                semicolonIndex = line.Length;
            var code = line[..semicolonIndex].Trim();

            var words = code.Split((char[])[' ', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (words.Length is 0)
                continue;
            statements.Add(ParseStatement(words));
        }

        return new([..statements]);
    }

    private static AssemblyStatement ParseStatement(string[] words)
    {
        if (words[0].StartsWith("."))
        {
            return new AssemblyDirective(Enum.Parse<DirectiveKind>(words[0][1..], true), words[1..]);
        }
        else if (words[0].EndsWith(':'))
        {
            if (words.Length > 1)
            {
                throw new Exception("Label");
            }
            return new AssemblyLabel(words[0][..^1]);
        }
        else
        {
            return new AssemblyInstruction(words[0], words[1..]);
        }
    }
}

internal abstract record AssemblyStatement
{
    // LD R0, R0, R1
    public abstract int Size { get; }

}

internal record AssemblyInstruction(string OpCode, string[] Arguments) : AssemblyStatement
{
    public override int Size => 1;

    public ArgumentReader GetArgumentReader(Dictionary<string, ushort>? labels = null)
    {
        return new(Arguments, labels);
    }
}

internal record AssemblyLabel(string Name) : AssemblyStatement
{
    public override int Size => 0;
}

internal record AssemblyDirective(DirectiveKind Kind, string[] Arguments) : AssemblyStatement
{
    public override int Size => Kind switch
    {
        DirectiveKind.Blkw => GetArgumentReader().NextLiteral(false),
        DirectiveKind.Fill => 1,
        DirectiveKind.Orig => 0,
        _ => throw new(),
    };

    public ArgumentReader GetArgumentReader()
    {
        return new(Arguments);
    }
}

enum DirectiveKind
{
    Orig,
    Fill,
    Blkw
}