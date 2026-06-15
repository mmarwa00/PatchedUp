using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [Header("Characters")]
    public GameObject teddyBrown;
    public GameObject teddyPatched;
    public GameObject child;

    [Header("Movement")]
    public Transform walkTarget;
    public float walkSpeed = 1.5f;

    [Header("Fade")]
    public CanvasGroup blackPanel;
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 1.5f;
    public float distanceToStartFade = 1.5f;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip cuteMusic;
    public AudioClip eerieMusic;
    public AudioSource footstepSource; // Attach to Child for walking sounds

    [Header("Lighting Controls")]
    public Light directionalLight;
    public float lightTransitionDuration = 1.5f;

    [Space(10)]
    public Color warmColor = new Color(1f, 0.95f, 0.8f);
    public float warmIntensity = 1f;
    public Color warmAmbient = new Color(0.5f, 0.5f, 0.5f);

    [Space(10)]
    public Color horrorColor = new Color(0.2f, 0.3f, 0.4f);
    public float horrorIntensity = 0.3f;
    public Color horrorAmbient = new Color(0.01f, 0.01f, 0.01f);

    [Header("UI Controls")]
    public GameObject startSignUI;

    private bool _fadingOut = false;

    private void Start()
    {
        _fadingOut = false;
        teddyBrown.SetActive(true);
        teddyPatched.SetActive(false);
        blackPanel.alpha = 0f;

        if (directionalLight != null) { directionalLight.color = warmColor; directionalLight.intensity = warmIntensity; }
        RenderSettings.ambientLight = warmAmbient;

        // Play Intro Music
        if (musicSource != null && cuteMusic != null) { musicSource.clip = cuteMusic; musicSource.loop = true; musicSource.Play(); }
        musicSource.time = 10.0f; // This jumps 10 seconds into the song instantly
        var ai = child.GetComponent("ChildAI") as MonoBehaviour;
        if (ai != null) ai.enabled = false;
        var nav = child.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null) nav.enabled = false;
    }

    private void Update()
    {
        if (_fadingOut) return;

        Vector3 targetPos = new Vector3(walkTarget.position.x, child.transform.position.y, walkTarget.position.z);
        Vector3 oldPos = child.transform.position;

        child.transform.position = Vector3.MoveTowards(child.transform.position, targetPos, walkSpeed * Time.deltaTime);

        // Footstep logic while moving
        if (Vector3.Distance(oldPos, child.transform.position) > 0.001f && footstepSource != null && !footstepSource.isPlaying)
            footstepSource.Play();

        Vector3 direction = targetPos - child.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f) child.transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(child.transform.position, targetPos) <= distanceToStartFade)
        {
            _fadingOut = true;
            StartCoroutine(DoIntroSequence());
        }
    }

    private IEnumerator DoIntroSequence()
    {
        // 1. Blackout and Music Switch
        yield return StartCoroutine(FadeAndSwitchMusic(eerieMusic, fadeOutDuration));

        // 2. Teddy Swap
        teddyBrown.SetActive(false);
        child.SetActive(false);
        teddyPatched.SetActive(true);

        Animator anim = teddyPatched.GetComponent<Animator>();
        if (anim != null) anim.speed = 0f;

        yield return new WaitForSeconds(0.5f);

        if (directionalLight != null) StartCoroutine(TransitionLight());

        // 3. Fade back in
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        if (anim != null) anim.speed = 1f;

        // 4. Extend the "Horror" phase duration if you want
        // Change the number below to make it longer or shorter before loading MainMenu
        yield return new WaitForSeconds(12.0f);

        // 5. Load the MainMenu scene
        if (musicSource != null) {
            musicSource.Stop();
            musicSource.clip = null;
            Debug.Log("[IntroController] Intro-Musik erfolgreich vernichtet. Bereit für Zone 1!");
        }

        SceneManager.LoadScene("zone1_greyboxing");
    }

    private IEnumerator FadeAndSwitchMusic(AudioClip nextClip, float duration)
    {
        float elapsed = 0f;
        float startVol = 1.0f; // Force start volume to 1.0

        // 1. Fade Out
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            musicSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        // 2. Prepare the Swap
        musicSource.Stop();
        musicSource.clip = nextClip;
        musicSource.volume = startVol;
        musicSource.pitch = 1.0f;   
        musicSource.mute = false;    

        // 3. Force Play
        musicSource.Play();

        // 4. Verification Check
        if (!musicSource.isPlaying)
        {
            Debug.LogWarning("AudioSource failed to start the new clip! Attempting manual override...");
            musicSource.PlayOneShot(nextClip);
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        blackPanel.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackPanel.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        blackPanel.alpha = to;
    }

    private IEnumerator TransitionLight()
    {
        float elapsed = 0f;
        while (elapsed < lightTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lightTransitionDuration;
            directionalLight.color = Color.Lerp(warmColor, horrorColor, t);
            directionalLight.intensity = Mathf.Lerp(warmIntensity, horrorIntensity, t);
            RenderSettings.ambientLight = Color.Lerp(warmAmbient, horrorAmbient, t);
            yield return null;
        }
    }
}