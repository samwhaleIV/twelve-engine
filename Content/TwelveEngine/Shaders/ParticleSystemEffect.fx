#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 WorldViewProjection;
sampler TextureSampler: register(s0);

struct VertexInput {
    float4 Position: POSITION0;
    float2 UV: TEXCOORD0;
};

struct PixelInput {
    float4 Position: SV_POSITION0;
    float2 UV: TEXCOORD0;
};

PixelInput SpriteVertexShader(VertexInput vertex) {
    PixelInput output;

    output.Position = mul(vertex.Position, WorldViewProjection);
    output.UV = vertex.UV;

    return output;
}

float4 SpritePixelShader(PixelInput fragment) : SV_TARGET {
    return tex2D(TextureSampler,fragment.UV.xy);
}

technique Technique1 {
    pass Pass1 {
        AlphaBlendEnable = true;
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}
