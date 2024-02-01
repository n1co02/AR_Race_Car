using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetIp : MonoBehaviour
{
    public TMP_InputField ipInputField = null;
    public TMP_Text setIpFeedbackText = null;
    void Start()
    {
        string currentlySetRaspiIp = PlayerPrefs.GetString("raspiIpAddress");
        ipInputField.text = currentlySetRaspiIp;
    }

    private void Update()
    {
        if(ipInputField.text == PlayerPrefs.GetString("raspiIpAddress"))
        {
            setIpFeedbackText.gameObject.SetActive(true);
        }
        else
        {
            setIpFeedbackText.gameObject.SetActive(false);
        }
    }

    public void setRaspiIp()
    {
        PlayerPrefs.SetString("raspiIpAddress", ipInputField.text);
        setIpFeedbackText.gameObject.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
