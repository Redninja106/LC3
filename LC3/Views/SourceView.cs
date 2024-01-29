using ImGuiNET;
using LC3.Assembly;
using SimulationFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Views;
internal class SourceView : View
{
    List<SourceFile> openFiles = [];
    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.MenuBar;
    private string newFileName = "";
    private SourceFile? activeFile;

    private string openFileDir; 

    public SourceView(Machine machine, Interpreter interpreter) : base(machine, interpreter)
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        openFileDir = Path.Combine(documentsPath, "LC3 Programs");
        if (!Directory.Exists(openFileDir))
            Directory.CreateDirectory(openFileDir);
    }

    private void AddSourceFile(string name, string source, bool saved)
    {
        openFiles.Add(new(name, source, Path.Combine(openFileDir, name), saved));
    }
    
    protected override void OnLayout()
    {
        if (ImGui.BeginMenuBar())
        {
            LayoutMenuBar();
            ImGui.EndMenuBar();
        }

        if (ImGui.BeginTabBar("tab bar"))
        {
            SourceFile? deleteFile = null;
            foreach (var file in openFiles)
            {
                bool open = true;
                if (ImGui.BeginTabItem(file.Name + "##" + file.GetHashCode(), ref open, file.Saved ? 0 : ImGuiTabItemFlags.UnsavedDocument))
                {
                    activeFile = file;
                    if (ImGui.InputTextMultiline("##input", ref file.Source, (uint)(file.Source.Length + 1024), new(-2, -2), ImGuiInputTextFlags.AllowTabInput))
                    {
                        file.Saved = false;
                    }
                    ImGui.EndTabItem();
                }
                if (!open)
                    deleteFile = file;
            }
            if (deleteFile != null)
                openFiles.Remove(deleteFile);

            ImGui.EndTabBar();
        }
    }

    private void LayoutMenuBar()
    {
        string? menuAction = null;
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("New..."))
                menuAction = "new";
            if (ImGui.MenuItem("Save..."))
                menuAction = "save";
            if (ImGui.MenuItem("Save As..."))
                menuAction = "save as";
            if (ImGui.MenuItem("Open..."))
                menuAction = "open";
            ImGui.EndMenu();
        }
        if (ImGui.IsKeyDown(ImGuiKey.ModCtrl) && Keyboard.IsKeyPressed(Key.S))
        {
            menuAction = "save";
        }
        switch (menuAction)
        {
            case "new":
                ImGui.OpenPopup("New source file", ImGuiPopupFlags.AnyPopupId);
                break;
            case "save":
                try
                {
                    File.WriteAllText(activeFile!.FullPath, activeFile.Source);
                    activeFile.Saved = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                break;
            case "open":
                ImGui.OpenPopup("Open source file", ImGuiPopupFlags.AnyPopupId);
                break;
            default:
                break;
        }
        if (ImGui.BeginPopupModal("New source file"))
        {
            ImGui.Text("name:");
            ImGui.SameLine();
            ImGui.InputText("##name", ref newFileName, 64);
            if (ImGui.Button("cancel"))
            {
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button("create") && !string.IsNullOrWhiteSpace(newFileName))
            {
                openFiles.Add(new(newFileName, "", Path.Combine(openFileDir, newFileName), false));
                newFileName = "";
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
        if (ImGui.BeginPopupModal("Open source file"))
        {
            if (ImGui.InputText("##path", ref openFileDir, 256))
            {
                if (!string.IsNullOrEmpty(openFileDir))
                {
                    openFileDir = Path.GetFullPath(openFileDir);
                }
            }
            if (ImGui.BeginListBox("##listbox", new(-2, -25)) && !string.IsNullOrEmpty(openFileDir))
            {
                if (ImGui.Selectable(".."))
                {
                    var parent = Directory.GetParent(openFileDir);
                    openFileDir = parent!.FullName;
                }

                foreach (var dir in Directory.GetDirectories(openFileDir))
                {
                    if (ImGui.Selectable(Path.GetFileName(dir)))
                    {
                        openFileDir = dir;
                    }
                }

                foreach (var file in Directory.GetFiles(openFileDir))
                {
                    if (!file.EndsWith(".c") && !file.EndsWith(".s") && !file.EndsWith(".hex"))
                    {
                        continue;
                    }

                    if (ImGui.Selectable(Path.GetFileName(file)))
                    {
                        try
                        {
                            openFiles.Add(new(Path.GetFileName(file), File.ReadAllText(file), file, true));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.PushID(file);
                    if (ImGui.IsItemHovered() && ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        ImGui.OpenPopup("menu");
                    }
                    if (ImGui.BeginPopup("menu"))
                    {
                        if (ImGui.Selectable("delete"))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        ImGui.EndPopup();
                    }
                    ImGui.PopID();

                    


                }
                ImGui.EndListBox();
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }
            }

            ImGui.EndPopup();
        }

        string? fileExt = Path.GetExtension(activeFile?.Name);
        if (fileExt is ".c" or ".h") 
        {
            if (ImGui.Button("Compile")) 
            { 
            }
        }
        if (fileExt is ".s" or ".asm")
        {
            if (ImGui.Button("Assemble"))
            {
                try
                {
                    var file = Assemble(activeFile!.Source);

                    var existingFile = openFiles.FirstOrDefault(o => o.FullPath == activeFile!.FullPath + ".hex");
                    if (existingFile != null)
                    {
                        existingFile.Source = file.ToSource();
                    }
                    else
                    {
                        openFiles.Add(new(activeFile!.Name + ".hex", file.ToSource(), activeFile!.FullPath + ".hex", false));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            if (ImGui.Button("Load Into Machine"))
            {
                try 
                { 
                    LoadToMachine(Assemble(activeFile!.Source));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        if (fileExt is ".x" or ".hex") 
        {
            if (ImGui.Button("Load Into Machine"))
            {
                try 
                {
                    LoadToMachine(HexFile.FromSource(activeFile!.Source));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    private HexFile Assemble(string source)
    {   
        var syntaxTree = AssemblySyntaxTree.FromSource(source);
        HexFile file = AssemblyCompiler.Compile(syntaxTree);
        return file;
    }

    private void LoadToMachine(HexFile file)
    {
        try
        {
            for (int i = 0; i < file.Values.Length; i++)
            {
                machine.WriteU16((ushort)(file.StartAddress + i), file.Values[i]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

class SourceFile
{
    public string Name;
    public string Source;
    public string FullPath;
    public bool Saved;
    public SourceFile(string name, string source, string fullPath, bool saved)
    {
        Name = name;
        Source = source;
        Saved = saved;
        FullPath = fullPath;
    }
}