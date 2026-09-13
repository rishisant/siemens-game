Shader "ByteCity/SpriteHighlight"
{
 Properties { [PerRendererData] _MainTex("Sprite",2D)="white"{} _Color("Tint",Color)=(.7,.8,1,1) _Occluded("Occluded only",Float)=0 }
 SubShader
 {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
  Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
  Pass
  {
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct appdata { float4 vertex:POSITION;float2 uv:TEXCOORD0;float4 color:COLOR; };
   struct v2f { float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float4 color:COLOR;float4 screen:TEXCOORD1; };
   sampler2D _MainTex, _ByteCityOccluders;float4 _MainTex_TexelSize,_Color;float _Occluded;
   v2f vert(appdata v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;o.screen=ComputeScreenPos(o.pos);return o;}
   float4 frag(v2f i):SV_Target
   {
    float a=tex2D(_MainTex,i.uv).a;
    float2 p=_MainTex_TexelSize.xy;
    float inside=min(min(tex2D(_MainTex,i.uv+float2(p.x,0)).a,tex2D(_MainTex,i.uv-float2(p.x,0)).a),min(tex2D(_MainTex,i.uv+float2(0,p.y)).a,tex2D(_MainTex,i.uv-float2(0,p.y)).a));
    float edge=saturate(a-inside);
    float sweep=pow(saturate(.5+.5*sin(i.uv.y*24-_Time.y*2.4)),12);
    float alpha=_Occluded>.5 ? (edge*.8+a*.13)*tex2D(_ByteCityOccluders,i.screen.xy/i.screen.w).r : edge*(.35+.25*sin(_Time.y*2))+a*sweep*.12;
    return float4(i.color.rgb,alpha*i.color.a);
   }
   ENDHLSL
  }
 }
}
