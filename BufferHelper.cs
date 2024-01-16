using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkNetTest3
{
    internal class BufferHelper
    {
        public static ComPtr<ID3D11Buffer> Create(ComPtr<ID3D11Device> device, int byteWidth, BindFlag bindFlag, int structureByteStride)
        {
            ComPtr<ID3D11Buffer> buffer = default;

            var bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)byteWidth,
                Usage = Usage.Default,
                BindFlags = (uint)bindFlag,
                CPUAccessFlags = (uint)CpuAccessFlag.None,
                MiscFlags = (uint)ResourceMiscFlag.None,
                StructureByteStride = (uint)structureByteStride,
            };

            unsafe
            {
                SilkMarshal.ThrowHResult(device.CreateBuffer(in bufferDesc, null, ref buffer));
            }


            return buffer;

        }

        public static ComPtr<ID3D11Buffer> Create<T>(ComPtr<ID3D11Device> device, T[] data, int byteWidth, BindFlag bindFlag, int structureByteStride) where T : unmanaged
        {
            ComPtr<ID3D11Buffer> buffer = default;

            var bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)byteWidth,
                Usage = Usage.Default,
                BindFlags = (uint)bindFlag,
                CPUAccessFlags = (uint)CpuAccessFlag.None,
                MiscFlags = (uint)ResourceMiscFlag.None,
                StructureByteStride = (uint)structureByteStride,
            };



            unsafe
            {
                fixed (T* pData = data)
                {
                    var subresourceData = new SubresourceData
                    {
                        PSysMem = pData
                    };

                    SilkMarshal.ThrowHResult(device.CreateBuffer(in bufferDesc, in subresourceData, ref buffer));
                }


            }


            return buffer;

        }


        public static ComPtr<ID3D11Buffer> Create<T>(ComPtr<ID3D11Device> device, ref T data, int byteWidth, BindFlag bindFlag, int structureByteStride) where T : unmanaged
        {
            ComPtr<ID3D11Buffer> buffer = default;

            var bufferDesc = new BufferDesc
            {
                ByteWidth = (uint)byteWidth,
                Usage = Usage.Default,
                BindFlags = (uint)bindFlag,
                CPUAccessFlags = (uint)CpuAccessFlag.None,
                MiscFlags = (uint)ResourceMiscFlag.None,
                StructureByteStride = (uint)structureByteStride,
            };



            unsafe
            {
                fixed (T* pData = &data)
                {
                    var subresourceData = new SubresourceData
                    {
                        PSysMem = pData
                    };

                    SilkMarshal.ThrowHResult(device.CreateBuffer(in bufferDesc, in subresourceData, ref buffer));
                }


            }


            return buffer;

        }
    }
}
