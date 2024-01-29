using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class NOTHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ref ushort dr = ref machine.GeneralRegisters[instruction.DR];
        ref ushort sr = ref machine.GeneralRegisters[instruction.SR1];
        dr = (ushort)~sr;
        machine.SpecialRegisters.SetConditions((short)dr);
    }
    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.DR = view.PickRegister("DR", instruction.DR);
        instruction.SR1 = view.PickRegister("SR", instruction.SR1);

        base.Layout(ref instruction, view);
    }
}
