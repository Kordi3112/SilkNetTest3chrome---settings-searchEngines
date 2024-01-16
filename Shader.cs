
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;

using System.Runtime.CompilerServices;
using System.Xml;


namespace SilkNetTest3
{
    public class Shader : IDisposable
    {
        [Flags]
        public enum ShaderTypes
        {
            Vertex = 1,
            Pixel = 1 << 1,
            Geometry = 1 << 2,
        }
        public enum EffectInputLayoutType
        {
            PositionColorTexture,
            PositionColor,
            PositionTexture,
            PositionTextureInt,
            PositionPosPos,
        }


        public const string VSEntryPoint = "VS";
        public const string PSEntryPoint = "PS";
        public const string GSEntryPoint = "GS";

        public const string VSTarget = "vs_4_0";
        public const string PSTarget = "ps_4_0";
        public const string GSTarget = "gs_4_0";

        private ShaderTypes types;

        private ComPtr<ID3D11VertexShader> vertexShader = null;

        private ComPtr<ID3D11PixelShader> pixelShader = null;

        private ComPtr<ID3D11GeometryShader> geometryShader = null;


        private ComPtr<ID3D10Blob> vertexSignature;

        private ComPtr<ID3D11InputLayout> inputLayout = null;

        private ComPtr<ID3D11Device> device = default;
        private ComPtr<ID3D11DeviceContext> context = default;

        private D3DCompiler compiler = null;

        public Shader(ComPtr<ID3D11Device> device)
        {
            this.device = device;

            compiler = D3DCompiler.GetApi();

            device.GetImmediateContext(ref context);
        }


        public unsafe void LoadFromData(byte[] data, string sourceName, ShaderTypes shaderTypesToLoad)
        {
            types = shaderTypesToLoad;

            if (shaderTypesToLoad.HasFlag(ShaderTypes.Vertex))
                CreateVertexShader(data, sourceName);


            if (shaderTypesToLoad.HasFlag(ShaderTypes.Geometry))
                CreateGeometryShader(data, sourceName);


            if (shaderTypesToLoad.HasFlag(ShaderTypes.Pixel))
                CreatePixelShader(data, sourceName);


        }

        public void SetInputLayout(EffectInputLayoutType type)
        {
            if (type == EffectInputLayoutType.PositionColor)
            {
                SetInputLayout(VertexPositionColor.GetInputElements());

                return;
            }

            if (type == EffectInputLayoutType.PositionColorTexture)
            {
                SetInputLayout(VertexPositionColorTexture.GetInputElements());

                return;
            }
            //TODO: implement the rest
            //......

            throw new Exception("Not implemented type");
        }

        public void ApplyShader()
        {

            context.IASetInputLayout(inputLayout);

            if (types.HasFlag(ShaderTypes.Vertex))
                context.VSSetShader(vertexShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
            else
                context.VSSetShader(Unsafe.NullRef<ComPtr<ID3D11VertexShader>>(), ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);

            if (types.HasFlag(ShaderTypes.Geometry))
                context.GSSetShader(geometryShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
            //else
               // context.GSSetShader(Unsafe.NullRef<ComPtr<ID3D11GeometryShader>>(), ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);

            if (types.HasFlag(ShaderTypes.Pixel))
                context.PSSetShader(pixelShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
            else
                context.PSSetShader(Unsafe.NullRef<ComPtr<ID3D11PixelShader>>(), ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);

        }



        private unsafe void SetInputLayout(InputElement[] elements)
        {
            InputElementDesc[] elementDescs = InputElement.GetElementDescs(elements);

            fixed(InputElementDesc* pElementDescs = elementDescs)
            {
               
                SilkMarshal.ThrowHResult(device.CreateInputLayout(pElementDescs, (uint)elementDescs.Length, vertexSignature.GetBufferPointer(), vertexSignature.GetBufferSize(), ref inputLayout));
            }
        }

        private unsafe ComPtr<ID3D10Blob> GetShaderCode(byte[] data, string sourceName, string entryPoint, string target)
        {
            ComPtr<ID3D10Blob> code = default;
            ComPtr<ID3D10Blob> errors = default;


            HResult hr = compiler.Compile
            (
                in data[0],
                (nuint)data.Length,
                sourceName,
                null,
                ref Unsafe.NullRef<ID3DInclude>(),
                entryPoint,
                target,
                0,
                0,
                ref code,
                ref errors
            );


            if (hr.IsFailure)
            {
                if (errors.Handle is not null)
                {
                    Console.WriteLine(SilkMarshal.PtrToString((nint)errors.GetBufferPointer(), NativeStringEncoding.Ansi));
                }

                

                hr.Throw();
            }

            errors.Dispose();

            return code;
        }

        private unsafe void CreateVertexShader(byte[] data, string sourceName)
        {
            vertexSignature = GetShaderCode(data, sourceName, VSEntryPoint, VSTarget);

            // Create vertex shader.
            SilkMarshal.ThrowHResult
            (
                device.CreateVertexShader
                (
                    vertexSignature.GetBufferPointer(),
                    vertexSignature.GetBufferSize(),
                    ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                    ref vertexShader
                )
            );

            
        }

        private unsafe void CreateGeometryShader(byte[] data, string sourceName)
        {
            var code = GetShaderCode(data, sourceName, GSEntryPoint, GSTarget);

            // Create vertex shader.
            SilkMarshal.ThrowHResult
            (
                device.CreateGeometryShader
                (
                    code.GetBufferPointer(),
                    code.GetBufferSize(),
                    ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                    ref geometryShader
                )
            );

            code.Dispose();
        }

        private unsafe void CreatePixelShader(byte[] data, string sourceName)
        {
            var code = GetShaderCode(data, sourceName, PSEntryPoint, PSTarget);

            // Create vertex shader.
            SilkMarshal.ThrowHResult
            (
                device.CreatePixelShader
                (
                    code.GetBufferPointer(),
                    code.GetBufferSize(),
                    ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                    ref pixelShader
                )
            );

            code.Dispose();
        }

        public void Dispose()
        {
            vertexSignature.Dispose();

            if (types.HasFlag(ShaderTypes.Vertex))
                vertexShader.Dispose();

            if (types.HasFlag(ShaderTypes.Geometry))
                geometryShader.Dispose();

            if (types.HasFlag(ShaderTypes.Pixel))
                pixelShader.Dispose();

            inputLayout.Dispose();
        }
    }
}
