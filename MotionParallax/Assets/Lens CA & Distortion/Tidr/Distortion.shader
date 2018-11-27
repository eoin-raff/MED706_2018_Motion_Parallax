Shader "Custom/Distortion" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BarrelPower("Barrel Power", Float) = 0
			strength("Strength", FLoat) = 0.5
			cylindricalRatio("Cylindrical Ratio", FLoat) = 0.5
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


	float3 gridColor = float3(0.0,0.0,0.0);
	float3 color = float3(0.4,0.4,0.4);
	float _BarrelPower = 1.0;
	float gradientValue;
	float r,g,b;


	uniform float strength;           // s: 0 = perspective, 1 = stereographic
	uniform float height;             // h: tan(verticalFOVInRadians / 2)
	uniform float aspectRatio;        // a: screenWidth / screenHeight
	uniform float cylindricalRatio;   // c: cylindrical distortion ratio. 1 = spherical
	float _ZoomScale;
	float _ZoomOffset;

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	float2 Distort(float2 p) {
		float theta = atan2(p.y, p.x);
		float radius = length(p);
		radius = pow(radius, _BarrelPower);
		p.x = radius * cos(theta);
		p.y = radius * sin(theta);
		return 0.5 * (p + 1.0);
	}

	sampler2D _MainTex;
	float4 _MainTex_ST;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 Distortion(float2 uv) {
		float PI = 3.14159265;

		float angle;
		float azim;
		float dist;

		float get_s, get_t, pos_s;
		//uv.x -= 0.5;
		//angle = uv.x * 360 - 180.0;
		angle = uv.x * 360 - 180.0; // Range is -180 to 180
		uv.y *= _BarrelPower;
		uv.y -= _ZoomOffset;
		if (angle >= -180.0 && angle < 0) { // I need to change range to -45 to 45
			float n = (angle/2) + 45;
			azim = n;//angle + 90.0 + 45;
			pos_s = 0.0;
		}
		if (angle >= 0 && angle < 180) {
			float n = (angle / 2) - 45;
			azim = n;//angle + 90.0 + 45;
			//azim = angle - 90.0 + 45;
			pos_s = 0.5;
		}
		//if (angle >= -180.0 && angle < -90.0) { //This part scales the value between -45 to 45
		//	azim = angle + 180.0 - 45.0;
		//	pos_s = 0.0;
		//}
		//
		//if (angle >= -90.0 && angle < 0.0) {
		//	azim = angle + 45.0;
		//	pos_s = 0.25;
		//}
		//
		//if (angle >= 0.0 && angle < 90.0) {
		//	azim = angle - 45.0;
		//	pos_s = 0.5;
		//}
		//
		//if (angle >= 90.0 && angle < 180.0) {
		//	azim = angle - 180.0 + 45.0;
		//	pos_s = 0.75;
		//}

		float a = (PI*azim) / (180 );
		float b = pow(tan(a), 2.0) ;
		float c = (b + 1.0) ;
		dist = sqrt(c);

		float geta = PI*azim / 180;
		float getb = (tan(geta) + 1.0);
		float getc = getb / 4.0;

		get_s = getc + pos_s;

		get_t = ((uv.y - 0.5)*dist + 0.5);

		fixed4 col;

		if (uv.y  < 0.5 - 0.5 / sqrt(2.0) || uv.y > 0.5 + 0.5 / sqrt(2.0)) {
			col = fixed4(0.0, 0.0, 0.0, 0.0);
		}
		else {
			col = tex2D(_MainTex, float2(get_s, get_t));
		}

		return col;
	}

	fixed4 frag(v2f i) : SV_Target
	{
	
	float2 normalizedUV = (i.uv * 2) - 1;//(i.vertex.xy / (_ScreenParams.xy / 2.0)) - 1.0;
	float2 distortedUV = Distort(normalizedUV);//*_ScreenParams.xy;
	color = tex2D(_MainTex, distortedUV);
	//col = color;
	//color = Distortion(i.uv);
	return  Distortion(i.uv);
	//return float4(color,1);
	}
		ENDCG
	}
	}
}
