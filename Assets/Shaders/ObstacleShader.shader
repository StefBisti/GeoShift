Shader "Custom/ObstacleShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", Float) = 1.0
        _Rotation ("Rotation", Float) = 0.0
    }
    SubShader {
        Tags {
            "RenderType"="Transparent" "Queue"="Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f{
		        float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;           
            };

            sampler2D _MainTex;
            float _Scale, _Rotation;

            v2f vert(appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float2 uv = worldPos.xy * _Scale;

                float rad = radians(_Rotation);
                float cosVal = cos(rad);
                float sinVal = sin(rad);

                float2 rotatedUV;
                rotatedUV.x = uv.x * cosVal - uv.y * sinVal;
                rotatedUV.y = uv.x * sinVal + uv.y * cosVal;
                
                o.uv = rotatedUV;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
