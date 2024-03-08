
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Texel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SyncExtraOptionsUI : ControlBase
    {
        public SyncPlayer syncPlayer;

        [Header("UI")]
        public GameObject holdActiveButton;
        public GameObject holdReleaseButton;
        public GameObject retryErrorButton;
        public GameObject avproFallbackButton;
        public GameObject internalSyncButton;

        const int UI_BUTTON_HOLD_ACTIVE = 0;
        const int UI_BUTTON_HOLD_RELEASE = 1;
        const int UI_BUTTON_RETRY_ERROR = 2;
        const int UI_BUTTON_AVPRO_FALLBACK = 3;
        const int UI_BUTTON_INTERNAL_SYNC = 4;
        const int UI_BUTTON_COUNT = 5;

        void Start()
        {
            _EnsureInit();
        }

        protected override int ButtonCount => UI_BUTTON_COUNT;

        protected override void _Init()
        {
            _DiscoverButton(UI_BUTTON_HOLD_ACTIVE, holdActiveButton, COLOR_YELLOW);
            _DiscoverButton(UI_BUTTON_HOLD_RELEASE, holdReleaseButton, COLOR_YELLOW);
            _DiscoverButton(UI_BUTTON_RETRY_ERROR, retryErrorButton, COLOR_YELLOW);
            _DiscoverButton(UI_BUTTON_AVPRO_FALLBACK, avproFallbackButton, COLOR_YELLOW);
            _DiscoverButton(UI_BUTTON_INTERNAL_SYNC, internalSyncButton, COLOR_YELLOW);

            syncPlayer._Register(TXLVideoPlayer.EVENT_VIDEO_STATE_UPDATE, this, nameof(_OnStateUpdate));
            syncPlayer._Register(TXLVideoPlayer.EVENT_VIDEO_READY, this, nameof(_OnVideoReady));

            _UpdateButtonState();
        }

        public void _OnStateUpdate()
        {
            _UpdateButtonState();
        }

        public void _OnVideoReady()
        {
            _UpdateButtonState();
        }

        public void _HoldActiveButton()
        {
            syncPlayer._SetHoldMode(!syncPlayer.HoldVideos);
        }

        public void _HoldReleaseButton()
        {
            syncPlayer._ReleaseHold();
        }

        void _UpdateButtonState()
        {
            _SetButton(UI_BUTTON_HOLD_ACTIVE, syncPlayer.HoldVideos);
            _SetButton(UI_BUTTON_HOLD_RELEASE, syncPlayer.HoldVideos && syncPlayer._videoReady);
            _SetButton(UI_BUTTON_RETRY_ERROR, syncPlayer.retryOnError);
            _SetButton(UI_BUTTON_AVPRO_FALLBACK, syncPlayer.streamFallback);
            _SetButton(UI_BUTTON_INTERNAL_SYNC, syncPlayer.autoInternalAVSync);
        }
    }
}
