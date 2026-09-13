Shader "ByteCity/OccluderMask"
{
 Properties { [PerRendererData] _MainTex("Sprite",2D)="white"{} }
 SubShader
 {
  Tags { "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
  Cull Off ZWrite Off ZTest Always Blend One One
  Pass
  {
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct appdata { float4 vertex:POSITION;float2 uv:TEXCOORD0;float4 color:COLOR; };
   struct v2f { float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float4 color:COLOR; };
   sampler2D _MainTex;
   v2f vert(appdata v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
   float4 frag(v2f i):SV_Target {float a=tex2D(_MainTex,i.uv).a*i.color.a;clip(a-.2);return 1;}
   ENDHLSL
  }
 }
}
