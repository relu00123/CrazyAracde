Shader "Unlit/OutlineShader"
{
      Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // 스프라이트 텍스처
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)  // 외곽선 색상
        _OutlineThickness ("Outline Thickness", Range(0.0, 10.0)) = 1.0  // 외곽선 두께
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }  // 투명 렌더링 설정
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  // 정점 변환
                o.texcoord = v.texcoord;  // 텍스처 좌표 전달
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // 텍스처 색상 가져오기
                half4 color = tex2D(_MainTex, i.texcoord);

                // 알파 값을 기준으로 외곽선을 그릴 픽셀 선택
                float alpha = color.a;

                // 주변 픽셀의 오프셋을 계산 (좌우 및 상하로 각각)
                float2 offset1 = float2(fwidth(i.texcoord.x), 0);
                float2 offset2 = float2(0, fwidth(i.texcoord.y));

                // 주변 픽셀의 알파 값 확인
                float surroundingAlpha = max(tex2D(_MainTex, i.texcoord + offset1).a, tex2D(_MainTex, i.texcoord - offset1).a);
                surroundingAlpha = max(surroundingAlpha, tex2D(_MainTex, i.texcoord + offset2).a);
                surroundingAlpha = max(surroundingAlpha, tex2D(_MainTex, i.texcoord - offset2).a);

                // 외곽선 계산: 경계 픽셀에서 외곽선을 두껍게 그리기
                float outline = 1.0 - smoothstep(0.0, _OutlineThickness * 0.01, surroundingAlpha - alpha);

                // 최종 색상: 외곽선 부분은 외곽선 색상으로, 스프라이트 본체는 원래 색상으로
                half4 finalColor = lerp(_OutlineColor, color, alpha);

                // 투명도 처리: 외곽선 및 내부 모두 투명도 고려
                finalColor.a = max(alpha, surroundingAlpha);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
