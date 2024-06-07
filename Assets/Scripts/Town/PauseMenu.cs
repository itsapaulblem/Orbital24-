// using UnityEngine;
// using UnityEngine.SceneManagement;
// using Cinemachine;


// public class PauseMenu : MonoBehaviour
// {
//     public GameObject pauseMenu;
//     public bool isPaused;

//     void Start()
//     {
//         pauseMenu.SetActive(false);
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.P))
//         {
//             if (isPaused)
//             {
//                 ResumeGame();
//             }
//             else
//             {
//                 PauseGame();
//             }
//         }

//         // Manually update Cinemachine virtual cameras if the game is paused
//         if (isPaused)
//         {
//             UpdateCinemachineCameras();
//         }
//     }

//     public void PauseGame()
//     {
//         pauseMenu.SetActive(true);
//         Time.timeScale = 0f;
//         isPaused = true;
//     }

//     public void ResumeGame()
//     {
//         pauseMenu.SetActive(false);
//         Time.timeScale = 1f;
//         isPaused = false;
//     }

//     public void Quit()
//     {
//         Time.timeScale = 1f;  
//         PlayerPrefs.SetInt("ShowUserDataUI",1);
//          SceneManager.LoadScene("Start");
//     }

//     public void RestartGame()
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }

//     private void UpdateCinemachineCameras()
//     {
//         // Find all active Cinemachine virtual cameras in the scene
//         var virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();

//         // Update each Cinemachine virtual camera manually
//         foreach (var vCam in virtualCameras)
//         {
//             vCam.UpdateCameraState(Vector3.zero, Time.unscaledDeltaTime);
//         }
//     }
// }
