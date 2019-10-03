Shader "Unlit/TorusShader"
{
    Properties
    {
        _ArrowTex ("Arrow Texture", 2D) = "white" {}
		_ArrowTintColor("Arrow texture TintColor",Color) = (1,1,1,1)
	}
    SubShader
    {	
		Tags {"RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
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
                float2 arrowUv : TEXCOORD0;
            };

            struct v2f
            {
                float2 arrowUv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _ArrowTex;
            float4 _ArrowTex_ST;
			float4 _ArrowTintColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.arrowUv = TRANSFORM_TEX(v.arrowUv, _ArrowTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				if (i.arrowUv.x < 0.5 * _ArrowTex_ST.x)
				{
					i.arrowUv.x -= _Time.y * 2;
				}
				else
				{
					i.arrowUv.x *= -1;
					i.arrowUv.x -= _Time.y * 2;
				}
				float4 arrow = tex2D(_ArrowTex, i.arrowUv) * _ArrowTintColor;
                fixed4 col = arrow;
                return col;
            }
            ENDCG
        }
    }
}
