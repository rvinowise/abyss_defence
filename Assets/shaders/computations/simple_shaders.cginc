struct appdata_t
{
    float4 vertex   : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct v2f
{
    float4 vertex   : SV_POSITION;
    float2 texcoord  : TEXCOORD0;
};


v2f simple_vert(appdata_t v)
{
    v2f OUT;
    float4 vPosition = UnityObjectToClipPos(v.vertex);
    OUT.vertex = vPosition;
    OUT.texcoord = v.texcoord;

    return OUT;
}