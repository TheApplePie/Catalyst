using System;
using System.Collections.Generic;
using System.Linq;

using GLFW3;
using VK = Vulkan;
using Vulkan.Khr;
using Vulkan.Ext;

namespace RPG.Rendering
{
    public class RenderContext
    {
        public VK.Device Device;
        public SurfaceKhr Surface;

        public VK.Queue GraphicsQueue;
        public VK.Queue ComputeQueue;
        public VK.Queue PresentQueue;
        public VK.CommandPool GraphicsCommandPool;
        public VK.CommandPool ComputeCommandPool;

        public SwapchainKhr Swapchain;
        public VK.Image[] Framebuffer;
        
        //Refrences
        public VK.Instance Instance;
        public Window Window;
        
        private int _graphicsQueueFamilyIndex = -1;
        private int _computeQueueFamilyIndex = -1;
        private int _presentQueueFamilyIndex = -1;

        public unsafe RenderContext(RenderContextCreateInfo createInfo)
        {
            Instance = createInfo.Instance;
            Window = createInfo.Window;
        
            //Setup Surface
            Surface = CreateSurface(Instance, Window);
        
            //Setup Device
            VK.PhysicalDevice physicalDevice = GetPhysicalDevice(Instance, Surface);

            foreach (var VARIABLE in physicalDevice.EnumerateExtensionProperties())
            {
                Console.WriteLine(VARIABLE.ExtensionName);
            }
        
            string[] requiredDeviceExtensions = {"VK_KHR_swapchain"};
            Device = CreateDevice(Instance, physicalDevice, requiredDeviceExtensions);

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

            //VK.ImageView imageViews = new VK.ImageView(Device, Framebuffer, imageCreateInfo, Instance.Allocator);
        }

        private VK.PhysicalDevice GetPhysicalDevice(VK.Instance instance, SurfaceKhr surface)
        {
            VK.PhysicalDevice[] physicalDevices = instance.EnumeratePhysicalDevices();

            if (physicalDevices.Length == 0)
                throw new System.Exception("No devices with Vulkan support found");

            foreach (VK.PhysicalDevice device in physicalDevices)
            {
                device.GetProperties();
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

        private VK.Device CreateDevice(VK.Instance instance, VK.PhysicalDevice physicalDevice, string[] requiredExtensions)
        {
            foreach (string extension in requiredExtensions)
                if (physicalDevice.EnumerateExtensionProperties().All(i => i.ExtensionName != extension))
                    throw new System.Exception($"Required Device Extension: {extension} not found");

            bool sameGraphicsAndPresent = _graphicsQueueFamilyIndex == _presentQueueFamilyIndex;
            var queueCreateInfos = new VK.DeviceQueueCreateInfo[sameGraphicsAndPresent ? 1 : 2];
            queueCreateInfos[0] = new VK.DeviceQueueCreateInfo(_graphicsQueueFamilyIndex, 1, 1.0f);
            if (!sameGraphicsAndPresent)
                queueCreateInfos[1] = new VK.DeviceQueueCreateInfo(_presentQueueFamilyIndex, 1, 1.0f);

            var deviceCreateInfo = new VK.DeviceCreateInfo(
                queueCreateInfos,
                requiredExtensions, physicalDevice.GetFeatures());

            return physicalDevice.CreateDevice(deviceCreateInfo, instance.Allocator);
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
        
        private static SurfaceKhr CreateSurface(VK.Instance instance, Window window)
        {
            GLFW3.Vulkan.CreateWindowSurface(instance.Handle, window, IntPtr.Zero,
                out ulong surfaceHandle);
            
            VK.AllocationCallbacks? allocationCallbacks = instance.Allocator;
            return new SurfaceKhr(instance, ref allocationCallbacks, (long)surfaceHandle);
        }
        
        ~RenderContext()
        {
            Swapchain.Dispose();
            Device.Dispose();
            Surface.Dispose();
        }
    }
}