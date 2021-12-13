namespace Catalyst.ImGui
{
    [System.Flags]
    public enum ImGuiTreeNodeFlags
    {
        None = 0,
        Selected = 1,
        Framed = 2,
        AllowItemOverlap = 4,
        NoTreePushOnOpen = 8,
        NoAutoOpenOnLog = 16,
        DefaultOpen = 32,
        OpenOnDoubleClick = 64,
        OpenOnArrow = 128,
        Leaf = 256,
        Bullet = 512,
        FramePadding = 1024,
        SpanAvailWidth = 2048,
        SpanFullWidth = 4096,
        NavLeftJumpsBackHere = 8192,
        CollapsingHeader = 26,
    }
}
