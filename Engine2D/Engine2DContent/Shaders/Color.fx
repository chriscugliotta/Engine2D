// =======
// Globals
// =======

float4x4 view;
float4x4 projection;


// ================
// Type definitions
// ================

struct VertexInput
{
  float4 Position  : POSITION0;
  float4 Color     : COLOR0;
};

struct PixelInput
{
  float4 Position   : POSITION0;
  float4 Color      : COLOR0;
};


// ==============
// Vertex shaders
// ==============

PixelInput ColorVertexShader(VertexInput input)
{
  // Declare result
  PixelInput output;

  // Apply vertex transformations
  output.Position = mul(input.Position, view);
  output.Position = mul(output.Position, projection);

  // Send color downstream
  output.Color = input.Color;

  // Return result
  return output;
}


// =============
// Pixel shaders
// =============

float4 ColorPixelShader(PixelInput input) : COLOR0
{
  // Return color
  return input.Color;
}


// =====================
// Techniques and passes
// =====================

technique ColorTechnique
{
  pass ColorPass
  {
    VertexShader = compile vs_2_0 ColorVertexShader();
    PixelShader = compile ps_2_0 ColorPixelShader();
  }
}

