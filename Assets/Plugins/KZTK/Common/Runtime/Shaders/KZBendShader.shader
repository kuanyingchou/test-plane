Shader "KZTK/KZBendShader" {
    Properties {
        // Diffuse texture
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // Degree of curvature
        _Curvature ("Curvature", Vector) = (0, 0, 0, 0)
        _RelativePos ("Relative Position", Vector) = (0, 0, 0, 0)
        _Scale ("Scale", float) = 0.01
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        // Surface shader function is called surf, and vertex preprocessor function is called vert
        #pragma surface surf Lambert vertex:vert
 
        // Access the shaderlab properties
        uniform sampler2D _MainTex;
        uniform float4 _Curvature;
        uniform float4 _RelativePos;
        uniform float _Scale;
 
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
 
            //[ x-axis
            float thetaX = radians(vv.z * _Curvature.x * _Scale);
            vv = float4(
                vv.x,
                vv.y * cos(thetaX) - vv.z * sin(thetaX),
                vv.y * sin(thetaX) + vv.z * cos(thetaX),
                0.0f
            );
 
            //[ y-axis
            float thetaY = radians(vv.z * _Curvature.y * _Scale);
            vv = float4(
                vv.x * cos(thetaY) + vv.z * sin(thetaY),
                vv.y,
                -vv.x * sin(thetaY) + vv.z * cos(thetaY),
                0.0f
            );

            //[ z-axis
            float thetaZ = radians(vv.z * _Curvature.z * _Scale);
            vv = float4(
                vv.x * cos(thetaZ) - vv.y * sin(thetaZ),
                vv.x * sin(thetaZ) + vv.y * cos(thetaZ),
                vv.z,
                0.0f
            );

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
