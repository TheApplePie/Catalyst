using Vulkan;
using Vulkan.Khr;
using GLFW3;

namespace RPG.Rendering
{
    public struct RenderContextCreateInfo
    {
        public Instance Instance;
        //Window
        public Window Window;

        //Image

        //Swapchain
        public Format Format;
        public PresentModeKhr PresentMode; //VSYNC ON = PresentModeKhr.Mailbox, VSYNC OFF = PresentModeKhr.Immediate

        public RenderContextCreateInfo(Instance instance, Window window, Format format = Format.B8G8R8A8UNorm, PresentModeKhr presentMode = PresentModeKhr.Mailbox)
        {
            Instance = instance;
            Window = window;
            Format = format;
            PresentMode = presentMode;
        }
    }
}