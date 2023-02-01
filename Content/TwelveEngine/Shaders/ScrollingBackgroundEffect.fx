#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
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

    float2 scale = float2(Scale / AspectRatio, Scale);
    float2 uv = Position + (input.UV + (scale - 1) * 0.5) / scale;

    float s; float c;
    sincos(Rotation,s,c);
    uv -= 0.5;
    uv = float2(dot(uv,float2(c,-s)),dot(uv,float2(s,c))); 
    uv += 0.5;

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