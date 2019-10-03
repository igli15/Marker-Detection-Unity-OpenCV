Shader "Hidden/ChromaticShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
			sampler2D _CameraDepthTexture;

			uniform half2 u_redOffset;
			uniform half2 u_greenOffset;
			uniform half2 u_blueOffset;
			uniform float u_effectDistance;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

				float depth = tex2D(_CameraDepthTexture, i.uv).r;
				depth = Linear01Depth(depth);
				depth = depth * _ProjectionParams.z;

			
				float filter = (1 - step(depth, u_effectDistance)) * -1;
				
				if (depth >= _ProjectionParams.z) return col;

				u_redOffset *= filter;
				u_greenOffset *= filter;
				u_blueOffset *= filter;

				col.r = tex2D(_MainTex, i.uv + u_redOffset).r;
				col.g = tex2D(_MainTex, i.uv + u_greenOffset).g;
				col.b = tex2D(_MainTex, i.uv + u_blueOffset).b;
				
				col.a = tex2D(_MainTex, i.uv).a;

                return col;
            }
            ENDCG
        }
    }
}
