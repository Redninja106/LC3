using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LC3.Views;
internal abstract class View(Machine machine, Interpreter interpreter)
{
    public virtual ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;

    public virtual string Title => GetType().Name;
    private bool open = true;
    private bool viewNumbersAsHex = true;

    public bool Open { get => open; set => open = value; }
    public Machine machine = machine;
    public Interpreter interpreter = interpreter;

    protected abstract void OnLayout();

    public void Layout()
    {
        if (open && ImGui.Begin(Title, ref open, WindowFlags))
        {
            OnLayout();
        }
        ImGui.End();
    }

    public bool HexInput<T>(string label, ref T value) where T : IBinaryInteger<T>
    {
        string s = this.viewNumbersAsHex ? ("0x" + value.ToString("x4", null)) : value.ToString()!;
        ImGui.PushID(label);
        ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(-6, -2));
        bool result = ImGui.InputText(label, ref s, 6, ImGuiInputTextFlags.EnterReturnsTrue);
        if (result)
        {
            if (TryParseNumber(s, out T? parsed))
            {
                value = parsed;
            }
        }
        if (ImGui.IsItemHovered() && ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            ImGui.OpenPopup("hex input");
        }
        if (ImGui.BeginPopup("hex input"))
        {
            if (ImGui.Button("copy", new(50, 0)))
            {
                ImGui.SetClipboardText("0x" + value.ToString("x4", null));
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button("paste", new(50, 0)))
            {
                var text = ImGui.GetClipboardText();
                if (TryParseNumber(text, out T? parsed))
                {
                    value = parsed;
                    result = true;
                }
                ImGui.CloseCurrentPopup();
            }

            ImGui.Text("view as:");
            if (ImGui.Button("decimal"))
            {
                viewNumbersAsHex = false;
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button("hex"))
            {
                viewNumbersAsHex = true;
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
        ImGui.PopID();
        return result;
    }

    private static bool TryParseNumber<T>(string value, [NotNullWhen(true)] out T? result) where T : IBinaryInteger<T>
    {
        result = default;
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        bool hex = false;
        if (value.StartsWith("0x"))
        {
            value = value[2..];
            hex = true;
        }
        if (value.StartsWith('x'))
        {
            value = value[1..];
            hex = true;
        }

        if (T.TryParse(value, hex ? NumberStyles.HexNumber : 0, null, out T? parsedValue))
        {
            result = parsedValue;
            return true;
        }
        return false;
    }

    public bool HexInput8(string label, ref byte value)
    {
        string s = value.ToString("x2");

        if (ImGui.InputText(label, ref s, 2, ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (string.IsNullOrEmpty(s))
            {
                value = 0;
                return true;
            }
            value = byte.Parse(s, NumberStyles.HexNumber);
            return true;
        }
        return false;
    }
}
