using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using GLFW3;
using VK = Vulkan;
using Vulkan.Khr;
using Vulkan.Ext;

namespace Catalyst.Rendering
{ 
    public class Renderer
    {
        public static VK.Instance Instance;
        public static VK.ApplicationInfo AppInfo;

        public static VK.ExtensionProperties[] Extensions;
        public static VK.LayerProperties[] Layers;
        
        public static bool UseAllocCallbacks = false;
        public static bool UseDebugCallback =
#if DEBUG
            true;
#else
            false;
#endif

        private static DebugReportCallbackExt _debugReportCallback;

        public VK.PhysicalDevice PhysicalDevice;
        public VK.Device Device;
        public SurfaceKhr Surface;
        
        public VK.Queue GraphicsQueue;
        public VK.Queue ComputeQueue;
        public VK.Queue PresentQueue;
        public VK.CommandPool GraphicsCommandPool;
        public VK.CommandPool ComputeCommandPool;

        public SwapchainKhr Swapchain;
        public VK.Image[] Framebuffer;

        private int _graphicsQueueFamilyIndex = -1;
        private int _computeQueueFamilyIndex = -1;
        private int _presentQueueFamilyIndex = -1;
        
        static Renderer()
        {
            if (!GLFW3.Vulkan.IsSupported)
                throw new System.Exception("Vulkan is not supported");
            
#if DEBUG
            string[] requiredExtensions = new [] {"VK_EXT_debug_utils", "VK_EXT_debug_report"}.Concat(GLFW3.Vulkan.GetRequiredInstanceExtensions()).ToArray();
            string[] requiredLayers = new [] {"VK_LAYER_KHRONOS_validation"};
#else
            string[] requiredExtensions = GLFW3.Vulkan.GetRequiredInstanceExtensions();
            string[] requiredLayers = new string[] {};
#endif

            VK.LayerProperties[] availableLayers = VK.Instance.EnumerateLayerProperties();
            VK.ExtensionProperties[] availableExtensions = VK.Instance.EnumerateExtensionProperties();

            foreach (string layer in requiredLayers)
                if (availableLayers.All(i => i.LayerName != layer))
                    throw new System.Exception($"Required layer: {layer} not found");

            foreach (string extension in requiredExtensions)
                if (availableExtensions.All(i => i.ExtensionName != extension))
                    throw new System.Exception($"Required Extension: {extension} not found");

            //TODO: APPINFO
            VK.InstanceCreateInfo instanceCreateInfo = new VK.InstanceCreateInfo(AppInfo, requiredLayers, requiredExtensions);

            if (UseAllocCallbacks)
                Instance = new VK.Instance(instanceCreateInfo,
                    new VK.AllocationCallbacks(
                        AllocFunc,
                        ReallocFunc,
                        FreeFunc));
            else
                Instance = new VK.Instance(instanceCreateInfo);

            if (UseDebugCallback)
            {
                DebugReportCallbackCreateInfoExt debugReportCreateInfo = new DebugReportCallbackCreateInfoExt(
                    DebugReportFlagsExt.All, DebugCallback
                );
                _debugReportCallback = Instance.CreateDebugReportCallbackExt(debugReportCreateInfo, Instance.Allocator);
            }
        }

        public Renderer(Window window) : this(CreateSurface(window)) { }

        public Renderer(SurfaceKhr surface)
        {
            Surface = surface;
        
            //Setup Device
            VK.PhysicalDevice physicalDevice = GetPhysicalDevice(Surface);

            string[] requiredDeviceExtensions = {"VK_KHR_swapchain"};
            Device = CreateDevice(physicalDevice, requiredDeviceExtensions);

            // Get queue(s).
            GraphicsQueue = Device.GetQueue(_graphicsQueueFamilyIndex);
            ComputeQueue = _computeQueueFamilyIndex == _graphicsQueueFamilyIndex
                ? GraphicsQueue
                : Device.GetQueue(_computeQueueFamilyIndex);
            PresentQueue = _presentQueueFamilyIndex == _graphicsQueueFamilyIndex
                ? GraphicsQueue
                : Device.GetQueue(_presentQueueFamilyIndex);

            // Create command pool(s).
            GraphicsCommandPool = Device.CreateCommandPool(new VK.CommandPoolCreateInfo(_graphicsQueueFamilyIndex));
            ComputeCommandPool = Device.CreateCommandPool(new VK.CommandPoolCreateInfo(_computeQueueFamilyIndex));

            Swapchain = CreateSwapchain(Instance, Device, Surface);
            Framebuffer = Swapchain.GetImages();
        }
        
        private VK.PhysicalDevice GetPhysicalDevice(SurfaceKhr surface)
        {
            VK.PhysicalDevice[] physicalDevices = Instance.EnumeratePhysicalDevices();

            if (physicalDevices.Length == 0)
                throw new System.Exception("No devices with Vulkan support found");

            foreach (VK.PhysicalDevice device in physicalDevices)
            {
                VK.QueueFamilyProperties[] queueFamilyProperties = device.GetQueueFamilyProperties();

                for (int i = 0; i < queueFamilyProperties.Length; i++)
                {
                    if (queueFamilyProperties[i].QueueFlags.HasFlag(VK.Queues.Graphics))
                    {
                        if (_graphicsQueueFamilyIndex == -1) _graphicsQueueFamilyIndex = i;
                        if (_computeQueueFamilyIndex == -1) _computeQueueFamilyIndex = i;

                        if (device.GetSurfaceSupportKhr(i, surface))
                            _presentQueueFamilyIndex = i;

                        if (_graphicsQueueFamilyIndex != -1 &&
                            _computeQueueFamilyIndex != -1 &&
                            _presentQueueFamilyIndex != -1)
                        {
                            return device; //TEST DEVICE SUITABILITY
                        }
                    }
                }
            }

            throw new System.Exception("No suitable devices found");
        }

        private VK.Device CreateDevice(VK.PhysicalDevice physicalDevice, string[] requiredExtensions)
        {
            foreach (string extension in requiredExtensions)
                if (physicalDevice.EnumerateExtensionProperties().All(i => i.ExtensionName != extension))
                    throw new System.Exception($"Required Device Extension: {extension} not found");
            
            //physicalDevice.GetSurfaceSupportKhr()
            
            bool sameGraphicsAndPresent = _graphicsQueueFamilyIndex == _presentQueueFamilyIndex;
            VK.DeviceQueueCreateInfo[] queueCreateInfos = new VK.DeviceQueueCreateInfo[sameGraphicsAndPresent ? 1 : 2];
            queueCreateInfos[0] = new VK.DeviceQueueCreateInfo(_graphicsQueueFamilyIndex, 1, 1.0f);
            if (!sameGraphicsAndPresent)
                queueCreateInfos[1] = new VK.DeviceQueueCreateInfo(_presentQueueFamilyIndex, 1, 1.0f);

            VK.DeviceCreateInfo deviceCreateInfo = new VK.DeviceCreateInfo(
                queueCreateInfos,
                requiredExtensions, physicalDevice.GetFeatures());

            return physicalDevice.CreateDevice(deviceCreateInfo, Instance.Allocator);
        }

        private static SwapchainKhr CreateSwapchain(VK.Instance instance, VK.Device device, SurfaceKhr surface)
        {
            SurfaceCapabilitiesKhr capabilities = device.Parent.GetSurfaceCapabilitiesKhr(surface);
            SurfaceFormatKhr[] formats = device.Parent.GetSurfaceFormatsKhr(surface);
            PresentModeKhr[] presentModes = device.Parent.GetSurfacePresentModesKhr(surface);

            VK.Format format = formats.Length == 1 && formats[0].Format == VK.Format.Undefined
                ? VK.Format.B8G8R8A8UNorm
                : formats[0].Format;

            PresentModeKhr presentMode =
                presentModes.Contains(PresentModeKhr.Mailbox) ? PresentModeKhr.Mailbox : //PREFERED
                presentModes.Contains(PresentModeKhr.FifoRelaxed) ? PresentModeKhr.FifoRelaxed :
                presentModes.Contains(PresentModeKhr.Fifo) ? PresentModeKhr.Fifo :
                PresentModeKhr.Immediate; //VSYNC OFF

            return device.CreateSwapchainKhr(new SwapchainCreateInfoKhr(
                surface: surface,
                imageFormat: format,
                imageExtent: capabilities.CurrentExtent,
                preTransform: capabilities.CurrentTransform,
                presentMode: presentMode), instance.Allocator);
        }
        
        private static SurfaceKhr CreateSurface(Window window)
        {
            GLFW3.Vulkan.CreateWindowSurface(Instance.Handle, window, IntPtr.Zero, 
                out ulong surfaceHandle);
            
            VK.AllocationCallbacks? allocationCallbacks = Instance.Allocator;
            return new SurfaceKhr(Instance, ref allocationCallbacks, (long)surfaceHandle);
        }

        public static IntPtr AllocFunc(IntPtr userData, VK.Size size, VK.Size alignment, VK.SystemAllocationScope allocationScope)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            var sourceArray = new byte[size];
            Marshal.Copy(sourceArray, 0, ptr, (int)size);
            
            Debug.Log($"Allocating: size: {size}, alignment: {alignment}, allocationScope: {allocationScope}, return ptr* : 0x{ptr}");
            return ptr;  
        }

        public static IntPtr ReallocFunc(IntPtr userData, IntPtr original, VK.Size size, VK.Size alignment, VK.SystemAllocationScope allocationScope)
        {
            IntPtr ptr = Marshal.ReAllocHGlobal(original, size);
            Marshal.FreeHGlobal(original);
            
            Debug.Log($"Reallocating: original: {original}, size: {size}, alignment: {alignment}, allocationScope: {allocationScope}, return ptr* : 0x{ptr}");
            return ptr;  
        }
        
        public static void FreeFunc(IntPtr userData, IntPtr memory)
        {
            Debug.Log($"Freeing ptr: 0x{memory}");
            Marshal.FreeHGlobal(memory);
        }

        public static bool DebugCallback(DebugReportCallbackInfo info)
        {
            Debug.Log($"[{info.Flags}][{info.LayerPrefix}] {info.Message}");
            return info.Flags.HasFlag(DebugReportFlagsExt.Error);
        }
        
        ~Renderer()
        {
            Swapchain.Dispose();
            Device.Dispose();
            Surface.Dispose();
        }
    }
}
