using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public Text ammoText;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetAmmoText(string text)
    {
        ammoText.text = text;
    }
}
