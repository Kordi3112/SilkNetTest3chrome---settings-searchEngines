using Silk.NET.DXGI;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkNetTest3
{
    
    public struct VertexPositionColorTexture(Vector2D<float> position, Vector4D<float> color, Vector2D<float> tCoords)
    {
        public Vector2D<float> Position = position;
        public Vector4D<float> Color = color;
        public Vector2D<float> TCoords = tCoords;

        public unsafe static int SizeInBytes() => sizeof(VertexPositionColorTexture);

        public static InputElement[] GetInputElements()
        {

            return
            [
                new InputElement("POSITION", 0, Format.FormatR32G32Float, 0, 0),
                new InputElement("COLOR", 0, Format.FormatR32G32B32A32Float, 8, 0),
                new InputElement("TEXCOORD", 0, Format.FormatR32G32Float, 24, 0),
            ];

        }
    }
}
