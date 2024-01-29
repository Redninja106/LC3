using ImGuiNET;
using LC3.Assembly;
using LC3.Views;

namespace LC3.InstructionHandlers;

class AddHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ref ushort dr = ref machine.GeneralRegisters[instruction.DR];
        ref ushort sr1 = ref machine.GeneralRegisters[instruction.SR1];

        if (instruction.GetBit(5))
        {
            dr = unchecked((ushort)(sr1 + Utils.SignExtend16(instruction.Imm5, 5)));
        }
        else
        {
            ref ushort sr2 = ref machine.GeneralRegisters[instruction.SR2];
            dr = unchecked((ushort)(sr1 + sr2));
        }

        machine.SpecialRegisters.SetConditions((short)dr);
    }
    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        bool immediate = instruction.GetBit(5);
        if (ImGui.Checkbox("Immediate", ref immediate))
        {
            instruction.SetBit(5, immediate);
        }

        instruction.DR = view.PickRegister("DR", instruction.DR);
        instruction.SR1 = view.PickRegister("SR1", instruction.SR1);
        if (immediate)
        {
            instruction.Imm5 = view.PickSigned("Imm5", instruction.Imm5, 5);
        }
        else
        {
            instruction.SR2 = view.PickRegister("SR2", instruction.SR2);
        }
        base.Layout(ref instruction, view);
    }
    public override string Format(Instruction instruction, ushort address)
    {
        if (instruction.GetBit(5))
        {
            return $"ADD R{instruction.DR} R{instruction.SR1} {Utils.SignExtend16(instruction.Imm5, 5)}";
        } 
        else
        {
            return $"ADD R{instruction.DR} R{instruction.SR1} R{instruction.SR2}";
        }
    }
    public override Instruction Parse(ref ArgumentReader reader, ushort address)
    {
        Instruction result = new()
        {
            OpCode = OpCode.ADD,
            DR = reader.NextRegister(),
            SR1 = reader.NextRegister()
        };

        try
        {
            var tempReader = reader;
            result.Imm5 = tempReader.NextLiteral(true);
            result.SetBit(5, true);
            reader = tempReader;
        }
        catch
        {
            result.SR2 = reader.NextRegister();
            result.SetBit(5, false);
        }
        return result;
    }
}
