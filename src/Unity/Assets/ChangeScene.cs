using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{    public void ChangeToScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}
