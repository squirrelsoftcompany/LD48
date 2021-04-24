Shader "Custom/PostProcessBlackVeil" {
    Properties{
      _MainTex("Texture", 2D) = "white" {}
      _VRadius("Radius", Range(0.0, 1.0)) = 1.0
      _VSoft("Softness", Range(0.0, 1.0)) = 0.5
    }

        SubShader{
          Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc" // required for v2f_img

            // Properties
            sampler2D _MainTex;
            float _VRadius;
            float _VSoft;

            float4 frag(v2f_img input) : COLOR {
                // sample texture for color
                float4 base = tex2D(_MainTex, input.uv);
                float distFromCenter = distance(input.uv.xy, float2(0.5, 0.5));
                float blackVeil = smoothstep(_VRadius, _VRadius - _VSoft, distFromCenter);
                base = saturate(base * blackVeil);
                return base;
              }
              ENDCG
        } }}