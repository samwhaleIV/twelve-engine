#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state {
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput {
    float4 Position: SV_POSITION;
    float4 Color: COLOR0;
    float2 UV: TEXCOORD0;
};

float Time, AspectRatio, Scale;

float4 SpritePixelShader(VertexShaderOutput input): COLOR { 
    float2 uv = float2(Time + input.UV.x * AspectRatio / Scale,(input.UV.y + (Scale - 1) * 0.5) / Scale);
    return tex2D(SpriteTextureSampler,uv) * input.Color;
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};