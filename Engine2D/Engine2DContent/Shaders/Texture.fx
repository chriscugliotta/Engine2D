
// =======
// Globals
// =======

static float3 coefficients = float3(0.2126f, 0.7152f, 0.0722f);

float4x4 view;
float4x4 projection;

texture tex;
bool grayscale = false;
float threshold = 0.0f;
float brightness = 1.0f;
float saturation = 1.0f;
float contrast = 1.0f;
float alpha = 1.0f;


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


// ================
// Type definitions
// ================

struct VertexInput
{
  float4 Position  : POSITION0;
  float4 Color     : COLOR0;
  float2 TexCoord0 : TEXCOORD0;
  //float  Alpha     : BLENDWEIGHT0;
};

struct PixelInput
{
  float4 Position  : POSITION0;
  float4 Color     : COLOR0;
  float2 TexCoord0 : TEXCOORD0;
  float2 ScreenCoords : TEXCOORD1;
  //float  Alpha     : BLENDWEIGHT0;
};



// =========
// Functions
// =========

//float4 ApplyGrayscale(float4 color)
//{
//  // Multiply by grayscale coefficients
//  float luminance = dot(coefficients.rgb, color.rgb);
//  // Return result
//  return float4(luminance, luminance, luminance, color.a);
//}


// ==============
// Vertex shaders
// ==============

PixelInput TextureVertexShader(VertexInput input)
{
  // Declare result
  PixelInput output;

  // Apply vertex transformations
  output.Position = input.Position - float4(0.5f, 0.5f, 0.0f, 0.0f);
  output.Position = mul(output.Position, view);
  output.Position = mul(output.Position, projection);

  // Send color and coordinates downstream
  output.Color = input.Color;
  output.TexCoord0 = input.TexCoord0;
  output.ScreenCoords = output.Position;
  //output.Alpha = input.Alpha;

  // Return result
  return output;
}


// =============
// Pixel shaders
// =============

float4 TexturePixelShader(PixelInput input) : COLOR0
{
  // Get texture color
  float4 color = tex2D(texSampler, input.TexCoord0);
  
  // Get luminance
  float luma = dot(coefficients, color.rgb);
  // Check if grayscale
  if (grayscale) { color.rgb = luma; }
  // Skip if luminance is below threshold
  clip(luma < threshold ? -1 : 1);

  // Get brightened color
  color.rgb = brightness * color.rgb;
  // Get brightened luma
  float brightLuma = dot(coefficients, color.rgb);
  // Get saturated color
  color.rgb = lerp(brightLuma, color.rgb, saturation);
  // Get average luminance
  float3 avgLuma = 0.5f;
  // Get contrasted color
  color.rgb = lerp(avgLuma, color.rgb, contrast);

  // Apply tint
  color *= input.Color;
  // Apply global alpha
  color.a *= alpha;
  // Apply local alpha
  //color.a *= input.Alpha;

  // Return result
  return color;
}


// =====================
// Techniques and passes
// =====================

technique TextureTechnique
{
  pass TexturePass
  {
    VertexShader = compile vs_2_0 TextureVertexShader();
    PixelShader = compile ps_2_0 TexturePixelShader();
  }
}

