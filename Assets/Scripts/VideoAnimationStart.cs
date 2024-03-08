
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using Texel;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class VideoAnimationStart : UdonSharpBehaviour
{
    public TXLVideoPlayer videoPlayer;
    public VRCUrl videoUrl;
    public GameObject animationGroup;

    void Start()
    {
        videoPlayer._Register(TXLVideoPlayer.EVENT_VIDEO_STATE_UPDATE, this, "_OnVideoStateUpdate");
    }

    public override void Interact()
    {
        videoPlayer._ChangeUrl(videoUrl);

        LocalPlayer local = (LocalPlayer)videoPlayer;
        if (local)
            local._TriggerPlay();
    }

    public void _OnVideoStateUpdate()
    {
        if (videoPlayer.playerState == TXLVideoPlayer.VIDEO_STATE_PLAYING)
            animationGroup.SetActive(true);
    }
}
