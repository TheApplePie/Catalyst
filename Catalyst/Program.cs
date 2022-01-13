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
        private static bool running = true;
        public static void Main(string[] args)
        {
            Game g = new Game();
            g.Window = new GameWindow(new GameWindowCreateInfo(1000, 1000, "test"));
            
            while (running)
            {
                Console.WriteLine(GLFW.TimerValue);
                GLFW.PollEvents();
            }
        }
    }
}