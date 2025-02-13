using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AsyncLoad : MonoBehaviour
{
    public void LoadScene(int index)
    {
        StartCoroutine(Loads(index));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator Loads (int scene_index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene_index);
        while(!operation.isDone)
        {
            yield return null;
        }
    }
}
