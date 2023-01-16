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

float T, AspectRatio, Scale, Bulge;
float2 Direction, BulgeOrigin;

float4 SpritePixelShader(VertexShaderOutput input): COLOR { 

    float2 uv = input.UV;
    uv = float2((T * Direction.x) + uv.x * AspectRatio / Scale,(T * Direction.y) + (uv.y + (Scale - 1) * 0.5) / Scale);

    uv.x %= 1;
    uv.y %= 1;

    uv -= BulgeOrigin;
    uv *= (1 - Bulge * pow(max(1 - sqrt(pow(uv.x, 2) + pow(uv.y, 2)) * 2, 0), 2));
    uv += BulgeOrigin;

    return tex2D(SpriteTextureSampler,uv) * input.Color;
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};