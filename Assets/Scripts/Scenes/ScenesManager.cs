using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(ChanceScene("Game"));
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator ChanceScene(string name)
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(name);
    }
    public void QuitGame()
    {
        StartCoroutine(Quit());
    }
    IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.3f);
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}


