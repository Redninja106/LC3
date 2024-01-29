using LC3.Views;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class JMPHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
        int baseR = instruction.BaseR;

        if (baseR == 0b111)
        {
            machine.SpecialRegisters.PC = machine.GeneralRegisters.R7;
        }
        else
        {
            machine.SpecialRegisters.PC = machine.GeneralRegisters[baseR];
        }
    }

    public override void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
        instruction.BaseR = view.PickRegister("BaseR", instruction.BaseR);
        base.Layout(ref instruction, view);
    }

    public override string Format(Instruction instruction, ushort address)
    {
        if (instruction.BaseR == 0b111)
        {
            return "RET";
        }
        else
        {
            return $"JMP R{instruction.BaseR}";
        }
    }
}
