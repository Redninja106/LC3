using ImGuiNET;
using LC3.InstructionHandlers;

namespace LC3.Views;

internal class QuickAssemblerView(Machine machine, Interpreter interpreter) : View(machine, interpreter)
{
    private ushort value;
    private OpCode[] opCodes = Enum.GetValues<OpCode>();
    private string opCodesStr = string.Join('\0', Enum.GetValues<OpCode>());
    private int currentOpcode;
    private ushort currentAddress;

    protected override void OnLayout()
    {
        Instruction instruction = new(value);
        if (ImGui.Combo("##combo", ref currentOpcode, opCodesStr))
        {
            instruction.OpCode = opCodes[currentOpcode];
        }
        var handler = Interpreter.GetOpCodeHandler(instruction.OpCode);

        handler.Layout(ref instruction, this);

        ImGui.Separator();

        value = instruction.Value;
        if (HexInput("value", ref value))
        {
            currentOpcode = Array.IndexOf(opCodes, new Instruction(value).OpCode);
        }

        HexInput("address", ref currentAddress);

        if (ImGui.Button("write"))
        {
            machine.WriteU16(currentAddress, value);
            currentAddress++;
        }
    }

    string registerString = "R0\0R1\0R2\0R3\0R4\0R5\0R6\0R7\0R8";
    public int PickRegister(string name, int value)
    {
        ImGui.Combo(name, ref value, registerString);
        return value;
    }

    public int PickSigned(string name, int value, int bits)
    {
        if (HexInput(name, ref value))
        {
            value = int.Clamp(value, (short) -(1 << bits), (short)((1 << bits) - 1));
        }
        return value & ((1 << bits) - 1);
    }

    internal bool PickBit(string name, bool value)
    {
        ImGui.Checkbox(name, ref value);
        return value;
    }
}