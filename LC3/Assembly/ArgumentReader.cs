using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Assembly;
internal struct ArgumentReader
{
    private int position = 0;
    private string[] arguments;

    private Dictionary<string, ushort>? labels = null;
    public readonly bool IsAtEnd => position >= arguments.Length;

    public ArgumentReader(string[] arguments, Dictionary<string, ushort>? labels = null)
    {
        this.arguments = arguments;
        this.labels = labels;
    }

    public string Next()
    {
        var result = arguments[position];
        position++;
        return result;
    }

    public int NextLiteral(bool allowNegative)
    {
        string word = Next();

        bool isHex = false;
        bool isNegative = false;

        if (word.StartsWith('-'))
        {
            if (!allowNegative)
            {
                throw new Exception("Literal must be positive");
            }
            word = word[1..];
            isNegative = true;
        }
        if (word.StartsWith('x'))
        {
            word = word[1..];
            isHex = true;
        }
        if (word.StartsWith("0x"))
        {
            word = word[2..];
            isHex = true;
        }

        if (!int.TryParse(word, isHex ? NumberStyles.HexNumber : 0, null, out int result))
        {
            throw new Exception("error parsing number!");
        }

        return isNegative ? -result : result;
    }

    public int NextRegister()
    {
        string word = Next();

        if (!word.StartsWith('r') && !word.StartsWith('R'))
        {
            throw new Exception("Expected a register!");
        }

        word = word[1..];

        if (!int.TryParse(word, out int result))
        {
            throw new Exception("Expected a register!");
        }

        if (result < 0 || result >= 8)
        {
            throw new Exception("Invalid register!");
        }

        return result;
    }

    public void End()
    {
        if (!IsAtEnd)
        {
            throw new Exception("Unexpected extra arguments!");
        }
    }

    public ushort NextLabel()
    {
        return labels[Next()];
    }

    public ushort NextAddress()
    {
        ArgumentReader reader = this;

        try
        {
            ushort label = reader.NextLabel();
            this = reader;
            return label;
        }
        catch
        {
        }

        return (ushort)NextLiteral(false);
    }
}
