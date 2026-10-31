using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public class ColorPickerPopup
{
    static readonly Dictionary<string, ColorPicker.PickerShapeType> Shapes = new()
    {
        ["hsv_rect"] = ColorPicker.PickerShapeType.HsvRectangle,
        ["hsv_wheel"] = ColorPicker.PickerShapeType.HsvWheel,
        ["vhs_circle"] = ColorPicker.PickerShapeType.VhsCircle,
        ["okhsl_circle"] = ColorPicker.PickerShapeType.OkhslCircle,
    };

    readonly PopupPanel popup = new();
    readonly ColorPicker wheel = new();
    readonly LineEdit hex = new();
    readonly Color defaultColor;

    public Node Node => popup;
    public event Action<string> ColorCommitted;

    public ColorPickerPopup(string shape, string defaultHex, ScreenData screen)
    {
        if (!Shapes.TryGetValue(shape, out var id))
            throw new InvalidOperationException(
                $"Unknown picker shape: {shape} (shapes: {string.Join(", ", Shapes.Keys)})");
        defaultColor = new Color(defaultHex);

        wheel.PickerShape = id;
        wheel.SamplerVisible = false;
        wheel.SlidersVisible = false;
        wheel.ColorModesVisible = false;
        wheel.PresetsVisible = false;
        wheel.CanAddSwatches = false;
        wheel.HexVisible = false; // ColorPicker has its own hex field; only ours remains
        wheel.ColorChanged += OnWheelChanged;

        hex.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        hex.TextSubmitted += _ => Commit();

        var box = new VBoxContainer();
        box.AddThemeConstantOverride("separation", screen.Theme.Gutter);
        box.AddChild(wheel);
        box.AddChild(hex);
        box.AddChild(ButtonsRow(screen));
        popup.AddChild(box);
    }

    HBoxContainer ButtonsRow(ScreenData screen)
    {
        var apply = new Button { Text = Loc.Instance.Tr("ui.character_create.apply") };
        apply.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        apply.Pressed += Commit;
        var cancel = new Button { Text = Loc.Instance.Tr("ui.character_create.cancel") };
        cancel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        cancel.Pressed += popup.Hide;

        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", screen.Theme.Gutter);
        row.AddChild(apply);
        row.AddChild(cancel);
        return row;
    }

    public void Open(string text)
    {
        wheel.Color = text.Length > 0 ? new Color(text) : defaultColor;
        ShowHex(wheel.Color);
        popup.PopupCentered();
    }

    void Commit()
    {
        var code = hex.Text.TrimStart('#');
        if (!IsHex(code))
        {
            ShowHex(wheel.Color); // invalid input snaps the field back to the wheel color
            return;
        }
        ColorCommitted?.Invoke($"#{new Color(code).ToHtml(true)}");
        popup.Hide();
    }

    void OnWheelChanged(Color color) => ShowHex(color);

    void ShowHex(Color color) => hex.Text = $"#{color.ToHtml(true)}";

    static bool IsHex(string text)
    {
        if (text.Length != 6 && text.Length != 8) return false;
        foreach (var c in text)
            if (!Uri.IsHexDigit(c)) return false;
        return true;
    }
}
