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

float AspectRatio, Scale, Bulge, TileScale, Rotation;
float2 Position, BulgeOrigin;
float4 ColorA, ColorB;

float4 SpritePixelShader(VertexShaderOutput input): COLOR { 
    float2 uv = input.UV;

    uv = float2(Position.x + uv.x * AspectRatio / Scale,Position.y + (uv.y + (Scale - 1) * 0.5) / Scale);

    float s; float c;
    sincos(Rotation,s,c);
    uv = float2(dot(uv,float2(c,-s)),dot(uv,float2(s,c))); 

    uv = frac(uv);
    uv -= BulgeOrigin;
    uv *= (1 - Bulge * pow(max(1 - sqrt(pow(uv.x, 2) + pow(uv.y, 2)) * 2, 0), 2));
    uv += BulgeOrigin;

    float4 textureColor = tex2D(SpriteTextureSampler,uv);

    uv = frac(uv * TileScale) - 0.5;
    float4 tileColor = lerp(ColorA,ColorB,step(uv.x * uv.y,0));

    return textureColor * tileColor * input.Color;
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};