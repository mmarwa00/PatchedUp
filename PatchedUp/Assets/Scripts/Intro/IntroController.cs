using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [Header("Characters")]
    public GameObject teddyBrown;
    public GameObject teddyPatched;
    public GameObject child;

    [Header("Table Objects - Act 2 swap")]
    public GameObject towels;
    public GameObject towels1;
    public GameObject singleTed;

    [Header("Movement")]
    public Transform walkTarget;
    public float walkSpeed = 1.5f;

    [Header("Fade")]
    public CanvasGroup blackPanel;
    public float fadeOutDuration = 3.0f;
    public float fadeInDuration = 3.0f;
    public float distanceToStartFade = 1.5f;
    public float finalFadeOutDuration = 4.0f;

    [Header("Music timing (actual playback is handled by GlobalMusic)")]
    public float musicCrossfadeDuration = 4.0f;
    public float musicFinalFadeOutDuration = 4.0f;
    public float cuteMusicStartTime = 10.0f;

    [Header("Footsteps")]
    public AudioSource footstepSource;

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

        towels.SetActive(true);
        towels1.SetActive(true);
        singleTed.SetActive(false);

        if (directionalLight != null) { directionalLight.color = warmColor; directionalLight.intensity = warmIntensity; }
        RenderSettings.ambientLight = warmAmbient;

        if (GlobalMusic.Instance != null)
            GlobalMusic.Instance.PlayCute(cuteMusicStartTime);

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
        // 1. Screen fades to black AND music crossfades to eerie, at the same time
        if (GlobalMusic.Instance != null)
            GlobalMusic.Instance.CrossfadeToEerie(musicCrossfadeDuration);

        yield return StartCoroutine(FadeScreen(0f, 1f, fadeOutDuration));

        // 2. Teddy Swap + Table Swap (screen is fully black now, all happens hidden)
        teddyBrown.SetActive(false);
        child.SetActive(false);
        teddyPatched.SetActive(true);

        towels.SetActive(false);
        towels1.SetActive(false);
        singleTed.SetActive(true);

        Animator anim = teddyPatched.GetComponent<Animator>();
        if (anim != null) anim.speed = 0f;

        yield return new WaitForSeconds(0.5f);

        if (directionalLight != null) StartCoroutine(TransitionLight());

        // 3. Fade screen back in - everything above is already swapped, so it's all visible as soon as light returns
        yield return StartCoroutine(FadeScreen(1f, 0f, fadeInDuration));

        if (anim != null) anim.speed = 1f;

        // 4. Hold on the horror phase
        yield return new WaitForSeconds(12.0f);

        // 5. Fade screen to black AND fade music out, together
        if (GlobalMusic.Instance != null)
            GlobalMusic.Instance.FadeOutCurrent(musicFinalFadeOutDuration);

        yield return StartCoroutine(FadeScreen(0f, 1f, finalFadeOutDuration));

        // 6. Load next scene - screen is black and music is silent, so the cut is invisible
        SceneManager.LoadScene("StoryScene");
    }

    private IEnumerator FadeScreen(float from, float to, float duration)
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