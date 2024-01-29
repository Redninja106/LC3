using LC3.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class BRHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        bool n = instruction.GetBit(11);
        bool z = instruction.GetBit(10);
        bool p = instruction.GetBit(09);

        bool N = machine.SpecialRegisters.N;
        bool Z = machine.SpecialRegisters.Z;
        bool P = machine.SpecialRegisters.P;

        if ((n && N) || (z && Z) || (p && P))
        {
            machine.SpecialRegisters.PC = (ushort)(machine.SpecialRegisters.PC + Utils.SignExtend16(instruction.PCOffset9, 9));
        }
    }
    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.SetBit(11, view.PickBit("n", instruction.GetBit(11)));
        instruction.SetBit(10, view.PickBit("z", instruction.GetBit(10)));
        instruction.SetBit(9, view.PickBit("p", instruction.GetBit(9)));
        instruction.PCOffset9 = view.PickSigned("PCOffset9", instruction.PCOffset9, 9);
    }
}
