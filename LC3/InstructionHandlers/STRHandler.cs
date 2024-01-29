using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class STRHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        ref ushort sr = ref machine.GeneralRegisters[instruction.DR];
        ref ushort baseR = ref machine.GeneralRegisters[instruction.BaseR];
        ushort address = (ushort)(baseR + Utils.SignExtend16(instruction.Offset6, 6));
        machine.WriteU16(address, sr);
    }

    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.DR = view.PickRegister("SR", instruction.DR);
        instruction.BaseR = view.PickRegister("BaseR", instruction.BaseR);
        instruction.Offset6 = view.PickSigned("Offset6", instruction.Offset6, 6);
        base.Layout(ref instruction, view);
    }
}
