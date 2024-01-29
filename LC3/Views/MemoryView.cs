using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Views;
internal class MemoryView(Machine machine, Interpreter interpreter) : View(machine, interpreter)
{
    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.MenuBar;

    private ushort location = 0x3000;
    private float scroll;

    protected override void OnLayout()
    {
        int scrollDelta = 0;

        if (ImGui.IsWindowHovered())
        {
            scroll -= ImGui.GetIO().MouseWheel;

            while (scroll > 1)
            {
                scrollDelta++;
                scroll--;
            }
            while (scroll < -1)
            {
                scrollDelta--;
                scroll++;
            }
        }
        float lineHeight = ImGui.GetTextLineHeight() + 6;

        ImGui.SetCursorPosY((2-scroll) * lineHeight);

        int baseAddress = location + scrollDelta;
        baseAddress = int.Clamp(baseAddress, 0, ushort.MaxValue - 1);
        location = (ushort)baseAddress;

        if (ImGui.BeginMenuBar())
        {
            ImGui.Text($"address: ");
            HexInput("", ref location);
            ImGui.EndMenuBar();
        }

        ImGui.Columns(3);
        for (int i = 0; i < 64; i++)
        {
            int address32 = baseAddress + i;

            if (address32 < 0 || address32 > ushort.MaxValue)
            {
                continue;
            }

            ushort address = (ushort)address32;

            ImGui.PushID(address);
            ImGui.Text("0x" + address.ToString("x4"));
            ImGui.NextColumn();
            ushort value = machine.ReadU16(address);
            if (HexInput("", ref value))
            {
                machine.WriteU16(address, value);
            }
            ImGui.NextColumn();

            if ((value & 0x00FF) == value)
            {
                ImGui.Text(((char)value).ToString());
            }
            else 
            {
                Instruction instruction = new(value);
                var handler = Interpreter.GetOpCodeHandler(instruction.OpCode);
                ImGui.Text(handler.Format(instruction, address).AsSpan());
            }

            ImGui.NextColumn();

            ImGui.PopID();

        }
    }

}