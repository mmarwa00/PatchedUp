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

    [Header("Lighting Controls")]
    public Light directionalLight;
    public float lightTransitionDuration = 1.5f;

    [Space(10)]
    public Color warmColor = new Color(1f, 0.95f, 0.8f);
    public float warmIntensity = 1f;
    public Color warmAmbient = new Color(0.5f, 0.5f, 0.5f); // Normal room brightness

    [Space(10)]
    public Color horrorColor = new Color(0.2f, 0.3f, 0.4f); // A creepy cold color, NOT black
    public float horrorIntensity = 0.3f; // Low intensity, but enough to cast shadows!
    public Color horrorAmbient = new Color(0.01f, 0.01f, 0.01f); // Kills the hidden room light

    [Header("Scene")]
    public string nextSceneName = "GameScene";

    private bool _fadingOut = false;

    private void Start()
    {
        _fadingOut = false;

        teddyBrown.SetActive(true);
        teddyPatched.SetActive(false);

        blackPanel.alpha = 0f;

        if (directionalLight != null)
        {
            directionalLight.color = warmColor;
            directionalLight.intensity = warmIntensity;
        }

        // Set starting room brightness
        RenderSettings.ambientLight = warmAmbient;

        var ai = child.GetComponent("ChildAI") as MonoBehaviour;
        if (ai != null) ai.enabled = false;

        var nav = child.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null) nav.enabled = false;
    }

    private void Update()
    {
        if (_fadingOut) return;

        Vector3 targetPos = new Vector3(
            walkTarget.position.x,
            child.transform.position.y,
            walkTarget.position.z
        );

        child.transform.position = Vector3.MoveTowards(
            child.transform.position,
            targetPos,
            walkSpeed * Time.deltaTime
        );

        Vector3 direction = targetPos - child.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
            child.transform.rotation = Quaternion.LookRotation(direction);

        float distance = Vector3.Distance(child.transform.position, targetPos);
        if (distance <= distanceToStartFade)
        {
            _fadingOut = true;
            StartCoroutine(DoIntroSequence());
        }
    }

    private IEnumerator DoIntroSequence()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        teddyBrown.SetActive(false);
        child.SetActive(false);
        teddyPatched.SetActive(true);

        Animator anim = teddyPatched.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = 0f;
        }

        yield return new WaitForSeconds(0.5f);

        if (directionalLight != null)
            StartCoroutine(TransitionLight());

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        if (anim != null)
        {
            anim.speed = 1f;
        }

        yield return new WaitForSeconds(5.5f);

        SceneManager.LoadScene(nextSceneName);
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

    // Now transitions Color, Intensity, AND the hidden Ambient Light
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
        directionalLight.color = horrorColor;
        directionalLight.intensity = horrorIntensity;
        RenderSettings.ambientLight = horrorAmbient;
    }
}