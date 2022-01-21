using System;
using System.Collections.Generic;
using System.Text;

using GLFW3;
using Vulkan;
using ImGui;

using Catalyst.Windowing;

namespace Catalyst
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game g = new Game();
            g.Start();
            
            GLFW.Terminate();
        }
    }
}