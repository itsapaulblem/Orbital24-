using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    public void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        // Set the authentication instance object 
        auth = FirebaseAuth.DefaultInstance;

        if (auth == null)
        {
            Debug.LogError("Failed to initialize FirebaseAuth.");
        }
    }

    // Function for the login button 
    public void LoginButton()
    {
        if (emailLoginField == null || passwordLoginField == null)
        {
            Debug.LogError("Email or Password input field is not set.");
            return;
        }

        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        if (auth == null)
        {
           // Debug.LogError("FirebaseAuth is not initialized.");
            yield break;
        }

        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to login with {loginTask.Exception}");
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;

                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;

                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;

                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;

                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }

            if (warningLoginText != null)
            {
                warningLoginText.text = message;
            }
            else
            {
                Debug.LogError("Warning Login Text is not set.");
            }
        }
        else
        {
            user = loginTask.Result.User; // Corrected line to access the FirebaseUser
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);

            if (warningLoginText != null)
            {
                warningLoginText.text = "";
            }
            if (confirmLoginText != null)
            {
                confirmLoginText.text = "Logged in";
            }
            else
            {
                Debug.LogError("Confirm Login Text is not set.");
            }
            
            // Load the next scene on the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadSceneAsync("Cutscene1");
            });
        }
    }
}
