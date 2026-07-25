using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GlyphRenderer : MonoBehaviour
{
    [SerializeField][Multiline] protected string glyphString;
    protected TextMeshProUGUI text;
    protected bool zxc = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += SwapInput;
        text = GetComponent<TextMeshProUGUI>();
    }

    void SwapInput(int beatNum)
    {
        if (beatNum % 2 == 0)
        {
            zxc = !zxc;
            Render();
        }
    }

    protected virtual void Render()
    {
        // string theString =
    }

    void OnDestroy()
    {
        AudioManager.OnBeat -= SwapInput;
    }
}
