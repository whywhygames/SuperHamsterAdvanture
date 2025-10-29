using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.Feedbacks
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.MMTools")]
    [FeedbackPath("Audio/MMSoundManager Sound")]
    [FeedbackHelp("This feedback will let you play a sound via the MMSoundManager. You will need a game object in your scene with a MMSoundManager object on it for this to work.")]
    public class MMF_MMSoundManagerSound : MMF_Feedback
    {
        public static bool FeedbackTypeAuthorized = true;

#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        public override bool HasCustomInspectors => true;
        public override bool HasAutomaticShakerSetup => true;
        public override bool EvaluateRequiresSetup()
        {
            bool requiresSetup = false;
            if (Sfx == null)
            {
                requiresSetup = true;
            }
            if ((RandomSfx != null) && (RandomSfx.Length > 0))
            {
                requiresSetup = false;
                foreach (AudioClip clip in RandomSfx)
                {
                    if (clip == null)
                    {
                        requiresSetup = true;
                    }
                }
            }
            return requiresSetup;
        }
        public override string RequiredTargetText { get { return Sfx != null ? Sfx.name + " - ID:" + ID : ""; } }
        public override string RequiresSetupText { get { return "This feedback requires that you set an Audio clip in its Sfx slot below, or one or more clips in the Random Sfx array."; } }
#endif

        public override bool HasRandomness => true;

        [MMFInspectorGroup("Sound", true, 14, true)]
        [Tooltip("The sound clip to play")]
        public AudioClip Sfx;

        [MMFInspectorGroup("Random Sound", true, 39, true)]
        [Tooltip("An array to pick a random sfx from")]
        public AudioClip[] RandomSfx;
        [Tooltip("If this is true, random sfx audio clips will be played in sequential order instead of at random")]
        public bool SequentialOrder = false;
        [Tooltip("If we're in sequential order, determines whether or not to hold at the last index")]
        [MMFCondition("SequentialOrder", true)]
        public bool SequentialOrderHoldLast = false;
        [Tooltip("If in sequential order hold last mode, index will reset to 0 automatically after this duration")]
        [MMFCondition("SequentialOrderHoldLast", true)]
        public float SequentialOrderHoldCooldownDuration = 2f;
        [Tooltip("If true, sfx will be picked at random until all have been played.")]
        public bool RandomUnique = false;

        [MMFInspectorGroup("Sound Properties", true, 24)]
        [Header("Volume")]
        [Range(0f, 2f)]
        public float MinVolume = 1f;
        [Range(0f, 2f)]
        public float MaxVolume = 1f;
        [Header("Pitch")]
        [Range(-3f, 3f)]
        public float MinPitch = 1f;
        [Range(-3f, 3f)]
        public float MaxPitch = 1f;

        [MMFInspectorGroup("SoundManager Options", true, 28)]
        public MMSoundManager.MMSoundManagerTracks MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
        public int ID = 0;
        public AudioMixerGroup AudioGroup = null;
        public AudioSource RecycleAudioSource = null;
        public bool Loop = false;
        public bool Persistent = false;
        public bool DoNotPlayIfClipAlreadyPlaying = false;
        public bool StopSoundOnFeedbackStop = false;

        protected AudioClip _randomClip;
        protected AudioSource _editorAudioSource;
        protected MMSoundManagerPlayOptions _options;
        protected AudioSource _playedAudioSource;
        protected float _randomPlaybackTime;
        protected float _randomPlaybackDuration;
        protected int _currentIndex = 0;
        protected AudioClip _lastPlayedClip;
        protected MMShufflebag<int> _randomUniqueShuffleBag;

        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);

            _lastPlayedClip = null;

            if (RandomSfx == null)
            {
                RandomSfx = Array.Empty<AudioClip>();
            }

            if (RandomUnique)
            {
                _randomUniqueShuffleBag = new MMShufflebag<int>(RandomSfx.Length);
                for (int i = 0; i < RandomSfx.Length; i++)
                {
                    _randomUniqueShuffleBag.Add(i, 1);
                }
            }
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            float intensityMultiplier = ComputeIntensity(feedbacksIntensity, position);

            if (RandomSfx.Length > 0)
            {
                _randomClip = PickRandomClip();
                if (_randomClip != null)
                {
                    PlaySound(_randomClip, position, intensityMultiplier);
                    return;
                }
            }

            if (Sfx != null)
            {
                PlaySound(Sfx, position, intensityMultiplier);
                return;
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            if (StopSoundOnFeedbackStop && (_playedAudioSource != null))
            {
                _playedAudioSource.Stop();
                if (RecycleAudioSource == null)
                {
                    MMSoundManager.Instance.FreeSound(_playedAudioSource);
                }
            }
        }

        protected virtual void RandomizeTimes()
        {
            _randomPlaybackTime = Random.Range(0f, 0f);
            _randomPlaybackDuration = Random.Range(0f, 0f);
            Owner.ComputeCachedTotalDuration();
        }

        protected virtual void PlaySound(AudioClip sfx, Vector3 position, float intensity)
        {
            if (DoNotPlayIfClipAlreadyPlaying)
            {
                if ((MMSoundManager.Instance.FindByClip(sfx) != null) && (MMSoundManager.Instance.FindByClip(sfx).isPlaying))
                {
                    return;
                }
            }

            float volume = Random.Range(MinVolume, MaxVolume);
            if (!Timing.ConstantIntensity)
            {
                volume *= intensity;
            }

            float pitch = Random.Range(MinPitch, MaxPitch);
            RandomizeTimes();

            _options.Location = position;
            _options.Loop = Loop;
            _options.Volume = volume;
            _options.ID = ID;
            _options.Persistent = Persistent;
            _options.RecycleAudioSource = RecycleAudioSource;
            _options.AudioGroup = AudioGroup;
            _options.Pitch = pitch;
            _options.PlaybackTime = _randomPlaybackTime;
            _options.PlaybackDuration = _randomPlaybackDuration;
            _options.DoNotAutoRecycleIfNotDonePlaying = true;

            _playedAudioSource = MMSoundManagerSoundPlayEvent.Trigger(sfx, _options);
            Owner.StartCoroutine(IsPlayingCoroutine());
            _lastPlayedClip = sfx;
        }

        protected virtual IEnumerator IsPlayingCoroutine()
        {
            if (_playedAudioSource != null)
            {
                while (_playedAudioSource.isPlaying)
                {
                    IsPlaying = true;
                    yield return null;
                }
                IsPlaying = false;
            }
        }

        protected virtual AudioClip PickRandomClip()
        {
            int newIndex = 0;

            if (!SequentialOrder)
            {
                if (RandomUnique)
                {
                    newIndex = _randomUniqueShuffleBag.Pick();
                }
                else
                {
                    newIndex = Random.Range(0, RandomSfx.Length);
                }
            }
            else
            {
                newIndex = _currentIndex;

                if (newIndex >= RandomSfx.Length)
                {
                    if (SequentialOrderHoldLast)
                    {
                        newIndex--;
                        if ((SequentialOrderHoldCooldownDuration > 0))
                        {
                            newIndex = 0;
                        }
                    }
                    else
                    {
                        newIndex = 0;
                    }
                }
                _currentIndex = newIndex + 1;
            }
            return RandomSfx[newIndex];
        }

        public virtual void ResetSequentialIndex()
        {
            _currentIndex = 0;
        }
    }
}
