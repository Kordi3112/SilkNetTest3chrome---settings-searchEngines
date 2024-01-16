using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkNetTest3
{
    /// <summary>
    /// Effects that are neccesary for VideoManager
    /// </summary>
    public class BasicShaders
    {

        public Shader BasicColorShader { get; private set; }
        public Shader BasicColorTextureShader { get; private set; }


        public void Load(ComPtr<ID3D11Device> device)
        {
            BasicColorShader = new Shader(device);
            BasicColorTextureShader = new Shader(device);

            BasicColorShader.LoadFromData(Encoding.ASCII.GetBytes(GetBasicColorSource()), "sourceBC", Shader.ShaderTypes.Vertex | Shader.ShaderTypes.Pixel);
            BasicColorTextureShader.LoadFromData(Encoding.ASCII.GetBytes(GetBasicColorTextureSource()), "sourceBCT", Shader.ShaderTypes.Vertex | Shader.ShaderTypes.Pixel);

            BasicColorShader.SetInputLayout(Shader.EffectInputLayoutType.PositionColor);
            BasicColorTextureShader.SetInputLayout(Shader.EffectInputLayoutType.PositionColorTexture);
        }



        private static string GetBasicColorSource()
        {
            string source = @"
                                struct VS_IN {
                                    float2 pos : POSITION; 
                                    float4 col : COLOR;
                                }; 
                                struct PS_IN { 
                                    float4 pos : SV_POSITION; 
                                    float4 col : COLOR;
                                }; 


                                PS_IN VS(VS_IN input) { 
                                    PS_IN output = (PS_IN)0; 
                                    output.pos = float4(input.pos, 0, 1); 
                                    output.col = input.col; 
                                    return output; 
                                } 

                                float4 PS(PS_IN input) : SV_Target 
                                {
                                    //return float4(1,0,0,1);
                                    return input.col;
                                }";

            return source;
        }

        private static string GetBasicColorTextureSource()
        {
            string source = @"
                                struct VS_IN { 
                                    float2 pos : POSITION; 
                                    float4 col : COLOR; 
                                    float2 tex : TEXCOORD;
                                }; 
                                struct PS_IN {
                                    float4 pos : SV_POSITION; 
                                    float4 col : COLOR; 
                                    float2 tex : TEXCOORD;
                                };

  
                                Texture2D shaderTexture: register(t0); 
                                SamplerState SampleType: register(s0); 

                                PS_IN VS(VS_IN input) { 
                                    PS_IN output = (PS_IN)0; 
                                    output.pos = float4(input.pos, 0, 1);
                                    output.col = input.col; 
                                    output.tex = input.tex; 
                                    return output; 
                                } 
                                float4 PS(PS_IN input) : SV_Target {
                                    float4 color = shaderTexture.Sample(SampleType, input.tex) * input.col; 
                                    //color = float4(input.tex.x,0,0,1);
                                    return color;
                                }";

            return source;
        }
    }
}
