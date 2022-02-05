sampler inputTexture;

float4 MainPS(float2 UV1: TEXCOORD0): COLOR0
{
	float4 color = tex2D(inputTexture, UV1);
	float2 UV2 = UV1;
	UV2 -= 0.01f;
	float4 leftColor = tex2D(inputTexture, UV2);

	color.r = leftColor.r;

	return color;
}

technique Techninque1
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 MainPS();
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
	}
};
