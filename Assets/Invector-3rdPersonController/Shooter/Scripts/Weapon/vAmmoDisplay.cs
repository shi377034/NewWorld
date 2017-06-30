using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Invector.ItemManager;
using UnityEngine.Events;

public class vAmmoDisplay : MonoBehaviour
{
    [System.Serializable]
    public class OnChangeAmmoEvent : UnityEvent<int> { }
    [SerializeField]
    protected Text display;
    vItem currentItem;
    public UnityEvent onShow, onHide;
    public OnChangeAmmoEvent onChangeAmmo;
    private int currentAmmoId;

    void Start()
    {
        if (display == null) Destroy(gameObject);
        display.text = "";
        currentAmmoId = -1;
    }

    public void Show()
    {
        display.gameObject.SetActive(true);
        onShow.Invoke();
    }

    public void Hide()
    {
        display.gameObject.SetActive(false);
        onHide.Invoke();
    }

    public void UpdateDisplay(string text, int id = 0)
    {
        if (!text.Equals("") && !display.gameObject.activeSelf)
        {
            display.gameObject.SetActive(true);
        }
        if (currentAmmoId != id)
        {
            onChangeAmmo.Invoke(id);
            currentAmmoId = id;
        }
        display.text = text;
    }
}
