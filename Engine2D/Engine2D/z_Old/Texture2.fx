// =======
// Globals
// =======

float4x4 view;
float4x4 projection;
Texture2D tex;


float4 threshold;


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
  float  Alpha     : BLENDWEIGHT0;
};

struct PixelInput
{
  float4 Position  : POSITION0;
  float4 Color     : COLOR0;
  float2 TexCoord0 : TEXCOORD0;
  float  Alpha     : BLENDWEIGHT0;
};


// ==============
// Vertex shaders
// ==============

PixelInput TextureVertexShader(VertexInput input)
{
  // Declare result
  PixelInput output;

  // Apply vertex transformations
  output.Position = mul(input.Position, view);
  output.Position = mul(output.Position, projection);

  // Send color and coordinates downstream
  output.Color = input.Color;
  output.TexCoord0 = input.TexCoord0;
  output.Alpha = input.Alpha;
  // output.ScreenCoords = output.Position;

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

  // Check texture transparency
  if (color.a > 0.000001f)
  {
    // If not transparent, apply tint
    color = (input.Color - color) * input.Color.a + color;

    // Check threshold
    // if (color.r < threshold.r || color.g < threshold.g || color.b < threshold.b || color.a < threshold.a)
    // {
    //   color = float4(0.0f, 0.0f, 0.0f, 0.0f);
    // }

    // Lastly, apply alpha
    color.a *= input.Alpha;
  }

  // Return result
  // color = tex2D(texSampler, input.TexCoord0);
  return color;
}


// =====================
// Techniques and passes
// =====================

technique TextureTechnique
{
  pass TexturePass
  {
    VertexShader = compile vs_3_0 TextureVertexShader();
    PixelShader = compile ps_3_0 TexturePixelShader();
  }
}

