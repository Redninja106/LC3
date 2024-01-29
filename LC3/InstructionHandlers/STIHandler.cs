using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class STIHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ushort addrLocation = (ushort)(machine.SpecialRegisters.PC + Utils.SignExtend16(instruction.PCOffset9, 9));
        ushort addr = machine.ReadU16(addrLocation);
        machine.WriteU16(addr, machine.GeneralRegisters[instruction.DR]);
    }

    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.DR = view.PickRegister("SR", instruction.DR);
        instruction.PCOffset9 = view.PickSigned("PCOffset9", instruction.PCOffset9, 9);
        base.Layout(ref instruction, view);
    }
}
