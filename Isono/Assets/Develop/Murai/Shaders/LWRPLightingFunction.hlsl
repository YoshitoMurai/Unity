
// 1D random numbers
float rand(float n)
{
    return frac(sin(n));
}
float noise1(float p)
{
   float fl = floor(p);
   float fc = frac(p);
   return lerp(rand(fl), rand(fl + 1.0f), fc);
}



float2 rand2(in float2 p)
{
   return frac(float2(sin(p.x * 1.32 + p.y * 54.077), cos(p.x * 91.32 + p.y * 9.077)));
}

float voronoi(in float2 x)
{
   float2 p = floor(x);
   float2 f = frac(x);
   
   float2 res = float2(8.0,8.0);
   for(int j = -1; j <= 1; j ++)
   {
      for(int i = -1; i <= 1; i ++)
      {
         float2 b = float2(i, j);
         float2 r = float2(b) - f + rand2(p + b);
         
         // chebyshev distance, one of many ways to do this
         float d = max(abs(r.x), abs(r.y));
         
         if(d < res.x)
         {
            res.y = res.x;
            res.x = d;
         }
         else if(d < res.y)
         {
            res.y = d;
         }
      }
   }
   return res.y - res.x;
}

#define flicker (noise1(time * 2.0) * 0.9 + 0.5)
void LWRPLightingFunction_float (in float2 uv, in float time, in float3 color, out float3 col)
{
   float v = 0.0f;
   float a = 1.2f;
   float f = 0.5f;
   
   for(int i = 1; i < 3; i ++) // 4 octaves also look nice, its getting a bit slow though
   {  
      float v1 = voronoi(uv * f + 1.0);
      float v2 = 0.0;
      
      // make the moving electrons-effect for higher octaves
      if(i > 0)
      {
         // of course everything based on voronoi
         v2 = voronoi(uv * f * 0.5 + 5.0 + time);
         
         float va = 0.0, vb = 0.0;
         va = 1.0 - smoothstep(0.0, 0.1, v1);
         vb = 1.0 - smoothstep(0.0, 0.08, v2);
         v += a * pow(va * (0.8 + vb), 2.0);
      }
      
      // make sharp edges
      v1 = 1.0 - smoothstep(0.0, 0.3, v1);
      
      // noise is used as intensity map
      v2 = a * (noise1(v1 * 5.5 + 0.1));
      
      // octave 0's intensity changes a bit
      if(i == 0)
         v += v2 * flicker;
      else
         v += v2;
      
      f *= 3.0;
      a *= 0.7;
   }
   v *= exp(-0.6f * length(uv)) * 0.8f;

   float3 cexp = color;
   cexp *= 1.3;
   col = float3(pow(v, cexp.x), pow(v, cexp.y), pow(v, cexp.z)) * 2.0;
   float gray = dot(col, float3(0.299, 0.587, 0.114));
   col = float3(gray, gray, gray);
   col = pow(col,0.7);
   //if (col.g > 0.99) col = float3(1, 1, 0);
}