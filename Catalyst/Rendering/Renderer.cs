using System;
using System.Collections.Generic;
using System.Linq;

using GLFW3;
using VK = Vulkan;
using Vulkan.Khr;
using Vulkan.Ext;

namespace RPG.Rendering
{ 
    public class Renderer
    {
        public VK.Instance Instance;
        public Window Window;
        
        public VK.ExtensionProperties[] Extensions;
        public VK.LayerProperties[] Layers;

        private DebugReportCallbackExt _debugReportCallback;

        public Renderer(RendererCreateInfo rendererInfo)
        {
            if (!GLFW3.Vulkan.IsSupported)
                throw new System.Exception("Vulkan is not supported");
            
            Window = rendererInfo.Window;

            string[] requiredExtensions = rendererInfo.RequiredExtensions.Concat(GLFW3.Vulkan.GetRequiredInstanceExtensions()).ToArray();
            string[] requiredLayers = rendererInfo.RequiredLayers;
            
            VK.LayerProperties[] availableLayers = VK.Instance.EnumerateLayerProperties();
            VK.ExtensionProperties[] availableExtensions = VK.Instance.EnumerateExtensionProperties();

            foreach (string layer in requiredLayers)
                if (availableLayers.All(i => i.LayerName != layer))
                    throw new System.Exception($"Required layer: {layer} not found");

            foreach (string extension in requiredExtensions)
                if (availableExtensions.All(i => i.ExtensionName != extension))
                    throw new System.Exception($"Required Extension: {extension} not found");
            
            //EnabledLayers = availableLayers;
            //EnabledExtensions = availableExtentions;
            
            VK.InstanceCreateInfo instanceCreateInfo = new VK.InstanceCreateInfo(rendererInfo.AppInfo, requiredLayers, requiredExtensions);

            //if (rendererInfo.AllocCallbacks)
            //    Instance = new VK.Instance(instanceCreateInfo,
            //        new VK.AllocationCallbacks(
            //            Debug.AllocFunc,
            //            Debug.ReallocFunc,
            //            Debug.FreeFunc));
            //else
                Instance = new VK.Instance(instanceCreateInfo);

            //if (rendererInfo.DebugCallback)
            //{
            //    DebugReportCallbackCreateInfoExt debugReportCreateInfo = new DebugReportCallbackCreateInfoExt(
            //        DebugReportFlagsExt.All, Debug.DebugCallback
            //    );
            //    _debugReportCallback = Instance.CreateDebugReportCallbackExt(debugReportCreateInfo, Instance.Allocator);
            //}
        }

        public RenderContext CreateContext()
        {
            return new RenderContext(new RenderContextCreateInfo(Instance, Window));
        }

        ~Renderer()
        {
            
        }
    }
}
