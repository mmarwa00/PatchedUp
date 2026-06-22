using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class StorySceneController : MonoBehaviour
{
    [Header("Pages")]
    public CanvasGroup page1;
    public CanvasGroup page2;
    public CanvasGroup page3;
    public CanvasGroup page4;
    public CanvasGroup page5;

    [Header("Texts")]
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;
    public TMP_Text text5;

    [Header("Final Fade")]
    public CanvasGroup blackPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip storyMusic;
    public float musicFadeInDuration = 2f;
    public float musicFadeOutDuration = 2f;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float waitBetweenPages = 3f;
    public float waitBeforeFinalFade = 2f;
    public float letterDelay = 0.08f;

    private string[] _fullTexts;

    private void Start()
    {
        page1.alpha = 0f;
        page2.alpha = 0f;
        page3.alpha = 0f;
        page4.alpha = 0f;
        page5.alpha = 0f;
        blackPanel.alpha = 1f;

        // Store full texts BEFORE clearing
        _fullTexts = new string[]
        {
            text1.text, text2.text, text3.text, text4.text, text5.text
        };

        // Clear all texts
        text1.text = "";
        text2.text = "";
        text3.text = "";
        text4.text = "";
        text5.text = "";

        audioSource.clip = storyMusic;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        StartCoroutine(PlayStory());
    }

    private IEnumerator PlayStory()
    {
        yield return StartCoroutine(FadeCanvasGroup(blackPanel, 1f, 0f, fadeDuration));
        StartCoroutine(FadeAudio(0f, 1f, musicFadeInDuration));

        yield return StartCoroutine(FadeCanvasGroup(page1, 0f, 1f, fadeDuration));
        yield return StartCoroutine(TypeText(text1, 0));
        yield return new WaitForSeconds(waitBetweenPages);

        yield return StartCoroutine(FadeCanvasGroup(page2, 0f, 1f, fadeDuration));
        yield return StartCoroutine(TypeText(text2, 1));
        yield return new WaitForSeconds(waitBetweenPages);

        yield return StartCoroutine(FadeCanvasGroup(page3, 0f, 1f, fadeDuration));
        yield return StartCoroutine(TypeText(text3, 2));
        yield return new WaitForSeconds(waitBetweenPages);

        yield return StartCoroutine(FadeCanvasGroup(page4, 0f, 1f, fadeDuration));
        yield return StartCoroutine(TypeText(text4, 3));
        yield return new WaitForSeconds(waitBetweenPages);

        yield return StartCoroutine(FadeCanvasGroup(page5, 0f, 1f, fadeDuration));
        yield return StartCoroutine(TypeText(text5, 4));
        yield return new WaitForSeconds(waitBeforeFinalFade);

        StartCoroutine(FadeAudio(1f, 0f, musicFadeOutDuration));
        yield return StartCoroutine(FadeCanvasGroup(blackPanel, 0f, 1f, fadeDuration));

        SceneManager.LoadScene("zone1_greyboxing");
    }

    private IEnumerator TypeText(TMP_Text textComponent, int index)
    {
        textComponent.text = _fullTexts[index];
        textComponent.ForceMeshUpdate();
        int totalChars = textComponent.textInfo.characterCount;
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalChars; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(letterDelay);
        }
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    private IEnumerator FadeAudio(float from, float to, float duration)
    {
        float elapsed = 0f;
        audioSource.volume = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        audioSource.volume = to;
    }
}