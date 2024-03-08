
using Texel;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TextureRefreshManager : UdonSharpBehaviour
{
    public VideoManager mux;
    public TextureRefresh unityTarget;
    public TextureRefresh avproTarget;

    void Start()
    {
        if (mux)
            mux._Register(VideoManager.SOURCE_CHANGE_EVENT, this, "_OnSourceChange");
    }

    public void _OnSourceChange()
    {
        if (unityTarget && mux.ActiveSourceType == VideoSource.VIDEO_SOURCE_UNITY)
            unityTarget.source = mux.CaptureRenderer;
        else if (avproTarget && mux.ActiveSourceType == VideoSource.VIDEO_SOURCE_AVPRO)
            avproTarget.source = mux.CaptureRenderer;
    }
}
