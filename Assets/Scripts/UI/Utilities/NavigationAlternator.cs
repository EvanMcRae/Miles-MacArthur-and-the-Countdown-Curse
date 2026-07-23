using UnityEngine;
using UnityEngine.UI;

public class NavigationAlternator : MonoBehaviour
{
    public Navigation leftNav, rightNav;

    public void SetLeftNav()
    {
        GetComponent<Selectable>().navigation = leftNav;
    }

    public void SetRightNav()
    {
        GetComponent<Selectable>().navigation = rightNav;
    }
}