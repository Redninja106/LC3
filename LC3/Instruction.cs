namespace LC3;

struct Instruction
{
    public ushort Value { get; private set; }

    public Instruction(ushort value)
    {
        this.Value = value;
    }    

    public OpCode OpCode 
    { 
        get => (OpCode)GetBits(12, 4);
        set => SetBits(12, 4, (int)value);
    }

    public int DR
    {
        get => GetBits(9, 3);
        set => SetBits(9, 3, value);
    }

    public int SR1 
    {
        get => GetBits(6, 3);
        set => SetBits(6, 3, value);
    }

    public int SR2
    {
        get => GetBits(0, 3);
        set => SetBits(0, 3, value);
    }

    public int Imm5 
    {
        get => GetBits(0, 5);
        set => SetBits(0, 5, value);
    }
    public int PCOffset9
    {
        get => GetBits(0, 9);
        set => SetBits(0, 9, value);
    }
    public int PCOffset11
    {
        get => GetBits(0, 11);
        set => SetBits(0, 11, value);
    }
    public int BaseR
    {
        get => GetBits(6, 3);
        set => SetBits(6, 3, value);
    }
    public int Offset6
    {
        get => GetBits(0, 6);
        set => SetBits(0, 6, value);
    }
    public int TrapVect8
    {
        get => GetBits(0, 8);
        set => SetBits(0, 8, value);
    }

    public bool GetBit(int bit)
    {
        return ((Value >> bit) & 1) == 1;
    }

    public void SetBit(int position, bool value)
    {
        SetBits(position, 1, value ? 1 : 0);
    }

    private void SetBits(int position, int count, int bits)
    {
        int mask = ((1 << count) - 1) << position;
        int existingBits = this.Value & ~mask;
        int newBits = (bits << position) & mask;
        this.Value = (ushort)(existingBits | newBits);
    }

    private int GetBits(int position, int count)
    {
        return (Value >> position) & ((1 << count) - 1);
    }
}
