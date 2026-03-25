using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private string baseText = "Loading";
    [SerializeField] private float dotSpeed = 0.5f;

    private float timer;
    private int dotCount;

    private void Awake()
    {
        if (loadingText == null)
            loadingText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (loadingText == null) return;

        timer += Time.unscaledDeltaTime;
        if (timer >= dotSpeed)
        {
            timer = 0f;
            dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3

            string dots = new string('.', dotCount);
            loadingText.text = baseText + dots;
        }
    }
}
