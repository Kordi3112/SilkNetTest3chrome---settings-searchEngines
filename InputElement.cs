using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkNetTest3
{
    public struct InputElement
    {
        public string Name;
        public int Index;

        public Format Format;

        public int Offset;

        public int Slot;

        public InputElement(string name, int index, Format format, int offset, int slot)
        {
            Name = name;
            Index = index;
            Format = format;
            Offset = offset;
            Slot = slot;
        }

        public unsafe static InputElementDesc[] GetElementDescs(InputElement[] elements)
        {
            InputElementDesc[] output = new InputElementDesc[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                InputElement element = elements[i];

                fixed (byte* name = SilkMarshal.StringToMemory(element.Name))
                {
                    output[i] = new InputElementDesc
                    {
                        SemanticName = name,
                        SemanticIndex = (uint)element.Index,
                        Format = element.Format,
                        InputSlot = (uint)element.Slot,
                        AlignedByteOffset = (uint)element.Offset,
                        InputSlotClass = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    };
                }
            }


            return output;
        }

    }
}
