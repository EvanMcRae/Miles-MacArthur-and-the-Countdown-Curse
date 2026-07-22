using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasBorder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Image>().enabled = true;

#if UNITY_EDITOR
        SceneVisibilityManager.instance.Hide(gameObject, false);
#endif
    }
}
