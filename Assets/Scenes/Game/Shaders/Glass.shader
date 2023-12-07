Shader "Onechain/Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_EdgeColor("Edge Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _EdgeFactor("Edge Factor", float) = 1.0
		_EdgeThickness("Silouette Dropoff Rate", float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Transparent"
        }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4	_Color;
			float4	_EdgeColor;
            float   _EdgeFactor;
			float   _EdgeThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float4 normal4 = float4(v.normal, 0.0f);
				o.normal = normalize(mul(normal4, unity_WorldToObject).xyz);
				//o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.viewDir = normalize(float3(0.0f, 0.0f, -1.0f));

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);

                float edgeFactor = abs(dot(i.viewDir, i.normal)*_EdgeFactor);
                float oneMinusEdge = 1.0f - edgeFactor;

				float3 rgb = (_Color.rgb * edgeFactor) + (_EdgeColor * oneMinusEdge);
                rgb = min(float3(1.0f, 1.0f, 1.0f), rgb);
                rgb = rgb * col.rgb;

                float opacity = min(1.0f, _Color.a / edgeFactor);
                opacity = pow(opacity, _EdgeThickness);
				opacity = opacity * col.a;

				return float4(rgb, opacity);
            }
            ENDCG
        }
    }
}