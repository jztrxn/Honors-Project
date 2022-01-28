using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        if (quitButton == null)
            quitButton = GetComponent<Button>();

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}
