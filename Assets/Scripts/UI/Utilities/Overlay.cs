using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class Overlay : MonoBehaviour
{
    [SerializeField] private GameObject firstSelection;
    private GameObject previousSelection;
    private Canvas _canvas;
    [SerializeField] private bool _restoresSelection = true;

    virtual protected void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    virtual protected void OnEnable()
    {
        _canvas.enabled = true;
        SetSelection();
    }

    virtual protected void OnDisable()
    {
        if (EventSystem.current != null)
        {
            RestoreSelection();
        }
        _canvas.enabled = false;
    }

    protected void SetSelection()
    {
        if (firstSelection == null) return;
        previousSelection = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(firstSelection);
    }

    protected void RestoreSelection()
    {
        if (firstSelection == null) return;
        EventSystem.current.SetSelectedGameObject(_restoresSelection ? previousSelection : null);
        previousSelection = null;
    }

    virtual protected void Update()
    {

    }
}
