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
        public static void Main(string[] args)
        {
            GLFW.Init();
            Window w = GLFW.CreateWindow(640, 480, "", Monitor.None, Window.None);
        }
    }
}