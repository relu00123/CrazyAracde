Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // 스프라이트 텍스처
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)  // 외곽선 색상
        _OutlineThickness ("Outline Thickness", Range(0.0, 100.0)) = 10.0  // 외곽선 두께
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

                // 스프라이트 주변에 외곽선을 추가 (텍스처의 UV 좌표를 확장하는 방식으로 외곽선을 계산)
                float2 offset1 = float2(fwidth(i.texcoord.x), 0);
                float2 offset2 = float2(0, fwidth(i.texcoord.y));

                // 주변 픽셀 중 알파 값이 있는지 확인
                float surroundingAlpha = tex2D(_MainTex, i.texcoord + offset1).a +
                                         tex2D(_MainTex, i.texcoord - offset1).a +
                                         tex2D(_MainTex, i.texcoord + offset2).a +
                                         tex2D(_MainTex, i.texcoord - offset2).a;

                // 외곽선을 그리기 위해 스프라이트 경계를 확장
                float outline = smoothstep(0.0, _OutlineThickness, surroundingAlpha - alpha);

                // 외곽선 부분과 스프라이트 본체 색상을 혼합
                half4 finalColor = lerp(_OutlineColor, color, alpha);

                // 외곽선 알파 값 적용
                finalColor.a = max(alpha, outline);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
