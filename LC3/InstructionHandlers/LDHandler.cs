using LC3.Assembly;
using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class LDHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ref ushort dr = ref machine.GeneralRegisters[instruction.DR];
        dr = machine.Memory[machine.SpecialRegisters.PC + Utils.SignExtend16(instruction.PCOffset9, 9)];
        machine.SpecialRegisters.SetConditions((short)dr);
    }

    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.DR = view.PickRegister("DR", instruction.DR);
        instruction.PCOffset9 = view.PickSigned("PCOffset9", instruction.PCOffset9, 9);
        base.Layout(ref instruction, view);
    }
    public override string Format(Instruction instruction, ushort address)
    {
        return $"LD R{instruction.DR} 0x{address + Utils.SignExtend16(instruction.PCOffset9, 9):x4}";
    }
    public override Instruction Parse(ref ArgumentReader reader, ushort address)
    {
        return new Instruction
        {
            OpCode = OpCode.LD,
            DR = reader.NextRegister(),
            PCOffset9 = reader.NextAddress() - address,
        };
    }
}
