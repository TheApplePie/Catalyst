using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using GLFW3;
using Catalyst.Rendering;

namespace Catalyst.Windowing
{
    public class GameWindow
    {
        public Window Window;
        //public RendererInstance Renderer;

        public GameWindow(GameWindowCreateInfo info)
        {
            Window = GLFW.CreateWindow(info.Width, info.Height, info.Title, Monitor.None, Window.None);

            GLFW.SetCloseCallback(Window, window => Close());
        }

        public void Close()
        {
            GLFW.DestroyWindow(Window);
        }

        public void Show() => GLFW.ShowWindow(Window);
        public void Hide() => GLFW.HideWindow(Window);
        public void SetWindowSize(int width, int height) { GLFW.SetWindowSize(Window, width, height); }
        public Vector2 GetWindowSize()
        {
            GLFW.GetWindowSize(Window, out int width, out int height);
            return new Vector2(width, height);
        }
    }
}
