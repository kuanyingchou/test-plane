//2014.4.11  ken  initial version. modified from https://alastaira.wordpress.com/2013/10/25/animal-crossing-curved-world-shader/
 
Shader "KZTK/KZCurvedShader" {
    Properties {
        // Diffuse texture
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // Degree of curvature
        _Curvature ("Curvature", Float) = 0.001
        _HorizontalCurvature ("Horizontal Curvature", Float) = 0.001
        _RelativePos ("Relative Position", Vector) = (0, 0, 0, 0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        // Surface shader function is called surf, and vertex preprocessor function is called vert
        #pragma surface surf Lambert vertex:vert
 
        // Access the shaderlab properties
        uniform sampler2D _MainTex;
        uniform float _Curvature;
        uniform float _HorizontalCurvature;
        uniform float4 _RelativePos;
 
        // Basic input structure to the shader function
        // requires only a single set of UV texture mapping coordinates
        struct Input {
            float2 uv_MainTex;
        };
 
        // This is where the curvature is applied
        void vert( inout appdata_full v)
        {
            // Transform the vertex coordinates from model space into world space
            float4 vv = mul( _Object2World, v.vertex );
 
            // Now adjust the coordinates to be relative to the camera position
            vv.xyz -= _RelativePos.xyz; //_WorldSpaceCameraPos.xyz;
 
            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
            // on the square of the distance from the camera in the z axis, multiplied
            // by the chosen curvature factor
            vv = float4(
                    (vv.z * vv.z) * - _HorizontalCurvature/10.0f , 
                    (vv.z * vv.z) * - _Curvature/10.0f, 
                    0.0f, 
                    0.0f );
 
            // Now apply the offset back to the vertices in model space
            v.vertex += mul(_World2Object, vv);
        }
 
        // This is just a default surface shader
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    // FallBack "Diffuse"
}
