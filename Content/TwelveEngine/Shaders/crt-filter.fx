sampler inputTexture;
float time;

float4 MainPS(float2 UV1: TEXCOORD0): COLOR0
{
	float4 color = tex2D(inputTexture,UV1);

	float2 UV2 = UV1;
	UV2 -= 0.001f;
	float4 leftColor = tex2D(inputTexture, UV2);

	color.r = leftColor.r;


    float row = ((UV1.y * 5.0f) + time) % 1.0f;

    color += row * -0.16f;
    color *= 1.10f;
    color.g *= 1.03f;

	return color;
}

technique Techninque1
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 MainPS();
	}
};
