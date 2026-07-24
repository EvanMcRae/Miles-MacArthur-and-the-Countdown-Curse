using TMPro;
using UnityEngine;

// TODO: This class only exists because we are too lazy to use input glyphs to tell players how to do things
// So on web, ESC is replaced with TAB because ESC is force assigned to the fullscreen exit
// Therefore the text ingame that tells you to pause with ESC is wrong and must be reassigned only for web

[RequireComponent(typeof(TextMeshProUGUI))]
public class WebTextOverride : MonoBehaviour
{
    [SerializeField] [Multiline] private string replacementText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Utils.IsWebPlayer())
        {
            GetComponent<TextMeshProUGUI>().text = replacementText;
        }
    }
}
