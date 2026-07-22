using UnityEngine;
using UnityEngine.UI;

public class NavigationAlternator : MonoBehaviour
{
    [SerializeField] private Navigation leftNav;
    [SerializeField] private Navigation rightNav;

    void Awake()
    {
        if (Utils.IsWebPlayer())
        {
            rightNav.selectOnUp = rightNav.selectOnDown; // Disable fullscreen toggle
        }
    }

    public void SetLeftNav()
    {
        GetComponent<Selectable>().navigation = leftNav;
    }

    public void SetRightNav()
    {
        GetComponent<Selectable>().navigation = rightNav;
    }
}