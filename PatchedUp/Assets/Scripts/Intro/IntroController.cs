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

    [Header("Lighting")]
    public Light directionalLight;
    public Color warmColor = new Color(1f, 0.95f, 0.8f);
    public Color horrorColor = new Color(0.4f, 0.7f, 0.4f);
    public float lightTransitionDuration = 1.5f;

    [Header("Scene")]
    public string nextSceneName = "GameScene";

    private bool _fadingOut = false;

    private void Start()
    {
        _fadingOut = false;

        // Turn brown bear ON, turn patched bear OFF
        teddyBrown.SetActive(true);
        teddyPatched.SetActive(false);

        blackPanel.alpha = 0f;

        if (directionalLight != null)
            directionalLight.color = warmColor;

        // disable ChildAI and NavMeshAgent
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
        // 1. Fade to black
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        // 2. Hide Brown, Show Patched Born while it is pitch black
        teddyBrown.SetActive(false);
 
        child.SetActive(false);
        teddyPatched.SetActive(true);

        // 3. Freeze the animation instantly so he stays laying down in the dark
        Animator anim = teddyPatched.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = 0f;
        }

        yield return new WaitForSeconds(0.5f);

        // 4. Start changing the light color (don't wait for it to finish)
        if (directionalLight != null)
            StartCoroutine(TransitionLight(warmColor, horrorColor));

        // 5. Fade the screen back in so you see him laying there
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        // 6. NOW unfreeze the animation exactly when the light is fully back!
       
        if (anim != null)
        {
            anim.speed = 1f;
        }

        // Wait a few seconds for the getting-up animation to finish before loading next scene
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

    private IEnumerator TransitionLight(Color from, Color to, float duration = -1f)
    {
        if (duration < 0) duration = lightTransitionDuration;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            directionalLight.color = Color.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        directionalLight.color = to;
    }
}