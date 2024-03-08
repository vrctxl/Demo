
using Texel;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SearchUrl : UdonSharpBehaviour
{
    [SerializeField]
    public TXLVideoPlayer videoPlayer;
    public VRCUrl baseUrl;

    [Header("UI")]
    public VRCUrlInputField urlInputField;

    private void Start()
    {
        urlInputField.SetUrl(baseUrl);
    }

    public void _HandleClick()
    {
        urlInputField.SetUrl(baseUrl);
    }

    public void _HandleSubmit()
    {
        VRCUrl url = urlInputField.GetUrl();
        if (url == null || url.Get() == "")
            return;

        videoPlayer._ChangeUrl(url);
    }
}
