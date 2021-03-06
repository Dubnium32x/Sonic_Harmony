﻿Shader "Freedom Engine/PRM (Normal)"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _PRMTex ("PRM (RGB)", 2D) = "white" {}
        _BumpMap ("Normal (RGB)", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _PRMTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 prm = tex2D (_PRMTex, IN.uv_MainTex).rgb;
            float3 albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;

            o.Albedo = albedo;
            o.Metallic = prm.r;
            o.Smoothness = prm.g;
            o.Occlusion = prm.b;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex));
        }

        ENDCG
    }

    FallBack "Diffuse"
}
