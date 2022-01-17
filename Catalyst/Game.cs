using System.Collections.Generic;
using Catalyst.Windowing;
using GLFW3;
using Vulkan;
using RPG.Rendering;

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
        public RenderContext Context;
        public RenderPipeline Pipeline;
        
        private RendererCreateInfo _rendererCreateInfo;
        
        static Game()
        {
            GLFW.WindowHint(Hint.ClientApi, ClientApi.None);
            GLFW.WindowHint(Hint.Decorated, true);
            GLFW.Init();
        }
        
        public Game()
        {
            State = GameState.Running;
        }

        public void Start()
        {
            AppInfo = new ApplicationInfo(GameName, Version);
            
            Window = new GameWindow(new GameWindowCreateInfo(1000,1000,"test"));

            _rendererCreateInfo = new RendererCreateInfo(AppInfo, Window.Window, false, false);
            Renderer = new Renderer(_rendererCreateInfo);

            Context = Renderer.CreateContext();

            State = GameState.Running;
            
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
            while (!GLFW.WindowShouldClose(Window.Window))
            {
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