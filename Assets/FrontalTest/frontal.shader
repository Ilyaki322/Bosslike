Shader "Unlit/DistanceFade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 1.0
        _Corner ("Fade Start Point (World)", Vector) = (0,0,0,0)
        _MaxDistance ("Max Fade Distance", Float) = 5.0
        _Direction ("Fill Direction", Vector) = (1, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Blend;
            float3 _Corner;
            float _MaxDistance;
            float4 _Direction;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(col.rgb, float3(1, 0, 0), 0.5);
                col.a *= 0.5;
                float2 delta = i.worldPos.xy - _Corner.xy;

                float dist = distance(i.worldPos.xy, _Corner.xy);
                float progress = dot(delta, normalize(_Direction.xy));

    
                float threshold = _Blend * _MaxDistance;
                float alphaMask = step(progress, threshold);

                col.a *= alphaMask;
                return col;
            }
            ENDCG
        }
    }
}
