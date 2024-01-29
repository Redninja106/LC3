using LC3.InstructionHandlers;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3;
internal class Interpreter
{
    private static readonly OpCodeHandler[] opCodeHandlers = new OpCodeHandler[16];
    
    public Machine machine;
    public bool isInBreak;

    static Interpreter()
    {
        opCodeHandlers[(int)OpCode.ADD] = new AddHandler();
        opCodeHandlers[(int)OpCode.AND] = new AndHandler();
        opCodeHandlers[(int)OpCode.BR] = new BRHandler();
        opCodeHandlers[(int)OpCode.JMP] = new JMPHandler();
        opCodeHandlers[(int)OpCode.JSR] = new JSRHandler();
        opCodeHandlers[(int)OpCode.LD] = new LDHandler();
        opCodeHandlers[(int)OpCode.LDI] = new LDIHandler();
        opCodeHandlers[(int)OpCode.LDR] = new LDRHandler();
        opCodeHandlers[(int)OpCode.LEA] = new LEAHandler();
        opCodeHandlers[(int)OpCode.NOT] = new NOTHandler();
        opCodeHandlers[(int)OpCode.RTI] = new RTIHandler();
        opCodeHandlers[(int)OpCode.ST] = new STHandler();
        opCodeHandlers[(int)OpCode.STI] = new STIHandler();
        opCodeHandlers[(int)OpCode.STR] = new STRHandler();
        opCodeHandlers[(int)OpCode.TRAP] = new TRAPHandler();
    }

    public Interpreter(Machine machine)
    {
        this.machine = machine;

    }

    public static OpCodeHandler GetOpCodeHandler(OpCode opCode)
    {
        return opCodeHandlers[(int)opCode];
    }

    public void Cycle()
    {
        machine.SpecialRegisters.IR = machine.ReadU16(machine.SpecialRegisters.PC);
        machine.SpecialRegisters.PC++;

        Instruction nextInstruction = new(machine.SpecialRegisters.IR);
        var handler = opCodeHandlers[(int)nextInstruction.OpCode];
        handler.Process(nextInstruction, machine);
    }

    public string FormatInstruction(Instruction instruction, ushort address)
    {
        return GetOpCodeHandler(instruction.OpCode).Format(instruction, address);
    }
}
