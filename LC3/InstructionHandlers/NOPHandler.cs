using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.InstructionHandlers;
internal class NOPHandler : OpCodeHandler
{
    public override void Process(Instruction instruction, Machine machine)
    {
    }
}
