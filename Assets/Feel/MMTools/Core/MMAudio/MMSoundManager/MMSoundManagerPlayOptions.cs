using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Options for playing a sound with MMSoundManager.
    /// </summary>
    [Serializable]
    public struct MMSoundManagerPlayOptions
    {
        [Header("Track")]
        public MMSoundManager.MMSoundManagerTracks Track;
        public AudioMixerGroup AudioGroup;

        [Header("Sound Settings")]
        public bool Loop;
        [Range(0f, 2f)] public float Volume;
        [Range(-3f, 3f)] public float Pitch;
        public int ID;

        [Header("Fade")]
        public bool Fade;
        [MMCondition("Fade", true)] public float FadeInitialVolume;
        [MMCondition("Fade", true)] public float FadeDuration;
        [MMCondition("Fade", true)] public MMTweenType FadeTween;

        [Header("Persistence & Source")]
        public bool Persistent;
        public AudioSource RecycleAudioSource;

        [Header("Timing")]
        public float PlaybackTime;
        public float PlaybackDuration;

        [Header("Spatial Settings")]
        [Range(-1f, 1f)] public float PanStereo;
        [Range(0f, 1f)] public float SpatialBlend;
        public Transform AttachToTransform;

        [Header("Solo & Bypass")]
        public bool SoloSingleTrack;
        public bool SoloAllTracks;
        public bool AutoUnSoloOnEnd;
        public bool BypassEffects;
        public bool BypassListenerEffects;
        public bool BypassReverbZones;

        [Header("Advanced Audio")]
        [Range(0, 256)] public int Priority;
        [Range(0f, 1.1f)] public float ReverbZoneMix;
        [Range(0f, 5f)] public float DopplerLevel;
        public Vector3 Location;
        [Range(0, 360)] public int Spread;
        public AudioRolloffMode RolloffMode;
        public float MinDistance;
        public float MaxDistance;
        public bool DoNotAutoRecycleIfNotDonePlaying;

        [Header("Custom Curves")]
        public bool UseCustomRolloffCurve;
        [MMCondition("UseCustomRolloffCurve", true)] public AnimationCurve CustomRolloffCurve;
        public bool UseSpatialBlendCurve;
        [MMCondition("UseSpatialBlendCurve", true)] public AnimationCurve SpatialBlendCurve;
        public bool UseReverbZoneMixCurve;
        [MMCondition("UseReverbZoneMixCurve", true)] public AnimationCurve ReverbZoneMixCurve;
        public bool UseSpreadCurve;
        [MMCondition("UseSpreadCurve", true)] public AnimationCurve SpreadCurve;

        /// <summary>
        /// Default options for common cases.
        /// </summary>
        public static MMSoundManagerPlayOptions Default => new MMSoundManagerPlayOptions
        {
            Track = MMSoundManager.MMSoundManagerTracks.Sfx,
            Location = Vector3.zero,
            Loop = false,
            Volume = 1f,
            Pitch = 1f,
            ID = 0,
            Fade = false,
            FadeInitialVolume = 0f,
            FadeDuration = 1f,
            FadeTween = MMTweenType.DefaultEaseInCubic,
            Persistent = false,
            RecycleAudioSource = null,
            AudioGroup = null,
            PanStereo = 0f,
            SpatialBlend = 0f,
            SoloSingleTrack = false,
            SoloAllTracks = false,
            AutoUnSoloOnEnd = false,
            BypassEffects = false,
            BypassListenerEffects = false,
            BypassReverbZones = false,
            Priority = 128,
            ReverbZoneMix = 1f,
            DopplerLevel = 1f,
            Spread = 0,
            RolloffMode = AudioRolloffMode.Logarithmic,
            MinDistance = 1f,
            MaxDistance = 500f,
            DoNotAutoRecycleIfNotDonePlaying = true
        };
    }
}
