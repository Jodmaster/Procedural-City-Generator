Shader"Custom/TerrainShader" {
    Properties{
        _GrassColor("Grass", Color) = (0,1,0,1)
        _MudColor("Mud", Color) = (1,1,1,1)
        _RockColor("Rock", Color) = (1,0,1,1)
        _GrassSlopeMax ("Grass Slope Max", Range(0,1)) = .5
        _MudSlopeMax ("Mud Slope Max", Range(0,1)) = .5
        _BlendLevel ("Blend level", Range(0,1)) = .5
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        
        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };
        
        half _MaxHeight;
        half _GrassSlopeMax;
        half _MudSlopeMax;
        half _BlendLevel;
        fixed4 _GrassColor;
        fixed4 _MudColor;
        fixed4 _RockColor;

        void surf(Input IN, inout SurfaceOutputStandard o){
                float slope = 1 - IN.worldNormal.y;
                float grassBlendHeight = _GrassSlopeMax * (1 - _BlendLevel);
                float rockBlendHeight = _MudSlopeMax * (1 - _BlendLevel);
                float grassWeight = 1 - saturate((slope - grassBlendHeight) / (_GrassSlopeMax - grassBlendHeight));
                float mudWeight = 1 - saturate((slope - rockBlendHeight) / (_MudSlopeMax - rockBlendHeight));
                o.Albedo = (_GrassColor * grassWeight) + (_MudColor * mudWeight * (1 - grassWeight)) + (_RockColor * (1 - mudWeight));
        }
        ENDCG
    }
}
