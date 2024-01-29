using ImGuiNET;

namespace LC3.Views;
internal class RegistersView(Machine machine, Interpreter interpreter) : View(machine, interpreter)
{
    protected override void OnLayout()
    {
        ImGui.Columns(2);
        for (int i = 0; i < 8; i++)
        {
            ImGui.PushID(i);
            ImGui.Text($"r{i}:");
            ImGui.NextColumn();
            HexInput("", ref machine.GeneralRegisters[i]);
            ImGui.NextColumn();
            ImGui.PopID();
        }

        ImGui.Separator();

        ImGui.Text($"PC:");
        ImGui.NextColumn();
        ushort pc = machine.SpecialRegisters.PC;
        HexInput("", ref pc);
        machine.SpecialRegisters.PC = pc;
        ImGui.NextColumn();

        machine.SpecialRegisters.P = LayoutCheckmark($"P:", machine.SpecialRegisters.P);
        machine.SpecialRegisters.Z = LayoutCheckmark($"Z:", machine.SpecialRegisters.Z);
        machine.SpecialRegisters.N = LayoutCheckmark($"N:", machine.SpecialRegisters.N);
    }

    private bool LayoutCheckmark(string label, bool value)
    {
        ImGui.PushID(label);
        ImGui.Text(label);
        ImGui.NextColumn();
        ImGui.Checkbox("", ref value);
        ImGui.NextColumn();
        ImGui.PopID();
        return value;
    }
}