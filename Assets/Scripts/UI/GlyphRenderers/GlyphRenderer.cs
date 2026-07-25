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
        string theString = glyphString.Replace("Q", $"<sprite=\"{(zxc ? "glyph_z" : "glyph_q")}\" index=0 tint=1>");
        theString = theString.Replace("E", $"<sprite=\"{(zxc ? "glyph_x" : "glyph_e")}\" index=0 tint=1>");
        text.text = theString;
    }

    public void Activate()
    {
        text.enabled = true;
    }

    public void Deactivate()
    {
        text.enabled = false;
    }

    void OnDestroy()
    {
        AudioManager.OnBeat -= SwapInput;
    }
}
