using System;
using System.Collections.Generic;
using Catalyst.Windowing;
using GLFW3;
using Vulkan;
using Catalyst.Rendering;
using Version = Vulkan.Version;

namespace Catalyst
{
    /* TODO:
     * Support Multiple Renderer Contexts & Windows
     * 
     */
    
    public class Game
    {
        public GameWindow Window;

        public GameState State;
        
        public string GameName = "RPG";
        public Version Version = new Version(0, 0, 1);

        public ApplicationInfo AppInfo;

        public Renderer Renderer;

        static Game()
        {
            GLFW.WindowHint(Hint.ClientApi, ClientApi.None);
            GLFW.WindowHint(Hint.Decorated, true);
            GLFW.Init();
        }
        
        public Game()
        {
            State = GameState.Running; //test
        }

        public void Start()
        {
            AppInfo = new ApplicationInfo(GameName, Version);
            
            Window = new GameWindow(new GameWindowCreateInfo(1000,1000,"test"));

            State = GameState.Running;

            Renderer = new Renderer(Window.Window);
            
            //LOAD RESOURCES
            
            Window.Show();
            while (State != GameState.Exiting)
            {
                switch (State)
                {
                    
                }
                
                Update();
            }
        }
        
        public void Tick() //Happens X times a second
        {
        }

        public void Update() //Happens every frame
        {
            GLFW.PollEvents();
        }
    }

    public enum GameState
    {
        Running,
        Exiting,
    }
}
