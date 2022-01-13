using System.Collections.Generic;
using Catalyst.Windowing;
using GLFW3;
using Vulkan;

namespace Catalyst
{
    public class Game
    {
        public Window[] Windows;

        public GameState State;
        
        static Game()
        {
            GLFW.WindowHint(Hint.ClientApi, ClientApi.None);
            GLFW.Init();
        }
        
        public Game()
        {
            State = GameState.Running;
        }
    }

    public enum GameState
    {
        Running,
        Exiting,
    }
}