using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using GLFW3;

namespace Catalyst.Windowing
{
    public class GameWindow : IDisposable
    {
        public Window Window;

        public GameWindow(GameWindowCreateInfo info)
        {
            Window = GLFW.CreateWindow(info.Width, info.Height, info.Title, Monitor.None, Window.None);
            GLFW.SetCloseCallback(Window, window => Dispose());
        }

        public void Dispose()
        {
            GLFW.DestroyWindow(Window);
        }

        public Vector2 GetWindowSize()
        {
            GLFW.GetWindowSize(Window, out int width, out int height);
            return new Vector2(width, height);
        }
        
        public void Show() => GLFW.ShowWindow(Window);
        public void Hide() => GLFW.HideWindow(Window);
        
        public void SetWindowSize(int width, int height) { GLFW.SetWindowSize(Window, width, height); }
    }
}
