using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class LEAHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ushort address = machine.ReadU16((ushort)(machine.SpecialRegisters.PC + instruction.PCOffset9));
        machine.GeneralRegisters[instruction.DR] = address;
    }
}
