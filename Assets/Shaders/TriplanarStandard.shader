Shader "Triplanar"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Albedo Top", 2D) = "white" {}
		_MainTex1("Albedo Front", 2D) = "white" {}
		_MainTex2("Albedo Side", 2D) = "white" {}
		_SizeX("SizeX", Float) = 20
		_SizeY("SizeY", Float) = 20
		_NX("NX", Range(0,1)) = 1
		_NY("NY", Range(0,1)) = 1
		_NZ("NZ", Range(0,1)) = 1
		_PreventRepeatMult("Prevent Repeat Mult", Float) = 0
		[MaterialToggle] _FlipTop("Flip Top", Float) = 0

		//_SplatMap("Splat", 2D) = "white" {}

		_OcclusionTex("Occlusion Map", 2D) = "white" {}

		//_BumpMap0("Normal", 2D) = "white" {}
		//_BumpMap1("Normal", 2D) = "white" {}
		//_BumpMap2("Normal", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MainTex1;
		sampler2D _MainTex2;
		fixed4 _Color;
		fixed _NX;
		fixed _NY;
		fixed _NZ;
		half _SizeX;
		half _SizeY;
		half _PreventRepeatMult;

		//sampler2D _BumpMap0;
		//sampler2D _BumpMap1;
		//sampler2D _BumpMap2;

		//sampler2D _SplatMap;

		sampler2D _OcclusionTex;
		uniform bool _FlipTop;

		struct Input
		{
			half3 worldPos;
			fixed3 worldNormal;
			fixed2 uv_OcclusionTex;
			INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half2 scale = half2(_SizeX, _SizeY);

			float3 wp = IN.worldPos;
			//float3 n = IN.worldNormal;

			// top down
			half2 uvy = (_FlipTop ? wp.xz : wp.zx) + wp.y * _PreventRepeatMult;

			// sideways
			half2 uvx = wp.zy + wp.x * _PreventRepeatMult;
			half2 uvz = wp.xy + wp.z * _PreventRepeatMult;
			//n.x * wp.yz + n.y * wp.xz + n.z * wp.xy;

			fixed4 cy = tex2D(_MainTex, uvy / scale); // top down
			fixed4 cx = tex2D(_MainTex2, uvx / scale); // left right
			fixed4 cz = tex2D(_MainTex1, uvz / scale); // back forth

			//fixed3 n0 = UnpackNormal(tex2D(_BumpMap0, IN.worldPos.xz / scale));
			//fixed3 n1 = UnpackNormal(tex2D(_BumpMap1, IN.worldPos.xy / scale));
			//fixed3 n2 = UnpackNormal(tex2D(_BumpMap2, IN.worldPos.zy / scale));

			fixed occlusion = tex2D(_OcclusionTex, IN.uv_OcclusionTex.xy);

			fixed3 nWNormal = Unity_SafeNormalize(IN.worldNormal*fixed3(_NX, _NY, _NZ));
			//fixed3 nWNormal = IN.worldNormal;
			fixed3 projnormal = saturate(pow(nWNormal*1.5, 4));

			//fixed4 splatMap = tex2D(_SplatMap, IN.worldPos.xz / scale);
			//fixed splat = splatMap.r;

			// Sides
			half4 result = lerp(cz, cx, projnormal.x);
			// Top
			result = lerp(result, cy, round(projnormal.y * (0.5+ result.r)));

			//half3 resultBump = lerp(n0, n1, projnormal.z);
			//resultBump = lerp(resultBump, n2, projnormal.x);
			//normal = lerp(normal, half3(0, 1, 0), projnormal.z);
			//half3 normal = resultBump;

			//o.Normal = normal;

			o.Albedo = result.rgb * _Color.rgb * occlusion;
			o.Alpha = result.a * _Color.a;

			o.Occlusion = occlusion;

			//o.Normal = normal;
		}
	ENDCG
	}
		Fallback "VertexLit"
}