﻿using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class LDIHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ushort addrLocation = (ushort)(machine.SpecialRegisters.PC + Utils.SignExtend16(instruction.PCOffset9, 9));
        ushort addr = machine.ReadU16(addrLocation);
        ushort result = machine.ReadU16(addr);
        machine.GeneralRegisters[instruction.DR] = result;
        machine.SpecialRegisters.SetConditions((short)result);
    }

    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.DR = view.PickRegister("DR", instruction.DR);
        instruction.PCOffset9 = view.PickSigned("PCOffset9", instruction.PCOffset9, 9);
        base.Layout(ref instruction, view);
    }
}
