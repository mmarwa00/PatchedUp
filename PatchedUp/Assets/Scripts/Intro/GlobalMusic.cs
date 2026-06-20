using UnityEngine;
using System.Collections;

// Put this script on an empty GameObject in your INTRO scene only.
// It will auto-survive into the next scene by itself (DontDestroyOnLoad).
// Do NOT also put a copy of this in zone1_greyboxing - it would create a duplicate.
public class GlobalMusic : MonoBehaviour
{
    public static GlobalMusic Instance;

    [Header("Audio Sources - drag two EMPTY AudioSource components here")]
    public AudioSource sourceA;
    public AudioSource sourceB;

    [Header("Clips")]
    public AudioClip cuteMusic;
    public AudioClip eerieMusic;

    private AudioSource _activeSource;
    private AudioSource _inactiveSource;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _activeSource = sourceA;
        _inactiveSource = sourceB;
    }

    // Call this once at the very start of the intro (cute music, no fade needed, just play).
    public void PlayCute(float startTime = 0f)
    {
        _activeSource.clip = cuteMusic;
        _activeSource.loop = true;
        _activeSource.volume = 1f;
        _activeSource.Play();
        if (startTime > 0f) _activeSource.time = startTime;

        _inactiveSource.volume = 0f;
    }

    // Call this when the teddy swap / horror moment happens.
    // Crossfades smoothly from whatever is currently playing into eerieMusic.
    public void CrossfadeToEerie(float duration)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(Crossfade(eerieMusic, duration));
    }

    // Call this right before loading the next scene, to fade music down to silence.
    public void FadeOutCurrent(float duration)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeOut(duration));
    }

    private IEnumerator Crossfade(AudioClip nextClip, float duration)
    {
        _inactiveSource.clip = nextClip;
        _inactiveSource.loop = true;
        _inactiveSource.volume = 0f;
        _inactiveSource.Play();

        float startVolActive = _activeSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _activeSource.volume = Mathf.Lerp(startVolActive, 0f, t);
            _inactiveSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        _activeSource.volume = 0f;
        _activeSource.Stop();
        _inactiveSource.volume = 1f;

        // Swap which source is "active" now
        var temp = _activeSource;
        _activeSource = _inactiveSource;
        _inactiveSource = temp;
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVol = _activeSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _activeSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        _activeSource.volume = 0f;
        _activeSource.Stop();
        _activeSource.clip = null;
    }
}