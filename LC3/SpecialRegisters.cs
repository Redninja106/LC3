namespace LC3;
class SpecialRegisters
{
    public ushort PC { get; set; }
    public ushort IR { get; set; }
    public bool N { get; set; } = false;
    public bool Z { get; set; } = true;
    public bool P { get; set; } = false;

    public void SetConditions(short value)
    {
        N = value < 0;
        Z = value == 0;
        P = value > 0;
    }
}