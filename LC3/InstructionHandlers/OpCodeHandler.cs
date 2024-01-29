using LC3.Assembly;
using LC3.Views;

namespace LC3.InstructionHandlers;

abstract class OpCodeHandler
{
    public abstract void Process(Instruction instruction, Machine machine);
    public virtual void Layout(ref Instruction instruction, QuickAssemblerView view)
    {
    }

    public virtual string Format(Instruction instruction, ushort address)
    {
        return instruction.OpCode.ToString() + " ???";
    }

    public virtual Instruction Parse(ref ArgumentReader reader, ushort address)
    {
        return default;
    }
}
