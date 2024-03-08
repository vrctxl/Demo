
using Texel;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AudioProfileUpdate : UdonSharpBehaviour
{
    public AudioManager audioManager;

    public GroupToggle profile51;
    public GroupToggle profile71;

    void Start()
    {
        audioManager._Register(AudioManager.EVENT_CHANNEL_GROUP_CHANGED, this, "_AudioChanged");
    }

    public void _AudioChanged()
    {
        profile51._ToggleOff();
        profile71._ToggleOff();

        if (!audioManager.SelectedChannelGroup)
            return;

        switch (audioManager.SelectedChannelGroup.groupName)
        {
            case "Surround 5.1":
                profile51._ToggleOn();
                break;
            case "Surround 7.1":
                profile71._ToggleOn();
                break;
        }
    }
}
