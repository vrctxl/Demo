Shader "AudioLink/Debug/AudioLinkMediaState"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MainTex("Normal", 2D) = "white" {}
	}
		SubShader
	{
		// shadow caster rendering pass, implemented manually
		// using macros from UnityCG.cginc
		Pass
		{
			Tags {"LightMode" = "ShadowCaster"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}



		Tags {"Queue" = "Transparent" "RenderType" = "Opaque" }
		AlphaToMask On

		LOD 200

		CGPROGRAM
				// Physically based Standard lighting model, and enable shadows on all light types
				#pragma surface surf Standard fullforwardshadows alpha

				#pragma target 5.0

				#include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"
				#include "Packages/com.llealloo.audiolink/Runtime/Shaders/SmoothPixelFont.cginc"

				sampler2D _MainTex;

				struct Input
				{
					float2 uv_MainTex;
				};

				half _Glossiness;
				half _Metallic;
				fixed4 _Color;

				// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
				// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
				// #pragma instancing_options assumeuniformscaling
				UNITY_INSTANCING_BUFFER_START(Props)
					// put more per-instance properties here
				UNITY_INSTANCING_BUFFER_END(Props)

				void surf(Input IN, inout SurfaceOutputStandard o)
				{
					fixed4 c = 0.;

						float value = 0;
						float4 mediaState = AudioLinkData(ALPASS_MEDIASTATE);

						float2 iuv = IN.uv_MainTex;
						iuv.y = 1. - iuv.y;
						const uint rows = 11;
						const uint cols = 21;
						const uint number_area_cols = 11;

						float2 pos = iuv * float2(cols,rows);
						uint2 dig = (uint2)(pos);

						// This line of code is tricky;  We determine how much we should soften the edge of the text
						// based on how quickly the text is moving across our field of view.  This gives us realy nice
						// anti-aliased edges.
						float2 softness = 2. / pow(length(float2(ddx(pos.x), ddy(pos.y))), 0.5);

						// Another option would be to set softness to 20 and YOLO it.

						float2 fmxy = float2(4, 6) - (glsl_mod(pos,1.)*float2(4.,6.));

						value = 0;
						int xoffset = 5;
						bool leadingzero = false;
						int points_after_decimal = 0;
						int max_decimals = 5;

						if (pos.y > 10.9)
						{
							// Prevent ugly under-chart next line.
							c = 0;
						}
						else if (dig.x < cols - number_area_cols && dig.y < 8)
						{
							uint sendchar = 0;
							const uint sendarr[80] = {
								'I', 'n', 's', 't', 'a', 'n', 'c', 'e', ' ', ' ',
								'V', 'o', 'l', 'u', 'm', 'e', ' ', ' ', ' ', ' ',
								'P', 'r', 'o', 'g', 'r', 'e', 's', 's', ' ', ' ',
								'S', 't', 'a', 't', 'e', ' ', ' ', ' ', ' ', ' ',
								'L', 'o', 'o', 'p', ' ', ' ', ' ', ' ', ' ', ' ',
								' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
								' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
								' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '
								};
							sendchar = sendarr[dig.x + dig.y * 10];
							c += PrintChar(sendchar, fmxy, softness, 0.0);
						}
						else
						{
							if (dig.y < 10)
								dig.x -= cols - number_area_cols;

							switch (dig.y)
							{
							case 0:
							case 1:
								value = mediaState.r;
								break;
							case 2:
								value = mediaState.g;
								break;
							case 3:
								value = mediaState.b;
								break;
							case 4:
								value = mediaState.a;
								break;
							case 5:
							case 6:
							case 7:
							case 8:
							case 9:
							case 10:
							default:
								c = 0;
								break;
							}
							float num = PrintNumberOnLine(value, fmxy, softness, dig.x - xoffset, points_after_decimal, max_decimals, leadingzero, 0);
							c.rgb = lerp(c.rgb, 1.0, num);
							c.a += num;
						}
					o.Emission = c.rgb;
					o.Albedo = c.rgb;

					// Metallic and smoothness come from slider variables
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;
				}
				ENDCG
	}
		FallBack "Diffuse"
}