using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScenes : MonoBehaviour
{
    public string sceneName;
    public Image Panel;
    public float fadeDuration = 1f;
    public GameObject Illu;

    private bool isTransitioning = false; // 중복 방지 플래그
    private bool hasLoaded = false;       // 다음 씬 넘어간 후 입력 방지용

    void Start()
    {
        // Panel은 유지, ChangeScenes는 다음 씬에서 파괴
        DontDestroyOnLoad(Panel.transform.root.gameObject);
    }

    IEnumerator FadeOutAndLoad()
    {
        isTransitioning = true;
        Panel.gameObject.SetActive(true);

        Color alpha = Panel.color;
        float time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeDuration;
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.completed += (_) =>
        {
            StartCoroutine(FadeIn());
            hasLoaded = true;
        };

        yield return null;
    }

    IEnumerator FadeIn()
    {
        Color alpha = Panel.color;
        float time = 0f;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / fadeDuration;
            alpha.a = Mathf.Lerp(1, 0, time);
            Panel.color = alpha;
            Destroy(Illu);
            yield return null;
        }

        Panel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isTransitioning && !hasLoaded && Input.anyKeyDown)
        {
            StartCoroutine(FadeOutAndLoad());
        }
    }
}
