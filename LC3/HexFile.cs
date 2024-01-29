using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3;
internal class HexFile(ushort startAddress, ushort[] values)
{
    public ushort StartAddress { get; } = startAddress;
    public ushort[] Values { get; } = values;

    public static HexFile FromSource(string source)
    {
        ushort? startAddress = null;
        List<ushort> values = [];
        string[] lines = source.Split(["\n\r", "\n"], StringSplitOptions.None);

        if (lines.Length < 1)
        {
            throw new Exception("missing origin! (first line)");
        }

        if (string.IsNullOrEmpty(lines[^1]))
        {
            lines = lines[..^1];
        }

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim('\r');
            if (lines[i].Length != 4)
                throw new Exception("error on line " + (i + 1));
            ushort value = ushort.Parse(lines[i], NumberStyles.HexNumber);
            if (startAddress == null)
            {
                startAddress = value;
            }
            else
            {
                values.Add(value);
            }
        }

        return new(startAddress!.Value, [.. values]);
    }

    public string ToSource()
    {
        StringBuilder sb = new();
        sb.AppendLine(StartAddress.ToString("x4"));
        foreach (var value in Values)
        {
            sb.AppendLine(value.ToString("x4"));
        }
        return sb.ToString();
    }
}
