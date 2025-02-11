Shader "Custom/ShapeInteriorShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Rotation ("Rotation", Range(0.0, 180)) = 0
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
                float2 uv : TEXCOORD0;
            };

            struct v2f{
		        float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Rotation, _UVStretchX;


            v2f vert (appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                float2 center = float2(0.5, 0.5);
                float2 uv = v.uv - center;

                float rad = radians(_Rotation);
                float cosA = cos(rad); 
                float sinA = sin(rad);

                float2 rotatedUV;
                rotatedUV.x = uv.x * cosA - uv.y * sinA;
                rotatedUV.y = uv.x * sinA + uv.y * cosA;
                rotatedUV += center;

                o.uv = TRANSFORM_TEX(rotatedUV, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDCG
        }
    }
}
