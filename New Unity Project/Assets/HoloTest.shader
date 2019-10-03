// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/HoloTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_LineTex("Texture",2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 lineUvs : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float3 fragPos: TEXCOORD2;
            };

            sampler2D _MainTex;
			sampler2D _LineTex;

            float4 _MainTex_ST;
			float4 _LineTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.lineUvs = TRANSFORM_TEX(v.uv, _LineTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.fragPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				
				float3 viewDir = normalize(i.fragPos - _WorldSpaceCameraPos);
				//float3 viewDir = normalize(UnityWorldSpaceViewDir(i.fragPos));
				
				float pitch = atan2(viewDir.y, viewDir.z);
				float yaw = atan2(viewDir.z, viewDir.x);

				i.lineUvs.x += cos(yaw) * cos(pitch);
				i.lineUvs.y += sin(pitch) ;
				
				fixed4 lines = tex2D(_LineTex, i.lineUvs);
				
				

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return lines * col ;
            }
            ENDCG
        }
    }
}
