using System.Collections.Generic;
using Catalyst.Windowing;
using GLFW3;
using Vulkan;

namespace Catalyst
{
    /* TODO:
     * Support Multiple Renderer Contexts & Windows
     * 
     */
    
    public class Game
    {
        public GameWindow Windows;

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

        public void Start()
        {
            while (State != GameState.Exiting)
            {
                switch (State)
                {
                    
                }
                
                GLFW.PollEvents();
            }
        }
    }

    public enum GameState
    {
        Running,
        Exiting,
    }
}