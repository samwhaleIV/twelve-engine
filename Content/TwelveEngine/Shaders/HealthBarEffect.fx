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

float OffColorStart, OffColorEnd;
float4 OffColor;

float InRange(float a,float b,float x) {
    return step(a,x) - step(b,x);
}

float4 SpritePixelShader(VertexShaderOutput input): COLOR {
    float4 color = lerp(input.Color,OffColor,InRange(OffColorStart,OffColorEnd,input.UV.x));
    return tex2D(SpriteTextureSampler,input.UV) * color;
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};
