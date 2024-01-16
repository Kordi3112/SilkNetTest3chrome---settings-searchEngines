using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SilkNetTest3
{
    public struct VertexPositionColor(Vector2D<float> pos, Vector4D<float> col)
    {
        public Vector2D<float> Position = pos;
        public Vector4D<float> Color = col;

        public static InputElement[] GetInputElements()
        {
            return
            [
                new InputElement("POSITION", 0, Format.FormatR32G32Float, 0, 0),
                new InputElement("COLOR", 0, Format.FormatR32G32B32A32Float, 8, 0),
            ];
        }
    }
}
