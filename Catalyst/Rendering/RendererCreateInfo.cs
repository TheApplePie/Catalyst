using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

using GLFW3;
using VK = Vulkan;
using Vulkan.Khr;
using Vulkan.Ext;

namespace RPG.Rendering
{
    public struct RendererCreateInfo
    {
        public VK.ApplicationInfo AppInfo;
        public Window Window;
        
        public string[] RequiredExtensions;
        public string[] RequiredLayers;

        public bool AllocCallbacks;
        public bool DebugCallback;

        public RendererCreateInfo(VK.ApplicationInfo appInfo, Window window, bool useAllocCallbacks = false, bool useDebugCallback = false)
        {
            AppInfo = appInfo;
            Window = window;
            
#if DEBUG
            RequiredLayers = new string[] {"VK_LAYER_KHRONOS_validation"};
            RequiredExtensions = new string[] {"VK_EXT_debug_utils", "VK_EXT_debug_report"};
#else
            RequiredLayers = new string[] { };
            RequiredExtensions = new string[] { };
#endif

            AllocCallbacks = useAllocCallbacks;
            DebugCallback = useDebugCallback;
        }
        
        public RendererCreateInfo(VK.ApplicationInfo appInfo, Window window, string[] requiredLayers, string[] requiredExtensions, bool useAllocCallbacks = false, bool useDebugCallback = false)
        {
            AppInfo = appInfo;
            Window = window;
            
            RequiredLayers = requiredLayers;
            RequiredExtensions = requiredExtensions;
            
            AllocCallbacks = useAllocCallbacks;
            DebugCallback = useDebugCallback;
        }
    }
}