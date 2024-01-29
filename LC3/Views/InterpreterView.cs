using ImGuiNET;
using SimulationFramework.Input;

namespace LC3.Views;
internal class InterpreterView(Machine machine, Interpreter interpreter) : View(machine, interpreter)
{
    bool running;
    int instructions;

    protected override void OnLayout()
    {
        if (ImGui.Button("continue"))
        {
            running = true;
        }
        if (ImGui.Button("break"))
        {
            running = false;
        }
        if (ImGui.Button("step") || running)
        {
            interpreter.Cycle();
            instructions++;
        }
    }
}