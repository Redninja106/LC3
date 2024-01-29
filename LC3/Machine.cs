


namespace LC3;

/// <summary>
/// Represents the state of an LC-3 machine.
/// </summary>
class Machine(int memorySize)
{
    public GeneralRegisters GeneralRegisters { get; } = new();
    public SpecialRegisters SpecialRegisters { get; } = new();

    public ushort[] Memory { get; } = new ushort[memorySize];

    public ushort ReadU16(ushort address)
    {
        return Memory[address];
    }

    public void WriteU16(ushort address, ushort value)
    {
        Memory[address] = value;
    }
}
