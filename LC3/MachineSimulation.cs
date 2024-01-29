using ImGuiNET;
using LC3.Views;
using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Input;

namespace LC3;

internal class MachineSimulation : Simulation
{
    public Machine machine;
    public Interpreter interpreter;

    private readonly List<View> views = new();

    public MachineSimulation()
    {
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        machine = new(1024 * 64);
        machine.SpecialRegisters.PC = 0x3000;
        interpreter = new(machine);

        views.Add(new RegistersView(machine, interpreter));
        views.Add(new InterpreterView(machine, interpreter));
        views.Add(new MemoryView(machine, interpreter));
        views.Add(new QuickAssemblerView(machine, interpreter));
        views.Add(new ExecutionHistoryView(machine, interpreter));
        views.Add(new SourceView(machine, interpreter));
    }

    public override void OnInitialize()
    {
        Window.Title = "LC3 Emulator";
    }

    public override void OnRender(ICanvas canvas)
    {
        canvas.Clear(Color.Black);

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Views"))
            {
                foreach (var view in views)
                {
                    if (ImGui.Selectable(view.Title, view.Open))
                    {
                        view.Open = !view.Open;
                    }
                }
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        foreach (var view in views)
        {
            view.Layout();
        }
    }
}