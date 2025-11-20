using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{

    public float hp = 100f;
    public GameObject overUI;
    public Image fadeImage;
    public GameObject returnButton;
    public GameObject gmaeOverImage;
    private bool isDead = false;
    
    void Start()
    {
        overUI.SetActive(false);
    }

    void Hit()
    {
        if (isDead) return;

        hp -= 50;
        if(hp <= 0)
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        if (isDead) return;

        isDead = true;
        Time.timeScale = 0.2f;
        overUI.SetActive(true);
        
        StartCoroutine(FadeToBlack());

    }
    
    IEnumerator FadeToBlack()
    {

        yield return new WaitForSecondsRealtime(1.5f);

        
        float fadeDuration = 1.5f;
        float timer = 0f;

        Color startColor = new Color(255f, 255f, 255f, 0f);
        Color endColor = new Color(255f, 255f, 255f, 1f);

        fadeImage.color = startColor;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = Color.Lerp(startColor, endColor, progress);

            yield return null;
        }
        fadeImage.color = endColor;
        Time.timeScale = 0f;
        //returnButton.SetActive(true);
        //gmaeOverImage.SetActive(true);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Hit();
        }
    }
}
