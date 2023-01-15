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

float Amount;
float2 Origin;

/* https://github.com/rivy/OpenPDN/blob/master/src/Resources/Files/License.txt */

/* Reworked from https://github.com/rivy/OpenPDN/blob/master/src/Effects/BulgeEffect.cs */

float4 SpritePixelShader(VertexShaderOutput input): COLOR {
    float2 uv = input.UV - Origin;
    uv *= (1 - Amount * pow(max(1 - sqrt(pow(uv.x, 2) + pow(uv.y, 2)) * 2, 0), 2));
    uv += Origin;
    return tex2D(SpriteTextureSampler,uv) * input.Color;
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};
