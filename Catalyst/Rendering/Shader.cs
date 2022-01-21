using System.IO;
using VK = Vulkan;

namespace Catalyst.Rendering
{
    public class Shader
    {
        public byte[] Data;
        
        public VK.ShaderModuleCreateInfo ShaderModuleCreateInfo;
        
        public Shader(string fileName, ref VK.Device device)
        {
            Data = File.ReadAllBytes(fileName);
            ShaderModuleCreateInfo = new VK.ShaderModuleCreateInfo(Data);
            
            device.CreateShaderModule(ShaderModuleCreateInfo,)
        }
    }
}