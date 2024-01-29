using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Views;
internal class ExecutionHistoryView(Machine machine, Interpreter interpreter) : View(machine, interpreter)
{
    private ushort lastPC = machine.SpecialRegisters.PC;
    public Queue<string> Lines { get; } = new();
    public int MaxLines { get; } = 64;
    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.MenuBar;

    protected override void OnLayout()
    {
        ushort pc = machine.SpecialRegisters.PC;

        if (ImGui.BeginMenuBar())
        {
            if (ImGui.Button("clear"))
            {
                Lines.Clear();
            }
            ImGui.EndMenuBar();
        }

        if (lastPC != pc)
        {
            Lines.Enqueue(interpreter.FormatInstruction(new(machine.SpecialRegisters.IR), pc));
            ImGui.SetScrollY(ImGui.GetScrollMaxY());
            lastPC = pc;
        }

        if (Lines.Count > MaxLines)
        {
            Lines.Dequeue();
        }
        foreach (var line in Lines)
        {
            ImGui.Text(line);
        }
    }
}
