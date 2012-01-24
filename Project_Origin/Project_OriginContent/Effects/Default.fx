//=============================================================================
// 	[GLOBALS]
//=============================================================================

float4x4 World;
float4x4 Projection;

//=============================================================================
// 	[FUNCTIONS]
//=============================================================================

float4 VertexShaderFunction(float4 input : POSITION0 ) : POSITION0
{
    float4 output = mul(input, World);    
    output = mul(output, Projection);

    return output;
}

float4 PixelShaderFunction(float4 input : POSITION0 ) : COLOR0
{
    return float4(1, 1, 1, 1);
}

//=============================================================================
//	[TECHNIQUES]
//=============================================================================

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
