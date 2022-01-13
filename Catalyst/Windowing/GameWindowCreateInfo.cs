namespace Catalyst.Windowing
{
    public struct GameWindowCreateInfo
    {
        public int Width, Height;
        public string Title;

        public GameWindowCreateInfo(int width, int height, string title)
        {
            Width = width;
            Height = height;
            Title = title;
        }
    }
}