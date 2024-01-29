using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class JSRHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        machine.GeneralRegisters.R7 = machine.SpecialRegisters.PC;

        if (instruction.GetBit(11))
        {
            machine.SpecialRegisters.PC = (ushort)(machine.SpecialRegisters.PC + Utils.SignExtend16(instruction.PCOffset11, 11));
        }
        else
        {
            machine.SpecialRegisters.PC = (ushort)instruction.BaseR;
        }
    }

    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.SetBit(11, view.PickBit("JSRR", instruction.GetBit(11)));

        if (instruction.GetBit(11))
        {
            instruction.PCOffset11 = view.PickSigned("PCOffset11", instruction.PCOffset11, 11);
        }
        else
        {
            instruction.BaseR = view.PickRegister("BaseR", instruction.BaseR);
        }

        base.Layout(ref instruction, view);
    }

    public override string Format(Instruction instruction, ushort address)
    {
        if (instruction.GetBit(11))
        {
            return $"JSR 0x{address + Utils.SignExtend16(instruction.PCOffset11, 11):x4}";
        }
        else
        {
            return $"JSRR R{instruction.BaseR}";
        }
    }
}

internal static class Utils
{
    public static short SignExtend16(int value, int bits)
    {
        int mask = 1 << (bits - 1);
        return (short)((value ^ mask) - mask);
    }
}
