Shader "Unlit/OutlineShader"
{
      Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // ��������Ʈ �ؽ�ó
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)  // �ܰ��� ����
        _OutlineThickness ("Outline Thickness", Range(0.0, 10.0)) = 1.0  // �ܰ��� �β�
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }  // ���� ������ ����
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
                o.vertex = UnityObjectToClipPos(v.vertex);  // ���� ��ȯ
                o.texcoord = v.texcoord;  // �ؽ�ó ��ǥ ����
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // �ؽ�ó ���� ��������
                half4 color = tex2D(_MainTex, i.texcoord);

                // ���� ���� �������� �ܰ����� �׸� �ȼ� ����
                float alpha = color.a;

                // �ֺ� �ȼ��� �������� ��� (�¿� �� ���Ϸ� ����)
                float2 offset1 = float2(fwidth(i.texcoord.x), 0);
                float2 offset2 = float2(0, fwidth(i.texcoord.y));

                // �ֺ� �ȼ��� ���� �� Ȯ��
                float surroundingAlpha = max(tex2D(_MainTex, i.texcoord + offset1).a, tex2D(_MainTex, i.texcoord - offset1).a);
                surroundingAlpha = max(surroundingAlpha, tex2D(_MainTex, i.texcoord + offset2).a);
                surroundingAlpha = max(surroundingAlpha, tex2D(_MainTex, i.texcoord - offset2).a);

                // �ܰ��� ���: ��� �ȼ����� �ܰ����� �β��� �׸���
                float outline = 1.0 - smoothstep(0.0, _OutlineThickness * 0.01, surroundingAlpha - alpha);

                // ���� ����: �ܰ��� �κ��� �ܰ��� ��������, ��������Ʈ ��ü�� ���� ��������
                half4 finalColor = lerp(_OutlineColor, color, alpha);

                // ���� ó��: �ܰ��� �� ���� ��� ���� ���
                finalColor.a = max(alpha, surroundingAlpha);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
