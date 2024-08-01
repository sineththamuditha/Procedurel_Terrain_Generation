Shader "Unlit/CloudShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CloudSpeed ("Cloud Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CloudSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float perlinNoise(float2 uv)
            {
                float2 p = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                float n = p.x + p.y * 57.0;
                float res = lerp(
                    lerp(frac(sin(n) * 43758.5453123), frac(sin(n + 1.0) * 43758.5453123), f.x),
                    lerp(frac(sin(n + 57.0) * 43758.5453123), frac(sin(n + 58.0) * 43758.5453123), f.x),
                    f.y);
                return res;
            }

            float fbm(float2 uv, int octaves)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                for (int i = 0; i < octaves; i++)
                {
                    value += amplitude * perlinNoise(uv * frequency);
                    uv *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.y += _Time * _CloudSpeed * 1;
                float noise = fbm(uv * 50, 6);
                float alpha = smoothstep(0.4, 0.6, noise);   // Clouds appear where noise is greater than 0.5
                return fixed4(1, 1, 1, alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
