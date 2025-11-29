using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour
{

    public void GoToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToHubScene()
    {
        SceneManager.LoadScene("Hub");
    }

    public void GoToResidenceScene()
    {
        SceneManager.LoadScene("Residence");
    }

    public void GoToModerneScene()
    {
        SceneManager.LoadScene("Douche Moderne");
    }

    public void GoToCreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }

}
