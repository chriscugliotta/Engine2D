// =======
// Globals
// =======

float4x4 view;
float4x4 projection;

texture tex;
texture mask;
float height;
int sampleSize;
float offsets[21];
float weights[21];


// ========
// Samplers
// ========

sampler texSampler = sampler_state
{
  Texture = <tex>;
  Filter = MIN_MAG_MIP_LINEAR;
  AddressU = clamp;
  AddressV = clamp;
};

sampler maskSampler = sampler_state
{
  Texture = <mask>;
  Filter = MIN_MAG_MIP_LINEAR;
  AddressU = clamp;
  AddressV = clamp;
};


// =====
// Types
// =====

struct VertexInput
{
  float4 Position  : POSITION0;
  float4 Color   : COLOR0;
  float2 TexCoord0 : TEXCOORD0;
};

struct PixelInput
{
  float4 Position  : POSITION0;
  float2 TexCoord0 : TEXCOORD0;
  float2 TexCoord1 : TEXCOORD1;
};


// ==============
// Vertex shaders
// ==============

PixelInput VerticalBlurVertexShader(VertexInput input)
{
  // Declare result
  PixelInput output;

  // Apply vertex transformations
  output.Position = input.Position - float4(0.5f, 0.5f, 0.0f, 0.0f);
  output.Position = mul(output.Position, view);
  output.Position = mul(output.Position, projection);

  // Get texel size
  float texelSize = 1.0f / height;
  
  // Get texture coordinate of this pixel and its vertical neighbor
  output.TexCoord0 = input.TexCoord0;
  output.TexCoord1 = input.TexCoord0 + float2(0.0f, texelSize);

  // Return result
  return output;
}


// =============
// Pixel shaders
// =============

float4 VerticalBlurPixelShader(PixelInput input) : COLOR0
{
  // Initialize blurred texture color as black
  float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);

  // Get pixel distance
  float2 d = input.TexCoord1 - input.TexCoord0;

  // Get weighted average of neighboring colors
  for (int i = 0; i < sampleSize; i++)
  {
    color += tex2D(texSampler, input.TexCoord0 - offsets[i] * d) * weights[i];
  }

  // Return result
  return color;
}

float4 MaskedVerticalBlurPixelShader(PixelInput input) : COLOR0
{
  // Get mask color
  float4 maskColor = tex2D(maskSampler, input.TexCoord0);
  // Get non-blurred texture color
  float4 trueColor = tex2D(texSampler, input.TexCoord0);

  // Check if mask is transparent
  if (maskColor.r < 0.000001f)
  {
    // If so, do not blur
    return trueColor;
  }
  else
  {
    // Otherwise, apply blur

    // Initialize blurred texture color as black
    float4 blurColor = float4(0.0f, 0.0f, 0.0f, 0.0f);

    // Get pixel distance
    float2 d = input.TexCoord1 - input.TexCoord0;

    // Get weighted average of neighboring colors
    for (int i = 0; i < sampleSize; i++)
    {
      blurColor += tex2D(texSampler, input.TexCoord0 - offsets[i] * d) * weights[i];
    }

    // Check if mask is opaque
    if (maskColor.r > 0.999999f)
    {
      // If so, return fully blurred color
      return blurColor;
    }
    else
    {
      // Otherwise, return partially blurred color
      return blurColor * maskColor.r + trueColor * (1.0f - maskColor.r);
    }
  }
}


// =====================
// Techniques and passes
// =====================

technique VerticalBlurTechnique
{
  pass VerticalBlurPass
  {
    VertexShader = compile vs_3_0 VerticalBlurVertexShader();
    PixelShader = compile ps_3_0 VerticalBlurPixelShader();
  }
}

technique MaskedVerticalBlurTechnique
{
  pass MaskedVerticalBlurPass
  {
    VertexShader = compile vs_3_0 VerticalBlurVertexShader();
    PixelShader = compile ps_3_0 MaskedVerticalBlurPixelShader();
  }
}
