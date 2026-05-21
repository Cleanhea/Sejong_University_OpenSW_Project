using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Main : MonoBehaviour
{
   public void OnClickNewGame()
   {
      SceneManager.LoadScene("GameScene");
   }

   public void OnClickSettings(){
        Debug.Log("Settings button clicked");
   }

   public void OnClickQuit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
   }
}
