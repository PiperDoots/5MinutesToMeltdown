Shader "Custom/Scanlines" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _ScanlineIntensity("Scanline Intensity", Range(0, 1)) = 0.5
        _ScanlineCount("Scanline Count", Range(0, 100)) = 50
        _ScanlineSpeed("Scanline Speed", Range(-10, 10)) = 1
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _ScanlineIntensity;
                float _ScanlineCount;
                float _ScanlineSpeed;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float scanline = sin((i.uv.y * _ScanlineCount + _Time.y * _ScanlineSpeed) * 3.14159) * _ScanlineIntensity;
                    float4 color = tex2D(_MainTex, i.uv);
                    return color * (1 - scanline);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
