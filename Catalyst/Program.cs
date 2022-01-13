using System;
using System.Collections.Generic;
using System.Text;

using GLFW3;
using Vulkan;
using ImGui;

namespace Catalyst
{
    public class Program
    {
        private static bool running = true;
        public static void Main(string[] args)
        {
            GLFW.Init();
            Window w = GLFW.CreateWindow(640, 480, "", Monitor.None, Window.None);
            GLFW.SetCloseCallback(w, window => { running = false; });
            while (running)
            {
                GLFW.PollEvents();
            }
        }
    }
}