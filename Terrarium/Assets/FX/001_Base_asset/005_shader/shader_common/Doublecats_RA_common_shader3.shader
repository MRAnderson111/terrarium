// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DC/Doublecats_RA_common_shader3"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[NoScaleOffset]_Base_Tex("Base_Tex", 2D) = "white" {}
		_Base_uv("Base_uv", Vector) = (1,1,0,0)
		_Base_speed("Base_speed", Vector) = (1,0,0,0)
		_Base_distort_power("Base_distort_power", Range( 0 , 1)) = 0
		[Toggle(_USE_CUSTOM1_ZW_MOVE_ON)] _Use_custom1_zw_move("Use_custom1_zw_move", Float) = 0
		[HDR]_R_light_color("R_light_color", Color) = (1,0.92775,0.6462264,0)
		[HDR]_R_dark_clolor("R_dark_clolor", Color) = (0.1509434,0.07064492,0.03203988,0)
		_Constart("Constart", Range( 0.45 , 3)) = 1
		[HDR]_Color("Color", Color) = (1,1,1,0)
		_Color_power("Color_power", Range( 0 , 4)) = 1
		_Distance("Distance", Range( 0 , 4)) = 2.635294
		_Alpha_Constart("Alpha_Constart", Range( 0.45 , 6)) = 1
		_Alpha("Alpha", Range( 0 , 3)) = 1
		[NoScaleOffset]_color_Tex("color_Tex", 2D) = "white" {}
		_color_uv("color_uv", Vector) = (1,1,0,0)
		_color_speed("color_speed", Vector) = (1,0,0,0)
		_color_distort_power("color_distort_power", Range( 0 , 1)) = 0
		_Desaturate("Desaturate", Range( 0 , 1)) = 0
		_color_Constart("color_Constart", Range( 0.1 , 6)) = 1
		[NoScaleOffset]_Dissolve_Tex("Dissolve_Tex", 2D) = "white" {}
		_Dissolve_uv("Dissolve_uv", Vector) = (1,1,0,0)
		_Dissolve_speed("Dissolve_speed", Vector) = (1,0,0,0)
		_Dissolve_distort_power1("Dissolve_distort_power", Range( 0 , 1)) = 0
		_Hardness("Hardness", Range( 0 , 22)) = 11
		_Dissolve("Dissolve", Range( 0 , 1)) = 1
		[Toggle(_USE_CUSTOM1_X_DISSOLVE_ON)] _use_custom1_x_dissolve("use_custom1_x_dissolve", Float) = 0
		[NoScaleOffset]_Base_mask_Tex("Base_mask_Tex", 2D) = "white" {}
		_Base_mask_uv("Base_mask_uv", Vector) = (1,1,0,0)
		_Base_mask_speed("Base_mask_speed", Vector) = (1,0,0,0)
		[Toggle(_CUSTOM2_ZW_MOVE_MASK_ON)] _Custom2_zw_move_mask("Custom2_zw_move_mask", Float) = 0
		_mask_Constart("mask_Constart", Range( 0.45 , 6)) = 1
		_mask_power("mask_power", Range( 1 , 9)) = 1
		_Mask_distort_power("Mask_distort_power", Range( 0 , 1)) = 0
		[NoScaleOffset]_Distort_Tex("Distort_Tex", 2D) = "bump" {}
		_Distort_uv1("Distort_uv", Vector) = (1,1,0,0)
		_Distort_speed1("Distort_speed", Vector) = (1,0,0,0)
		_direction("direction", Vector) = (0,0,1,1)
		_Distort("Distort", Range( -2 , 2)) = 0
		[Toggle(_USE_CUSTOM1_Y_DISTORT_ON)] _use_custom1_y_distort("use_custom1_y_distort", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		[Enum(off,0,on,1)]_Zwrite("Zwrite", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 0
		[IntRange][Enum(UnityEngine.Rendering.CompareFunction)]_Ztest("Ztest", Float) = 4
		[Toggle(_USE_FRENSEL_ON)] _use_frensel("use_frensel", Float) = 0
		[Toggle(_FRENSEL_FLIP_ON)] _frensel_flip("frensel_flip", Float) = 0
		_frensel("frensel", Range( -0.01 , 1)) = -0.01
		_frensel_edge("frensel_edge", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull [_Cull]
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140012
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma instancing_options renderinglayer

			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        	#pragma multi_compile_fragment _ DEBUG_DISPLAY
        	#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        	#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _USE_CUSTOM1_ZW_MOVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISTORT_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON
			#pragma shader_feature_local _CUSTOM2_ZW_MOVE_MASK_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_speed;
			float4 _color_speed;
			float4 _Base_mask_speed;
			float4 _Base_mask_uv;
			float4 _Dissolve_uv;
			float4 _direction;
			float4 _Distort_uv1;
			float4 _color_uv;
			float4 _Distort_speed1;
			float4 _Base_speed;
			float4 _R_light_color;
			float4 _R_dark_clolor;
			float4 _Color;
			float4 _Base_uv;
			float4 _Mask_ST;
			float _Hardness;
			float _Dissolve;
			float _Dissolve_distort_power1;
			float _Mask_distort_power;
			float _mask_Constart;
			float _mask_power;
			float _Distance;
			float _Cull;
			float _color_distort_power;
			float _Alpha_Constart;
			float _color_Constart;
			float _Desaturate;
			float _frensel;
			float _Color_power;
			float _Constart;
			float _Base_distort_power;
			float _Distort;
			float _Zwrite;
			float _Ztest;
			float _Alpha;
			float _frensel_edge;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Base_Tex;
			sampler2D _Distort_Tex;
			sampler2D _color_Tex;
			sampler2D _Dissolve_Tex;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Base_mask_Tex;


			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 vertexPos138 = v.vertex.xyz;
				float4 ase_clipPos138 = TransformObjectToHClip((vertexPos138).xyz);
				float4 screenPos138 = ComputeScreenPos(ase_clipPos138);
				o.ase_texcoord5 = screenPos138;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord7.xyz = ase_worldNormal;
				
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				o.ase_texcoord6 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN
				#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 break143 = _Base_speed;
				float mulTime141 = _TimeParameters.x * break143.z;
				float2 appendResult142 = (float2(break143.x , break143.y));
				float2 appendResult97 = (float2(_Base_uv.x , _Base_uv.y));
				float2 appendResult98 = (float2(_Base_uv.z , _Base_uv.w));
				float2 texCoord95 = IN.ase_texcoord3.xy * appendResult97 + appendResult98;
				float2 panner140 = ( mulTime141 * appendResult142 + texCoord95);
				float4 texCoord92 = IN.ase_texcoord4;
				texCoord92.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult90 = (float2(texCoord92.x , texCoord92.y));
				#ifdef _USE_CUSTOM1_ZW_MOVE_ON
				float2 staticSwitch93 = ( panner140 + appendResult90 );
				#else
				float2 staticSwitch93 = panner140;
				#endif
				float4 break211 = _Distort_speed1;
				float mulTime212 = _TimeParameters.x * break211.z;
				float2 appendResult213 = (float2(break211.x , break211.y));
				float2 appendResult210 = (float2(_Distort_uv1.x , _Distort_uv1.y));
				float2 appendResult209 = (float2(_Distort_uv1.z , _Distort_uv1.w));
				float2 texCoord214 = IN.ase_texcoord3.xy * appendResult210 + appendResult209;
				float2 panner215 = ( mulTime212 * appendResult213 + texCoord214);
				float3 tex2DNode125 = UnpackNormalScale( tex2D( _Distort_Tex, panner215 ), 1.0f );
				float2 appendResult123 = (float2(tex2DNode125.r , tex2DNode125.g));
				float2 appendResult226 = (float2(_direction.z , _direction.w));
				float4 texCoord116 = IN.ase_texcoord3;
				texCoord116.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISTORT_ON
				float staticSwitch118 = texCoord116.w;
				#else
				float staticSwitch118 = _Distort;
				#endif
				float2 Distort126 = ( ( appendResult123 * appendResult226 ) * staticSwitch118 );
				float4 tex2DNode20 = tex2D( _Base_Tex, ( staticSwitch93 + ( Distort126 * _Base_distort_power ) ) );
				float4 lerpResult130 = lerp( _R_dark_clolor , _R_light_color , pow( tex2DNode20.r , _Constart ));
				float4 break195 = _color_speed;
				float mulTime200 = _TimeParameters.x * break195.z;
				float2 appendResult199 = (float2(break195.x , break195.y));
				float2 appendResult196 = (float2(_color_uv.x , _color_uv.y));
				float2 appendResult197 = (float2(_color_uv.z , _color_uv.w));
				float2 texCoord198 = IN.ase_texcoord3.xy * appendResult196 + appendResult197;
				float2 panner201 = ( mulTime200 * appendResult199 + texCoord198);
				float3 desaturateInitialColor232 = tex2D( _color_Tex, ( panner201 + ( Distort126 * _color_distort_power ) ) ).rgb;
				float desaturateDot232 = dot( desaturateInitialColor232, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar232 = lerp( desaturateInitialColor232, desaturateDot232.xxx, _Desaturate );
				float3 temp_cast_1 = (_color_Constart).xxx;
				float3 color_part206 = pow( desaturateVar232 , temp_cast_1 );
				float3 temp_output_33_0 = (( _Color * lerpResult130 * _Color_power * IN.ase_color * float4( color_part206 , 0.0 ) )).rgb;
				
				float4 break156 = _Dissolve_speed;
				float mulTime154 = _TimeParameters.x * break156.z;
				float2 appendResult153 = (float2(break156.x , break156.y));
				float2 appendResult150 = (float2(_Dissolve_uv.x , _Dissolve_uv.y));
				float2 appendResult151 = (float2(_Dissolve_uv.z , _Dissolve_uv.w));
				float2 texCoord152 = IN.ase_texcoord3.xy * appendResult150 + appendResult151;
				float2 panner155 = ( mulTime154 * appendResult153 + texCoord152);
				float4 texCoord113 = IN.ase_texcoord3;
				texCoord113.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
				float staticSwitch114 = texCoord113.z;
				#else
				float staticSwitch114 = _Dissolve;
				#endif
				float lerpResult107 = lerp( _Hardness , -1.0 , staticSwitch114);
				float2 uv_Mask = IN.ase_texcoord3.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 screenPos138 = IN.ase_texcoord5;
				float4 ase_screenPosNorm138 = screenPos138 / screenPos138.w;
				ase_screenPosNorm138.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm138.z : ase_screenPosNorm138.z * 0.5 + 0.5;
				float screenDepth138 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm138.xy ),_ZBufferParams);
				float distanceDepth138 = abs( ( screenDepth138 - LinearEyeDepth( ase_screenPosNorm138.z,_ZBufferParams ) ) / ( _Distance ) );
				float4 break163 = _Base_mask_speed;
				float mulTime167 = _TimeParameters.x * break163.z;
				float2 appendResult166 = (float2(break163.x , break163.y));
				float2 appendResult162 = (float2(_Base_mask_uv.x , _Base_mask_uv.y));
				float2 appendResult164 = (float2(_Base_mask_uv.z , _Base_mask_uv.w));
				float2 texCoord165 = IN.ase_texcoord3.xy * appendResult162 + appendResult164;
				float2 panner168 = ( mulTime167 * appendResult166 + texCoord165);
				float2 temp_output_170_0 = ( panner168 + ( Distort126 * _Mask_distort_power ) );
				float4 texCoord173 = IN.ase_texcoord6;
				texCoord173.xy = IN.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult174 = (float2(texCoord173.x , texCoord173.y));
				#ifdef _CUSTOM2_ZW_MOVE_MASK_ON
				float2 staticSwitch176 = ( temp_output_170_0 + appendResult174 );
				#else
				float2 staticSwitch176 = temp_output_170_0;
				#endif
				float Base_mask177 = ( pow( tex2D( _Base_mask_Tex, staticSwitch176 ).r , _mask_Constart ) * _mask_power );
				float3 ase_worldNormal = IN.ase_texcoord7.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult181 = dot( ase_worldNormal , ase_worldViewDir );
				float temp_output_182_0 = abs( dotResult181 );
				#ifdef _FRENSEL_FLIP_ON
				float staticSwitch189 = ( 1.0 - temp_output_182_0 );
				#else
				float staticSwitch189 = temp_output_182_0;
				#endif
				float smoothstepResult186 = smoothstep( _frensel , ( _frensel + _frensel_edge ) , staticSwitch189);
				#ifdef _USE_FRENSEL_ON
				float staticSwitch187 = smoothstepResult186;
				#else
				float staticSwitch187 = 1.0;
				#endif
				float frensel188 = staticSwitch187;
				float temp_output_35_0 = saturate( ( IN.ase_color.a * pow( tex2DNode20.r , _Alpha_Constart ) * _Alpha * saturate( ( ( tex2D( _Dissolve_Tex, ( panner155 + ( Distort126 * _Dissolve_distort_power1 ) ) ).r * _Hardness ) - lerpResult107 ) ) * tex2D( _Mask, uv_Mask ).r * saturate( distanceDepth138 ) * Base_mask177 * frensel188 ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = temp_output_33_0;
				float Alpha = temp_output_35_0;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.clipPos, Color);
				#endif

				#if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140012
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _USE_CUSTOM1_ZW_MOVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISTORT_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON
			#pragma shader_feature_local _CUSTOM2_ZW_MOVE_MASK_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_speed;
			float4 _color_speed;
			float4 _Base_mask_speed;
			float4 _Base_mask_uv;
			float4 _Dissolve_uv;
			float4 _direction;
			float4 _Distort_uv1;
			float4 _color_uv;
			float4 _Distort_speed1;
			float4 _Base_speed;
			float4 _R_light_color;
			float4 _R_dark_clolor;
			float4 _Color;
			float4 _Base_uv;
			float4 _Mask_ST;
			float _Hardness;
			float _Dissolve;
			float _Dissolve_distort_power1;
			float _Mask_distort_power;
			float _mask_Constart;
			float _mask_power;
			float _Distance;
			float _Cull;
			float _color_distort_power;
			float _Alpha_Constart;
			float _color_Constart;
			float _Desaturate;
			float _frensel;
			float _Color_power;
			float _Constart;
			float _Base_distort_power;
			float _Distort;
			float _Zwrite;
			float _Ztest;
			float _Alpha;
			float _frensel_edge;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Base_Tex;
			sampler2D _Distort_Tex;
			sampler2D _Dissolve_Tex;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Base_mask_Tex;


			
			float3 _LightDirection;
			float3 _LightPosition;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 vertexPos138 = v.vertex.xyz;
				float4 ase_clipPos138 = TransformObjectToHClip((vertexPos138).xyz);
				float4 screenPos138 = ComputeScreenPos(ase_clipPos138);
				o.ase_texcoord4 = screenPos138;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord6.xyz = ase_worldNormal;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord1;
				o.ase_texcoord5 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.ase_normal );

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = clipPos;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 break143 = _Base_speed;
				float mulTime141 = _TimeParameters.x * break143.z;
				float2 appendResult142 = (float2(break143.x , break143.y));
				float2 appendResult97 = (float2(_Base_uv.x , _Base_uv.y));
				float2 appendResult98 = (float2(_Base_uv.z , _Base_uv.w));
				float2 texCoord95 = IN.ase_texcoord2.xy * appendResult97 + appendResult98;
				float2 panner140 = ( mulTime141 * appendResult142 + texCoord95);
				float4 texCoord92 = IN.ase_texcoord3;
				texCoord92.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult90 = (float2(texCoord92.x , texCoord92.y));
				#ifdef _USE_CUSTOM1_ZW_MOVE_ON
				float2 staticSwitch93 = ( panner140 + appendResult90 );
				#else
				float2 staticSwitch93 = panner140;
				#endif
				float4 break211 = _Distort_speed1;
				float mulTime212 = _TimeParameters.x * break211.z;
				float2 appendResult213 = (float2(break211.x , break211.y));
				float2 appendResult210 = (float2(_Distort_uv1.x , _Distort_uv1.y));
				float2 appendResult209 = (float2(_Distort_uv1.z , _Distort_uv1.w));
				float2 texCoord214 = IN.ase_texcoord2.xy * appendResult210 + appendResult209;
				float2 panner215 = ( mulTime212 * appendResult213 + texCoord214);
				float3 tex2DNode125 = UnpackNormalScale( tex2D( _Distort_Tex, panner215 ), 1.0f );
				float2 appendResult123 = (float2(tex2DNode125.r , tex2DNode125.g));
				float2 appendResult226 = (float2(_direction.z , _direction.w));
				float4 texCoord116 = IN.ase_texcoord2;
				texCoord116.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISTORT_ON
				float staticSwitch118 = texCoord116.w;
				#else
				float staticSwitch118 = _Distort;
				#endif
				float2 Distort126 = ( ( appendResult123 * appendResult226 ) * staticSwitch118 );
				float4 tex2DNode20 = tex2D( _Base_Tex, ( staticSwitch93 + ( Distort126 * _Base_distort_power ) ) );
				float4 break156 = _Dissolve_speed;
				float mulTime154 = _TimeParameters.x * break156.z;
				float2 appendResult153 = (float2(break156.x , break156.y));
				float2 appendResult150 = (float2(_Dissolve_uv.x , _Dissolve_uv.y));
				float2 appendResult151 = (float2(_Dissolve_uv.z , _Dissolve_uv.w));
				float2 texCoord152 = IN.ase_texcoord2.xy * appendResult150 + appendResult151;
				float2 panner155 = ( mulTime154 * appendResult153 + texCoord152);
				float4 texCoord113 = IN.ase_texcoord2;
				texCoord113.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
				float staticSwitch114 = texCoord113.z;
				#else
				float staticSwitch114 = _Dissolve;
				#endif
				float lerpResult107 = lerp( _Hardness , -1.0 , staticSwitch114);
				float2 uv_Mask = IN.ase_texcoord2.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 screenPos138 = IN.ase_texcoord4;
				float4 ase_screenPosNorm138 = screenPos138 / screenPos138.w;
				ase_screenPosNorm138.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm138.z : ase_screenPosNorm138.z * 0.5 + 0.5;
				float screenDepth138 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm138.xy ),_ZBufferParams);
				float distanceDepth138 = abs( ( screenDepth138 - LinearEyeDepth( ase_screenPosNorm138.z,_ZBufferParams ) ) / ( _Distance ) );
				float4 break163 = _Base_mask_speed;
				float mulTime167 = _TimeParameters.x * break163.z;
				float2 appendResult166 = (float2(break163.x , break163.y));
				float2 appendResult162 = (float2(_Base_mask_uv.x , _Base_mask_uv.y));
				float2 appendResult164 = (float2(_Base_mask_uv.z , _Base_mask_uv.w));
				float2 texCoord165 = IN.ase_texcoord2.xy * appendResult162 + appendResult164;
				float2 panner168 = ( mulTime167 * appendResult166 + texCoord165);
				float2 temp_output_170_0 = ( panner168 + ( Distort126 * _Mask_distort_power ) );
				float4 texCoord173 = IN.ase_texcoord5;
				texCoord173.xy = IN.ase_texcoord5.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult174 = (float2(texCoord173.x , texCoord173.y));
				#ifdef _CUSTOM2_ZW_MOVE_MASK_ON
				float2 staticSwitch176 = ( temp_output_170_0 + appendResult174 );
				#else
				float2 staticSwitch176 = temp_output_170_0;
				#endif
				float Base_mask177 = ( pow( tex2D( _Base_mask_Tex, staticSwitch176 ).r , _mask_Constart ) * _mask_power );
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult181 = dot( ase_worldNormal , ase_worldViewDir );
				float temp_output_182_0 = abs( dotResult181 );
				#ifdef _FRENSEL_FLIP_ON
				float staticSwitch189 = ( 1.0 - temp_output_182_0 );
				#else
				float staticSwitch189 = temp_output_182_0;
				#endif
				float smoothstepResult186 = smoothstep( _frensel , ( _frensel + _frensel_edge ) , staticSwitch189);
				#ifdef _USE_FRENSEL_ON
				float staticSwitch187 = smoothstepResult186;
				#else
				float staticSwitch187 = 1.0;
				#endif
				float frensel188 = staticSwitch187;
				float temp_output_35_0 = saturate( ( IN.ase_color.a * pow( tex2DNode20.r , _Alpha_Constart ) * _Alpha * saturate( ( ( tex2D( _Dissolve_Tex, ( panner155 + ( Distort126 * _Dissolve_distort_power1 ) ) ).r * _Hardness ) - lerpResult107 ) ) * tex2D( _Mask, uv_Mask ).r * saturate( distanceDepth138 ) * Base_mask177 * frensel188 ) );
				

				float Alpha = temp_output_35_0;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140012
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _USE_CUSTOM1_ZW_MOVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISTORT_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON
			#pragma shader_feature_local _CUSTOM2_ZW_MOVE_MASK_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_speed;
			float4 _color_speed;
			float4 _Base_mask_speed;
			float4 _Base_mask_uv;
			float4 _Dissolve_uv;
			float4 _direction;
			float4 _Distort_uv1;
			float4 _color_uv;
			float4 _Distort_speed1;
			float4 _Base_speed;
			float4 _R_light_color;
			float4 _R_dark_clolor;
			float4 _Color;
			float4 _Base_uv;
			float4 _Mask_ST;
			float _Hardness;
			float _Dissolve;
			float _Dissolve_distort_power1;
			float _Mask_distort_power;
			float _mask_Constart;
			float _mask_power;
			float _Distance;
			float _Cull;
			float _color_distort_power;
			float _Alpha_Constart;
			float _color_Constart;
			float _Desaturate;
			float _frensel;
			float _Color_power;
			float _Constart;
			float _Base_distort_power;
			float _Distort;
			float _Zwrite;
			float _Ztest;
			float _Alpha;
			float _frensel_edge;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Base_Tex;
			sampler2D _Distort_Tex;
			sampler2D _Dissolve_Tex;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Base_mask_Tex;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 vertexPos138 = v.vertex.xyz;
				float4 ase_clipPos138 = TransformObjectToHClip((vertexPos138).xyz);
				float4 screenPos138 = ComputeScreenPos(ase_clipPos138);
				o.ase_texcoord4 = screenPos138;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord6.xyz = ase_worldNormal;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord1;
				o.ase_texcoord5 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 break143 = _Base_speed;
				float mulTime141 = _TimeParameters.x * break143.z;
				float2 appendResult142 = (float2(break143.x , break143.y));
				float2 appendResult97 = (float2(_Base_uv.x , _Base_uv.y));
				float2 appendResult98 = (float2(_Base_uv.z , _Base_uv.w));
				float2 texCoord95 = IN.ase_texcoord2.xy * appendResult97 + appendResult98;
				float2 panner140 = ( mulTime141 * appendResult142 + texCoord95);
				float4 texCoord92 = IN.ase_texcoord3;
				texCoord92.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult90 = (float2(texCoord92.x , texCoord92.y));
				#ifdef _USE_CUSTOM1_ZW_MOVE_ON
				float2 staticSwitch93 = ( panner140 + appendResult90 );
				#else
				float2 staticSwitch93 = panner140;
				#endif
				float4 break211 = _Distort_speed1;
				float mulTime212 = _TimeParameters.x * break211.z;
				float2 appendResult213 = (float2(break211.x , break211.y));
				float2 appendResult210 = (float2(_Distort_uv1.x , _Distort_uv1.y));
				float2 appendResult209 = (float2(_Distort_uv1.z , _Distort_uv1.w));
				float2 texCoord214 = IN.ase_texcoord2.xy * appendResult210 + appendResult209;
				float2 panner215 = ( mulTime212 * appendResult213 + texCoord214);
				float3 tex2DNode125 = UnpackNormalScale( tex2D( _Distort_Tex, panner215 ), 1.0f );
				float2 appendResult123 = (float2(tex2DNode125.r , tex2DNode125.g));
				float2 appendResult226 = (float2(_direction.z , _direction.w));
				float4 texCoord116 = IN.ase_texcoord2;
				texCoord116.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISTORT_ON
				float staticSwitch118 = texCoord116.w;
				#else
				float staticSwitch118 = _Distort;
				#endif
				float2 Distort126 = ( ( appendResult123 * appendResult226 ) * staticSwitch118 );
				float4 tex2DNode20 = tex2D( _Base_Tex, ( staticSwitch93 + ( Distort126 * _Base_distort_power ) ) );
				float4 break156 = _Dissolve_speed;
				float mulTime154 = _TimeParameters.x * break156.z;
				float2 appendResult153 = (float2(break156.x , break156.y));
				float2 appendResult150 = (float2(_Dissolve_uv.x , _Dissolve_uv.y));
				float2 appendResult151 = (float2(_Dissolve_uv.z , _Dissolve_uv.w));
				float2 texCoord152 = IN.ase_texcoord2.xy * appendResult150 + appendResult151;
				float2 panner155 = ( mulTime154 * appendResult153 + texCoord152);
				float4 texCoord113 = IN.ase_texcoord2;
				texCoord113.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
				float staticSwitch114 = texCoord113.z;
				#else
				float staticSwitch114 = _Dissolve;
				#endif
				float lerpResult107 = lerp( _Hardness , -1.0 , staticSwitch114);
				float2 uv_Mask = IN.ase_texcoord2.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 screenPos138 = IN.ase_texcoord4;
				float4 ase_screenPosNorm138 = screenPos138 / screenPos138.w;
				ase_screenPosNorm138.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm138.z : ase_screenPosNorm138.z * 0.5 + 0.5;
				float screenDepth138 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm138.xy ),_ZBufferParams);
				float distanceDepth138 = abs( ( screenDepth138 - LinearEyeDepth( ase_screenPosNorm138.z,_ZBufferParams ) ) / ( _Distance ) );
				float4 break163 = _Base_mask_speed;
				float mulTime167 = _TimeParameters.x * break163.z;
				float2 appendResult166 = (float2(break163.x , break163.y));
				float2 appendResult162 = (float2(_Base_mask_uv.x , _Base_mask_uv.y));
				float2 appendResult164 = (float2(_Base_mask_uv.z , _Base_mask_uv.w));
				float2 texCoord165 = IN.ase_texcoord2.xy * appendResult162 + appendResult164;
				float2 panner168 = ( mulTime167 * appendResult166 + texCoord165);
				float2 temp_output_170_0 = ( panner168 + ( Distort126 * _Mask_distort_power ) );
				float4 texCoord173 = IN.ase_texcoord5;
				texCoord173.xy = IN.ase_texcoord5.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult174 = (float2(texCoord173.x , texCoord173.y));
				#ifdef _CUSTOM2_ZW_MOVE_MASK_ON
				float2 staticSwitch176 = ( temp_output_170_0 + appendResult174 );
				#else
				float2 staticSwitch176 = temp_output_170_0;
				#endif
				float Base_mask177 = ( pow( tex2D( _Base_mask_Tex, staticSwitch176 ).r , _mask_Constart ) * _mask_power );
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult181 = dot( ase_worldNormal , ase_worldViewDir );
				float temp_output_182_0 = abs( dotResult181 );
				#ifdef _FRENSEL_FLIP_ON
				float staticSwitch189 = ( 1.0 - temp_output_182_0 );
				#else
				float staticSwitch189 = temp_output_182_0;
				#endif
				float smoothstepResult186 = smoothstep( _frensel , ( _frensel + _frensel_edge ) , staticSwitch189);
				#ifdef _USE_FRENSEL_ON
				float staticSwitch187 = smoothstepResult186;
				#else
				float staticSwitch187 = 1.0;
				#endif
				float frensel188 = staticSwitch187;
				float temp_output_35_0 = saturate( ( IN.ase_color.a * pow( tex2DNode20.r , _Alpha_Constart ) * _Alpha * saturate( ( ( tex2D( _Dissolve_Tex, ( panner155 + ( Distort126 * _Dissolve_distort_power1 ) ) ).r * _Hardness ) - lerpResult107 ) ) * tex2D( _Mask, uv_Mask ).r * saturate( distanceDepth138 ) * Base_mask177 * frensel188 ) );
				

				float Alpha = temp_output_35_0;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140012
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _USE_CUSTOM1_ZW_MOVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISTORT_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON
			#pragma shader_feature_local _CUSTOM2_ZW_MOVE_MASK_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_speed;
			float4 _color_speed;
			float4 _Base_mask_speed;
			float4 _Base_mask_uv;
			float4 _Dissolve_uv;
			float4 _direction;
			float4 _Distort_uv1;
			float4 _color_uv;
			float4 _Distort_speed1;
			float4 _Base_speed;
			float4 _R_light_color;
			float4 _R_dark_clolor;
			float4 _Color;
			float4 _Base_uv;
			float4 _Mask_ST;
			float _Hardness;
			float _Dissolve;
			float _Dissolve_distort_power1;
			float _Mask_distort_power;
			float _mask_Constart;
			float _mask_power;
			float _Distance;
			float _Cull;
			float _color_distort_power;
			float _Alpha_Constart;
			float _color_Constart;
			float _Desaturate;
			float _frensel;
			float _Color_power;
			float _Constart;
			float _Base_distort_power;
			float _Distort;
			float _Zwrite;
			float _Ztest;
			float _Alpha;
			float _frensel_edge;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Base_Tex;
			sampler2D _Distort_Tex;
			sampler2D _Dissolve_Tex;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Base_mask_Tex;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 vertexPos138 = v.vertex.xyz;
				float4 ase_clipPos138 = TransformObjectToHClip((vertexPos138).xyz);
				float4 screenPos138 = ComputeScreenPos(ase_clipPos138);
				o.ase_texcoord2 = screenPos138;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord5.xyz = ase_worldPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord3 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 break143 = _Base_speed;
				float mulTime141 = _TimeParameters.x * break143.z;
				float2 appendResult142 = (float2(break143.x , break143.y));
				float2 appendResult97 = (float2(_Base_uv.x , _Base_uv.y));
				float2 appendResult98 = (float2(_Base_uv.z , _Base_uv.w));
				float2 texCoord95 = IN.ase_texcoord.xy * appendResult97 + appendResult98;
				float2 panner140 = ( mulTime141 * appendResult142 + texCoord95);
				float4 texCoord92 = IN.ase_texcoord1;
				texCoord92.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult90 = (float2(texCoord92.x , texCoord92.y));
				#ifdef _USE_CUSTOM1_ZW_MOVE_ON
				float2 staticSwitch93 = ( panner140 + appendResult90 );
				#else
				float2 staticSwitch93 = panner140;
				#endif
				float4 break211 = _Distort_speed1;
				float mulTime212 = _TimeParameters.x * break211.z;
				float2 appendResult213 = (float2(break211.x , break211.y));
				float2 appendResult210 = (float2(_Distort_uv1.x , _Distort_uv1.y));
				float2 appendResult209 = (float2(_Distort_uv1.z , _Distort_uv1.w));
				float2 texCoord214 = IN.ase_texcoord.xy * appendResult210 + appendResult209;
				float2 panner215 = ( mulTime212 * appendResult213 + texCoord214);
				float3 tex2DNode125 = UnpackNormalScale( tex2D( _Distort_Tex, panner215 ), 1.0f );
				float2 appendResult123 = (float2(tex2DNode125.r , tex2DNode125.g));
				float2 appendResult226 = (float2(_direction.z , _direction.w));
				float4 texCoord116 = IN.ase_texcoord;
				texCoord116.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISTORT_ON
				float staticSwitch118 = texCoord116.w;
				#else
				float staticSwitch118 = _Distort;
				#endif
				float2 Distort126 = ( ( appendResult123 * appendResult226 ) * staticSwitch118 );
				float4 tex2DNode20 = tex2D( _Base_Tex, ( staticSwitch93 + ( Distort126 * _Base_distort_power ) ) );
				float4 break156 = _Dissolve_speed;
				float mulTime154 = _TimeParameters.x * break156.z;
				float2 appendResult153 = (float2(break156.x , break156.y));
				float2 appendResult150 = (float2(_Dissolve_uv.x , _Dissolve_uv.y));
				float2 appendResult151 = (float2(_Dissolve_uv.z , _Dissolve_uv.w));
				float2 texCoord152 = IN.ase_texcoord.xy * appendResult150 + appendResult151;
				float2 panner155 = ( mulTime154 * appendResult153 + texCoord152);
				float4 texCoord113 = IN.ase_texcoord;
				texCoord113.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
				float staticSwitch114 = texCoord113.z;
				#else
				float staticSwitch114 = _Dissolve;
				#endif
				float lerpResult107 = lerp( _Hardness , -1.0 , staticSwitch114);
				float2 uv_Mask = IN.ase_texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 screenPos138 = IN.ase_texcoord2;
				float4 ase_screenPosNorm138 = screenPos138 / screenPos138.w;
				ase_screenPosNorm138.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm138.z : ase_screenPosNorm138.z * 0.5 + 0.5;
				float screenDepth138 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm138.xy ),_ZBufferParams);
				float distanceDepth138 = abs( ( screenDepth138 - LinearEyeDepth( ase_screenPosNorm138.z,_ZBufferParams ) ) / ( _Distance ) );
				float4 break163 = _Base_mask_speed;
				float mulTime167 = _TimeParameters.x * break163.z;
				float2 appendResult166 = (float2(break163.x , break163.y));
				float2 appendResult162 = (float2(_Base_mask_uv.x , _Base_mask_uv.y));
				float2 appendResult164 = (float2(_Base_mask_uv.z , _Base_mask_uv.w));
				float2 texCoord165 = IN.ase_texcoord.xy * appendResult162 + appendResult164;
				float2 panner168 = ( mulTime167 * appendResult166 + texCoord165);
				float2 temp_output_170_0 = ( panner168 + ( Distort126 * _Mask_distort_power ) );
				float4 texCoord173 = IN.ase_texcoord3;
				texCoord173.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult174 = (float2(texCoord173.x , texCoord173.y));
				#ifdef _CUSTOM2_ZW_MOVE_MASK_ON
				float2 staticSwitch176 = ( temp_output_170_0 + appendResult174 );
				#else
				float2 staticSwitch176 = temp_output_170_0;
				#endif
				float Base_mask177 = ( pow( tex2D( _Base_mask_Tex, staticSwitch176 ).r , _mask_Constart ) * _mask_power );
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float3 ase_worldPos = IN.ase_texcoord5.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult181 = dot( ase_worldNormal , ase_worldViewDir );
				float temp_output_182_0 = abs( dotResult181 );
				#ifdef _FRENSEL_FLIP_ON
				float staticSwitch189 = ( 1.0 - temp_output_182_0 );
				#else
				float staticSwitch189 = temp_output_182_0;
				#endif
				float smoothstepResult186 = smoothstep( _frensel , ( _frensel + _frensel_edge ) , staticSwitch189);
				#ifdef _USE_FRENSEL_ON
				float staticSwitch187 = smoothstepResult186;
				#else
				float staticSwitch187 = 1.0;
				#endif
				float frensel188 = staticSwitch187;
				float temp_output_35_0 = saturate( ( IN.ase_color.a * pow( tex2DNode20.r , _Alpha_Constart ) * _Alpha * saturate( ( ( tex2D( _Dissolve_Tex, ( panner155 + ( Distort126 * _Dissolve_distort_power1 ) ) ).r * _Hardness ) - lerpResult107 ) ) * tex2D( _Mask, uv_Mask ).r * saturate( distanceDepth138 ) * Base_mask177 * frensel188 ) );
				

				surfaceDescription.Alpha = temp_output_35_0;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140012
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _USE_CUSTOM1_ZW_MOVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISTORT_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON
			#pragma shader_feature_local _CUSTOM2_ZW_MOVE_MASK_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_speed;
			float4 _color_speed;
			float4 _Base_mask_speed;
			float4 _Base_mask_uv;
			float4 _Dissolve_uv;
			float4 _direction;
			float4 _Distort_uv1;
			float4 _color_uv;
			float4 _Distort_speed1;
			float4 _Base_speed;
			float4 _R_light_color;
			float4 _R_dark_clolor;
			float4 _Color;
			float4 _Base_uv;
			float4 _Mask_ST;
			float _Hardness;
			float _Dissolve;
			float _Dissolve_distort_power1;
			float _Mask_distort_power;
			float _mask_Constart;
			float _mask_power;
			float _Distance;
			float _Cull;
			float _color_distort_power;
			float _Alpha_Constart;
			float _color_Constart;
			float _Desaturate;
			float _frensel;
			float _Color_power;
			float _Constart;
			float _Base_distort_power;
			float _Distort;
			float _Zwrite;
			float _Ztest;
			float _Alpha;
			float _frensel_edge;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Base_Tex;
			sampler2D _Distort_Tex;
			sampler2D _Dissolve_Tex;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Base_mask_Tex;


			
			float4 _SelectionID;


			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 vertexPos138 = v.vertex.xyz;
				float4 ase_clipPos138 = TransformObjectToHClip((vertexPos138).xyz);
				float4 screenPos138 = ComputeScreenPos(ase_clipPos138);
				o.ase_texcoord2 = screenPos138;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord5.xyz = ase_worldPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord3 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 break143 = _Base_speed;
				float mulTime141 = _TimeParameters.x * break143.z;
				float2 appendResult142 = (float2(break143.x , break143.y));
				float2 appendResult97 = (float2(_Base_uv.x , _Base_uv.y));
				float2 appendResult98 = (float2(_Base_uv.z , _Base_uv.w));
				float2 texCoord95 = IN.ase_texcoord.xy * appendResult97 + appendResult98;
				float2 panner140 = ( mulTime141 * appendResult142 + texCoord95);
				float4 texCoord92 = IN.ase_texcoord1;
				texCoord92.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult90 = (float2(texCoord92.x , texCoord92.y));
				#ifdef _USE_CUSTOM1_ZW_MOVE_ON
				float2 staticSwitch93 = ( panner140 + appendResult90 );
				#else
				float2 staticSwitch93 = panner140;
				#endif
				float4 break211 = _Distort_speed1;
				float mulTime212 = _TimeParameters.x * break211.z;
				float2 appendResult213 = (float2(break211.x , break211.y));
				float2 appendResult210 = (float2(_Distort_uv1.x , _Distort_uv1.y));
				float2 appendResult209 = (float2(_Distort_uv1.z , _Distort_uv1.w));
				float2 texCoord214 = IN.ase_texcoord.xy * appendResult210 + appendResult209;
				float2 panner215 = ( mulTime212 * appendResult213 + texCoord214);
				float3 tex2DNode125 = UnpackNormalScale( tex2D( _Distort_Tex, panner215 ), 1.0f );
				float2 appendResult123 = (float2(tex2DNode125.r , tex2DNode125.g));
				float2 appendResult226 = (float2(_direction.z , _direction.w));
				float4 texCoord116 = IN.ase_texcoord;
				texCoord116.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISTORT_ON
				float staticSwitch118 = texCoord116.w;
				#else
				float staticSwitch118 = _Distort;
				#endif
				float2 Distort126 = ( ( appendResult123 * appendResult226 ) * staticSwitch118 );
				float4 tex2DNode20 = tex2D( _Base_Tex, ( staticSwitch93 + ( Distort126 * _Base_distort_power ) ) );
				float4 break156 = _Dissolve_speed;
				float mulTime154 = _TimeParameters.x * break156.z;
				float2 appendResult153 = (float2(break156.x , break156.y));
				float2 appendResult150 = (float2(_Dissolve_uv.x , _Dissolve_uv.y));
				float2 appendResult151 = (float2(_Dissolve_uv.z , _Dissolve_uv.w));
				float2 texCoord152 = IN.ase_texcoord.xy * appendResult150 + appendResult151;
				float2 panner155 = ( mulTime154 * appendResult153 + texCoord152);
				float4 texCoord113 = IN.ase_texcoord;
				texCoord113.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
				float staticSwitch114 = texCoord113.z;
				#else
				float staticSwitch114 = _Dissolve;
				#endif
				float lerpResult107 = lerp( _Hardness , -1.0 , staticSwitch114);
				float2 uv_Mask = IN.ase_texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 screenPos138 = IN.ase_texcoord2;
				float4 ase_screenPosNorm138 = screenPos138 / screenPos138.w;
				ase_screenPosNorm138.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm138.z : ase_screenPosNorm138.z * 0.5 + 0.5;
				float screenDepth138 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm138.xy ),_ZBufferParams);
				float distanceDepth138 = abs( ( screenDepth138 - LinearEyeDepth( ase_screenPosNorm138.z,_ZBufferParams ) ) / ( _Distance ) );
				float4 break163 = _Base_mask_speed;
				float mulTime167 = _TimeParameters.x * break163.z;
				float2 appendResult166 = (float2(break163.x , break163.y));
				float2 appendResult162 = (float2(_Base_mask_uv.x , _Base_mask_uv.y));
				float2 appendResult164 = (float2(_Base_mask_uv.z , _Base_mask_uv.w));
				float2 texCoord165 = IN.ase_texcoord.xy * appendResult162 + appendResult164;
				float2 panner168 = ( mulTime167 * appendResult166 + texCoord165);
				float2 temp_output_170_0 = ( panner168 + ( Distort126 * _Mask_distort_power ) );
				float4 texCoord173 = IN.ase_texcoord3;
				texCoord173.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult174 = (float2(texCoord173.x , texCoord173.y));
				#ifdef _CUSTOM2_ZW_MOVE_MASK_ON
				float2 staticSwitch176 = ( temp_output_170_0 + appendResult174 );
				#else
				float2 staticSwitch176 = temp_output_170_0;
				#endif
				float Base_mask177 = ( pow( tex2D( _Base_mask_Tex, staticSwitch176 ).r , _mask_Constart ) * _mask_power );
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float3 ase_worldPos = IN.ase_texcoord5.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult181 = dot( ase_worldNormal , ase_worldViewDir );
				float temp_output_182_0 = abs( dotResult181 );
				#ifdef _FRENSEL_FLIP_ON
				float staticSwitch189 = ( 1.0 - temp_output_182_0 );
				#else
				float staticSwitch189 = temp_output_182_0;
				#endif
				float smoothstepResult186 = smoothstep( _frensel , ( _frensel + _frensel_edge ) , staticSwitch189);
				#ifdef _USE_FRENSEL_ON
				float staticSwitch187 = smoothstepResult186;
				#else
				float staticSwitch187 = 1.0;
				#endif
				float frensel188 = staticSwitch187;
				float temp_output_35_0 = saturate( ( IN.ase_color.a * pow( tex2DNode20.r , _Alpha_Constart ) * _Alpha * saturate( ( ( tex2D( _Dissolve_Tex, ( panner155 + ( Distort126 * _Dissolve_distort_power1 ) ) ).r * _Hardness ) - lerpResult107 ) ) * tex2D( _Mask, uv_Mask ).r * saturate( distanceDepth138 ) * Base_mask177 * frensel188 ) );
				

				surfaceDescription.Alpha = temp_output_35_0;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On


			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140012
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _USE_CUSTOM1_ZW_MOVE_ON
			#pragma shader_feature_local _USE_CUSTOM1_Y_DISTORT_ON
			#pragma shader_feature_local _USE_CUSTOM1_X_DISSOLVE_ON
			#pragma shader_feature_local _CUSTOM2_ZW_MOVE_MASK_ON
			#pragma shader_feature_local _USE_FRENSEL_ON
			#pragma shader_feature_local _FRENSEL_FLIP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_speed;
			float4 _color_speed;
			float4 _Base_mask_speed;
			float4 _Base_mask_uv;
			float4 _Dissolve_uv;
			float4 _direction;
			float4 _Distort_uv1;
			float4 _color_uv;
			float4 _Distort_speed1;
			float4 _Base_speed;
			float4 _R_light_color;
			float4 _R_dark_clolor;
			float4 _Color;
			float4 _Base_uv;
			float4 _Mask_ST;
			float _Hardness;
			float _Dissolve;
			float _Dissolve_distort_power1;
			float _Mask_distort_power;
			float _mask_Constart;
			float _mask_power;
			float _Distance;
			float _Cull;
			float _color_distort_power;
			float _Alpha_Constart;
			float _color_Constart;
			float _Desaturate;
			float _frensel;
			float _Color_power;
			float _Constart;
			float _Base_distort_power;
			float _Distort;
			float _Zwrite;
			float _Ztest;
			float _Alpha;
			float _frensel_edge;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Base_Tex;
			sampler2D _Distort_Tex;
			sampler2D _Dissolve_Tex;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _Base_mask_Tex;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 vertexPos138 = v.vertex.xyz;
				float4 ase_clipPos138 = TransformObjectToHClip((vertexPos138).xyz);
				float4 screenPos138 = ComputeScreenPos(ase_clipPos138);
				o.ase_texcoord3 = screenPos138;
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord5.xyz = ase_worldPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord1;
				o.ase_texcoord4 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord5.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

				o.clipPos = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			void frag( VertexOutput IN
				, out half4 outNormalWS : SV_Target0
			#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
			#endif
				 )
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 break143 = _Base_speed;
				float mulTime141 = _TimeParameters.x * break143.z;
				float2 appendResult142 = (float2(break143.x , break143.y));
				float2 appendResult97 = (float2(_Base_uv.x , _Base_uv.y));
				float2 appendResult98 = (float2(_Base_uv.z , _Base_uv.w));
				float2 texCoord95 = IN.ase_texcoord1.xy * appendResult97 + appendResult98;
				float2 panner140 = ( mulTime141 * appendResult142 + texCoord95);
				float4 texCoord92 = IN.ase_texcoord2;
				texCoord92.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult90 = (float2(texCoord92.x , texCoord92.y));
				#ifdef _USE_CUSTOM1_ZW_MOVE_ON
				float2 staticSwitch93 = ( panner140 + appendResult90 );
				#else
				float2 staticSwitch93 = panner140;
				#endif
				float4 break211 = _Distort_speed1;
				float mulTime212 = _TimeParameters.x * break211.z;
				float2 appendResult213 = (float2(break211.x , break211.y));
				float2 appendResult210 = (float2(_Distort_uv1.x , _Distort_uv1.y));
				float2 appendResult209 = (float2(_Distort_uv1.z , _Distort_uv1.w));
				float2 texCoord214 = IN.ase_texcoord1.xy * appendResult210 + appendResult209;
				float2 panner215 = ( mulTime212 * appendResult213 + texCoord214);
				float3 tex2DNode125 = UnpackNormalScale( tex2D( _Distort_Tex, panner215 ), 1.0f );
				float2 appendResult123 = (float2(tex2DNode125.r , tex2DNode125.g));
				float2 appendResult226 = (float2(_direction.z , _direction.w));
				float4 texCoord116 = IN.ase_texcoord1;
				texCoord116.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_Y_DISTORT_ON
				float staticSwitch118 = texCoord116.w;
				#else
				float staticSwitch118 = _Distort;
				#endif
				float2 Distort126 = ( ( appendResult123 * appendResult226 ) * staticSwitch118 );
				float4 tex2DNode20 = tex2D( _Base_Tex, ( staticSwitch93 + ( Distort126 * _Base_distort_power ) ) );
				float4 break156 = _Dissolve_speed;
				float mulTime154 = _TimeParameters.x * break156.z;
				float2 appendResult153 = (float2(break156.x , break156.y));
				float2 appendResult150 = (float2(_Dissolve_uv.x , _Dissolve_uv.y));
				float2 appendResult151 = (float2(_Dissolve_uv.z , _Dissolve_uv.w));
				float2 texCoord152 = IN.ase_texcoord1.xy * appendResult150 + appendResult151;
				float2 panner155 = ( mulTime154 * appendResult153 + texCoord152);
				float4 texCoord113 = IN.ase_texcoord1;
				texCoord113.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_CUSTOM1_X_DISSOLVE_ON
				float staticSwitch114 = texCoord113.z;
				#else
				float staticSwitch114 = _Dissolve;
				#endif
				float lerpResult107 = lerp( _Hardness , -1.0 , staticSwitch114);
				float2 uv_Mask = IN.ase_texcoord1.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 screenPos138 = IN.ase_texcoord3;
				float4 ase_screenPosNorm138 = screenPos138 / screenPos138.w;
				ase_screenPosNorm138.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm138.z : ase_screenPosNorm138.z * 0.5 + 0.5;
				float screenDepth138 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm138.xy ),_ZBufferParams);
				float distanceDepth138 = abs( ( screenDepth138 - LinearEyeDepth( ase_screenPosNorm138.z,_ZBufferParams ) ) / ( _Distance ) );
				float4 break163 = _Base_mask_speed;
				float mulTime167 = _TimeParameters.x * break163.z;
				float2 appendResult166 = (float2(break163.x , break163.y));
				float2 appendResult162 = (float2(_Base_mask_uv.x , _Base_mask_uv.y));
				float2 appendResult164 = (float2(_Base_mask_uv.z , _Base_mask_uv.w));
				float2 texCoord165 = IN.ase_texcoord1.xy * appendResult162 + appendResult164;
				float2 panner168 = ( mulTime167 * appendResult166 + texCoord165);
				float2 temp_output_170_0 = ( panner168 + ( Distort126 * _Mask_distort_power ) );
				float4 texCoord173 = IN.ase_texcoord4;
				texCoord173.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult174 = (float2(texCoord173.x , texCoord173.y));
				#ifdef _CUSTOM2_ZW_MOVE_MASK_ON
				float2 staticSwitch176 = ( temp_output_170_0 + appendResult174 );
				#else
				float2 staticSwitch176 = temp_output_170_0;
				#endif
				float Base_mask177 = ( pow( tex2D( _Base_mask_Tex, staticSwitch176 ).r , _mask_Constart ) * _mask_power );
				float3 ase_worldPos = IN.ase_texcoord5.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult181 = dot( IN.normalWS , ase_worldViewDir );
				float temp_output_182_0 = abs( dotResult181 );
				#ifdef _FRENSEL_FLIP_ON
				float staticSwitch189 = ( 1.0 - temp_output_182_0 );
				#else
				float staticSwitch189 = temp_output_182_0;
				#endif
				float smoothstepResult186 = smoothstep( _frensel , ( _frensel + _frensel_edge ) , staticSwitch189);
				#ifdef _USE_FRENSEL_ON
				float staticSwitch187 = smoothstepResult186;
				#else
				float staticSwitch187 = 1.0;
				#endif
				float frensel188 = staticSwitch187;
				float temp_output_35_0 = saturate( ( IN.ase_color.a * pow( tex2DNode20.r , _Alpha_Constart ) * _Alpha * saturate( ( ( tex2D( _Dissolve_Tex, ( panner155 + ( Distort126 * _Dissolve_distort_power1 ) ) ).r * _Hardness ) - lerpResult107 ) ) * tex2D( _Mask, uv_Mask ).r * saturate( distanceDepth138 ) * Base_mask177 * frensel188 ) );
				

				surfaceDescription.Alpha = temp_output_35_0;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(IN.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = IN.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.Vector4Node;208;-5101.684,295.2681;Inherit;False;Property;_Distort_speed1;Distort_speed;35;0;Create;True;0;0;0;False;0;False;1,0,0,0;-1,-1,0.15,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;207;-5089.958,62.79051;Inherit;False;Property;_Distort_uv1;Distort_uv;34;0;Create;True;0;0;0;False;0;False;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;211;-4882.691,449.5041;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;209;-4570.409,286.849;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;210;-4553.409,68.84959;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;214;-4373.409,137.8495;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;212;-4506.652,579.6263;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;213;-4613.652,475.6263;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;215;-4043.652,272.6261;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;225;-3566.539,628.5038;Inherit;False;Property;_direction;direction;36;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;125;-3741.629,271.2452;Inherit;True;Property;_Distort_Tex;Distort_Tex;33;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;0630e81508c5cb94ba0691156c94fb02;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;116;-3093.166,694.3784;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;226;-3285.993,615.0851;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;123;-3270.061,314.5829;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-3070.986,524.7353;Inherit;False;Property;_Distort;Distort;37;0;Create;True;0;0;0;False;0;False;0;0.1;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;118;-2770.104,571.4073;Inherit;False;Property;_use_custom1_y_distort;use_custom1_y_distort;38;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-3108.993,433.0851;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-2828.665,291.0067;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;160;-2343.513,2224.938;Inherit;False;Property;_Base_mask_uv;Base_mask_uv;27;0;Create;True;0;0;0;False;0;False;1,1,0,0;3,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;161;-2355.239,2457.416;Inherit;False;Property;_Base_mask_speed;Base_mask_speed;28;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-1855.598,269.2661;Inherit;False;Distort;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;164;-1823.966,2448.997;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;163;-2136.247,2611.652;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;162;-1806.966,2230.997;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;165;-1626.966,2299.997;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;167;-1760.209,2741.774;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;96;-2764.991,-723.9532;Inherit;False;Property;_Base_uv;Base_uv;1;0;Create;True;0;0;0;False;0;False;1,1,0,0;3,1.8,0,-0.18;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;169;-1051.173,2361.968;Inherit;False;126;Distort;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;144;-2728.793,-350.7201;Inherit;False;Property;_Base_speed;Base_speed;2;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0.2,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;221;-1131.796,2599.84;Inherit;False;Property;_Mask_distort_power;Mask_distort_power;32;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;166;-1867.209,2637.774;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;193;-1096.882,4445.44;Inherit;False;Property;_color_speed;color_speed;15;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,1,0.2,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;194;-1085.156,4212.962;Inherit;False;Property;_color_uv;color_uv;14;0;Create;True;0;0;0;False;0;False;1,1,0,0;1,0.5,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;-731.7965,2506.84;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;195;-877.8904,4599.676;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WorldNormalVector;180;-428.4202,2706.435;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;196;-548.6094,4219.021;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;197;-565.6094,4437.021;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;179;-394.8352,2990.54;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;143;-2408.161,-394.4246;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector4Node;147;-2524.288,1220.283;Inherit;False;Property;_Dissolve_speed;Dissolve_speed;21;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0.2,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;97;-2078.88,-775.0789;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;146;-2512.562,987.8059;Inherit;False;Property;_Dissolve_uv;Dissolve_uv;20;0;Create;True;0;0;0;False;0;False;1,1,0,0;2,3,1.3,1.41;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;173;-552.6556,2426.919;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;168;-1297.209,2434.774;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;98;-2095.88,-557.0792;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;198;-368.6094,4288.021;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;200;-501.8523,4729.798;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;202;234.5859,4303.246;Inherit;False;126;Distort;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;199;-608.8523,4625.798;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;142;-2139.123,-368.3029;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;174;-124.1176,2445.986;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-1048.815,2090.931;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;219;263.5229,4590.47;Inherit;False;Property;_color_distort_power;color_distort_power;16;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;156;-2305.296,1374.519;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DotProductOpNode;181;-74.83521,2934.54;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;151;-1993.016,1211.864;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;95;-1898.88,-706.079;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;141;-2032.123,-264.3029;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;150;-1976.016,993.865;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;92;-2016.563,-52.4554;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;201;-38.85229,4422.798;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;663.522,4497.47;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;140;-1826.123,-420.3029;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;217;-1476.958,1515.244;Inherit;False;Property;_Dissolve_distort_power1;Dissolve_distort_power;22;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;152;-1796.016,1062.865;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;149;-1306.28,1343.75;Inherit;False;126;Distort;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;153;-2036.258,1400.641;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;90;-1588.025,-33.3887;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;182;242.7418,2914.396;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;154;-1929.258,1504.641;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;175;94.88242,2462.986;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;155;-1466.258,1197.641;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;203;209.5411,4078.957;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;190;546.4998,2909.097;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;411.5188,3061.536;Inherit;False;Property;_frensel;frensel;45;0;Create;True;0;0;0;False;0;False;-0.01;-0.01;-0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;218;-1076.959,1422.244;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-1910.386,504.4382;Inherit;False;Property;_Base_distort_power;Base_distort_power;3;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;375.6268,3200.543;Inherit;False;Property;_frensel_edge;frensel_edge;46;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-1432.025,-102.3887;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;176;291.9574,2391.892;Inherit;False;Property;_Custom2_zw_move_mask;Custom2_zw_move_mask;29;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;204;410.962,3879.163;Inherit;True;Property;_color_Tex;color_Tex;13;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;228;648.1094,2093.583;Inherit;False;Property;_mask_Constart;mask_Constart;30;0;Create;True;0;0;0;False;0;False;1;1;0.45;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;189;614.8088,2729.004;Inherit;False;Property;_frensel_flip;frensel_flip;44;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-615.7635,831.8236;Inherit;False;Property;_Dissolve;Dissolve;24;0;Create;True;0;0;0;False;0;False;1;0.609;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;113;-350.2611,1085.784;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-1510.387,411.4382;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;93;-1309.95,-328.4825;Inherit;False;Property;_Use_custom1_zw_move;Use_custom1_zw_move;4;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;703.0278,3155.35;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;171;294.2073,1764.307;Inherit;True;Property;_Base_mask_Tex;Base_mask_Tex;26;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;233;604.4957,4175.609;Inherit;False;Property;_Desaturate;Desaturate;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;145;-1217.865,853.7999;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;191;1159.786,2709.902;Inherit;False;Constant;_Float15;Float 15;44;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;232;952.7661,4051.901;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;186;835.2158,2979.1;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;966.699,2049.964;Inherit;False;Property;_mask_power;mask_power;31;0;Create;True;0;0;0;False;0;False;1;1;1;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;1276.637,4301.388;Inherit;False;Property;_color_Constart;color_Constart;18;0;Create;True;0;0;0;False;0;False;1;2;0.1;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;127;-1005.449,-92.44879;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;99;-1016.444,654.0061;Inherit;True;Property;_Dissolve_Tex;Dissolve_Tex;19;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;114;-64.20009,934.1133;Inherit;False;Property;_use_custom1_x_dissolve;use_custom1_x_dissolve;25;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-371.5546,595.4609;Inherit;False;Property;_Hardness;Hardness;23;0;Create;True;0;0;0;False;0;False;11;0;0;22;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;229;793.1226,1912.99;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;136;356.4235,735.0688;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-87.90869,475.0484;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;966.699,1848.964;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;187;1363.942,2718.494;Inherit;False;Property;_use_frensel;use_frensel;43;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-965.2712,-352.2458;Inherit;True;Property;_Base_Tex;Base_Tex;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;137;615.4236,909.0699;Inherit;False;Property;_Distance;Distance;10;0;Create;True;0;0;0;False;0;False;2.635294;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;107;-83.53351,722.2306;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;235;1274.65,4085.794;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-364.2225,-194.4455;Inherit;False;Property;_Constart;Constart;7;0;Create;True;0;0;0;False;0;False;1;2;0.45;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;206;1353.782,3894.582;Inherit;False;color_part;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;134;-392.2093,-373.0392;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;132;-772.1206,-835.1239;Inherit;False;Property;_R_light_color;R_light_color;5;1;[HDR];Create;True;0;0;0;False;0;False;1,0.92775,0.6462264,0;3.776172,3.364443,1.656528,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;106;154.4665,484.2305;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;159;169.176,55.80559;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;319.0657,74.29709;Inherit;False;Property;_Alpha_Constart;Alpha_Constart;11;0;Create;True;0;0;0;False;0;False;1;1;0.45;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;138;693.65,731.8209;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;131;-736.7756,-664.9689;Inherit;False;Property;_R_dark_clolor;R_dark_clolor;6;1;[HDR];Create;True;0;0;0;False;0;False;0.1509434,0.07064492,0.03203988,0;0.4716981,0.2384818,0.1535244,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;130;-276.4476,-517.0469;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;108;347.3645,456.7877;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;139;973.1312,732.7918;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;157;382.0789,-114.2966;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;178;1218.296,955.031;Inherit;False;177;Base_mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;1089.46,1123.417;Inherit;False;188;frensel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;129;250.8176,1082.969;Inherit;True;Property;_Mask;Mask;39;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;42;12.78459,-642.9928;Inherit;False;Property;_Color;Color;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0.735849,0.735849,0.735849,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;205;143.3939,-133.0517;Inherit;False;206;color_part;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-119.1539,-298.591;Inherit;False;Property;_Color_power;Color_power;9;0;Create;True;0;0;0;False;0;False;1;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-244.3707,181.4384;Inherit;False;Property;_Alpha;Alpha;12;0;Create;True;0;0;0;False;0;False;1;2;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;30;-426.2529,-39.49192;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;390.8635,-366.5229;Inherit;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;1314.55,591.4058;Inherit;False;8;8;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;77;1144.94,-818.4952;Inherit;False;345.9991;319.9341;Comment;3;75;76;135;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;1319.452,106.3459;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;76;1325.338,-613.9609;Inherit;False;Property;_Cull;Cull;41;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;1197.546,-583.0758;Inherit;False;Property;_Ztest;Ztest;42;2;[IntRange];[Enum];Create;True;0;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;1313.94,-737.4952;Inherit;False;Property;_Zwrite;Zwrite;40;1;[Enum];Create;True;0;2;off;0;on;1;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;33;732.2189,-365.3273;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;35;1469.567,593.4711;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;236;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;237;1847.432,113.7621;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;DC/Doublecats_RA_common_shader3;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;23;Surface;1;638980514451346106;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;True;True;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;238;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;239;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;240;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;241;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;242;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;243;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;244;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;245;1847.432,113.7621;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;177;1148.327,1757.741;Inherit;False;Base_mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;188;1609.616,2937.102;Inherit;False;frensel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;211;0;208;0
WireConnection;209;0;207;3
WireConnection;209;1;207;4
WireConnection;210;0;207;1
WireConnection;210;1;207;2
WireConnection;214;0;210;0
WireConnection;214;1;209;0
WireConnection;212;0;211;2
WireConnection;213;0;211;0
WireConnection;213;1;211;1
WireConnection;215;0;214;0
WireConnection;215;2;213;0
WireConnection;215;1;212;0
WireConnection;125;1;215;0
WireConnection;226;0;225;3
WireConnection;226;1;225;4
WireConnection;123;0;125;1
WireConnection;123;1;125;2
WireConnection;118;1;117;0
WireConnection;118;0;116;4
WireConnection;227;0;123;0
WireConnection;227;1;226;0
WireConnection;120;0;227;0
WireConnection;120;1;118;0
WireConnection;126;0;120;0
WireConnection;164;0;160;3
WireConnection;164;1;160;4
WireConnection;163;0;161;0
WireConnection;162;0;160;1
WireConnection;162;1;160;2
WireConnection;165;0;162;0
WireConnection;165;1;164;0
WireConnection;167;0;163;2
WireConnection;166;0;163;0
WireConnection;166;1;163;1
WireConnection;222;0;169;0
WireConnection;222;1;221;0
WireConnection;195;0;193;0
WireConnection;196;0;194;1
WireConnection;196;1;194;2
WireConnection;197;0;194;3
WireConnection;197;1;194;4
WireConnection;143;0;144;0
WireConnection;97;0;96;1
WireConnection;97;1;96;2
WireConnection;168;0;165;0
WireConnection;168;2;166;0
WireConnection;168;1;167;0
WireConnection;98;0;96;3
WireConnection;98;1;96;4
WireConnection;198;0;196;0
WireConnection;198;1;197;0
WireConnection;200;0;195;2
WireConnection;199;0;195;0
WireConnection;199;1;195;1
WireConnection;142;0;143;0
WireConnection;142;1;143;1
WireConnection;174;0;173;1
WireConnection;174;1;173;2
WireConnection;170;0;168;0
WireConnection;170;1;222;0
WireConnection;156;0;147;0
WireConnection;181;0;180;0
WireConnection;181;1;179;0
WireConnection;151;0;146;3
WireConnection;151;1;146;4
WireConnection;95;0;97;0
WireConnection;95;1;98;0
WireConnection;141;0;143;2
WireConnection;150;0;146;1
WireConnection;150;1;146;2
WireConnection;201;0;198;0
WireConnection;201;2;199;0
WireConnection;201;1;200;0
WireConnection;220;0;202;0
WireConnection;220;1;219;0
WireConnection;140;0;95;0
WireConnection;140;2;142;0
WireConnection;140;1;141;0
WireConnection;152;0;150;0
WireConnection;152;1;151;0
WireConnection;153;0;156;0
WireConnection;153;1;156;1
WireConnection;90;0;92;1
WireConnection;90;1;92;2
WireConnection;182;0;181;0
WireConnection;154;0;156;2
WireConnection;175;0;170;0
WireConnection;175;1;174;0
WireConnection;155;0;152;0
WireConnection;155;2;153;0
WireConnection;155;1;154;0
WireConnection;203;0;201;0
WireConnection;203;1;220;0
WireConnection;190;0;182;0
WireConnection;218;0;149;0
WireConnection;218;1;217;0
WireConnection;91;0;140;0
WireConnection;91;1;90;0
WireConnection;176;1;170;0
WireConnection;176;0;175;0
WireConnection;204;1;203;0
WireConnection;189;1;182;0
WireConnection;189;0;190;0
WireConnection;224;0;126;0
WireConnection;224;1;223;0
WireConnection;93;1;140;0
WireConnection;93;0;91;0
WireConnection;185;0;184;0
WireConnection;185;1;183;0
WireConnection;171;1;176;0
WireConnection;145;0;155;0
WireConnection;145;1;218;0
WireConnection;232;0;204;0
WireConnection;232;1;233;0
WireConnection;186;0;189;0
WireConnection;186;1;184;0
WireConnection;186;2;185;0
WireConnection;127;0;93;0
WireConnection;127;1;224;0
WireConnection;99;1;145;0
WireConnection;114;1;110;0
WireConnection;114;0;113;3
WireConnection;229;0;171;1
WireConnection;229;1;228;0
WireConnection;109;0;99;1
WireConnection;109;1;111;0
WireConnection;230;0;229;0
WireConnection;230;1;231;0
WireConnection;187;1;191;0
WireConnection;187;0;186;0
WireConnection;20;1;127;0
WireConnection;107;0;111;0
WireConnection;107;2;114;0
WireConnection;235;0;232;0
WireConnection;235;1;234;0
WireConnection;206;0;235;0
WireConnection;134;0;20;1
WireConnection;134;1;133;0
WireConnection;106;0;109;0
WireConnection;106;1;107;0
WireConnection;159;0;20;1
WireConnection;138;1;136;0
WireConnection;138;0;137;0
WireConnection;130;0;131;0
WireConnection;130;1;132;0
WireConnection;130;2;134;0
WireConnection;108;0;106;0
WireConnection;139;0;138;0
WireConnection;157;0;159;0
WireConnection;157;1;158;0
WireConnection;32;0;42;0
WireConnection;32;1;130;0
WireConnection;32;2;43;0
WireConnection;32;3;30;0
WireConnection;32;4;205;0
WireConnection;31;0;30;4
WireConnection;31;1;157;0
WireConnection;31;2;89;0
WireConnection;31;3;108;0
WireConnection;31;4;129;1
WireConnection;31;5;139;0
WireConnection;31;6;178;0
WireConnection;31;7;192;0
WireConnection;88;0;33;0
WireConnection;88;3;35;0
WireConnection;33;0;32;0
WireConnection;35;0;31;0
WireConnection;237;2;33;0
WireConnection;237;3;35;0
WireConnection;177;0;230;0
WireConnection;188;0;187;0
ASEEND*/
//CHKSM=3DEDE8FEB28001E3CF6A8C09AAAD5B759CB1D982