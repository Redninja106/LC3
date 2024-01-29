namespace LC3;
class GeneralRegisters
{
    private readonly ushort[] registers = new ushort[8];
    public ref ushort R0 => ref registers[0];
    public ref ushort R1 => ref registers[1];
    public ref ushort R2 => ref registers[2];
    public ref ushort R3 => ref registers[3];
    public ref ushort R4 => ref registers[4];
    public ref ushort R5 => ref registers[5];
    public ref ushort R6 => ref registers[6];
    public ref ushort R7 => ref registers[7];
    public ref ushort this[int index] => ref registers[index];
}
