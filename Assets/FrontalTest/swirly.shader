Shader "B83/Twirl"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Twirl ("twirl", float) = 0
    }
    SubShader
    {

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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            uniform float _Twirl;

            fixed4 frag(v2f i) : SV_Target
            {
                // center UV coords
                float2 uv = i.uv -0.5;
                // get angle from the center to uv coordinate
                float a = atan2(uv.y, uv.x);
                // get distance from the center
                float d = length(uv);
                // offset angle based on twirl strength and distance from the center.
                // affect the center the most
                a += (0.5 - d) * _Twirl;
                // recalculate new uv coordinate with new angle.
                uv.x = cos(a) * d;
                uv.y = sin(a) * d;
                uv += 0.5;

                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}