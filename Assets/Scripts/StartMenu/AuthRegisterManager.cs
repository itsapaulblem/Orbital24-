using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase; 
using Firebase.Auth; 
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class AuthRegisterManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth; 
    public FirebaseUser user; 

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField; 
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField; 
    public TMP_Text warningRegisterText; 

    // Called when the script instance is being loaded
    public void Awake()
    {
        // Check and fix Firebase dependencies asynchronously
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

    // Initialize Firebase authentication
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    // Called when the register button is pressed
    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    // Coroutine to handle user registration
    private IEnumerator Register(string _email, string _password, string _username)
    {
        // Check if username is empty
        if (string.IsNullOrEmpty(_username))
        {
            warningRegisterText.text = "Missing Username";
        }
        // Check if passwords match
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password does not match!";
        }
        else
        {
            // Attempt to create a new user with email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(() => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                // Handle registration errors
                string message = "Register failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already in Use";
                        break;
                }
                warningRegisterText.text = message; 
            }
            else
            {
                AuthResult result = RegisterTask.Result; 
                user = result.User; 
                if (user != null)
                {
                    // Set the user profile with the provided username
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(() => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning($"Failed to update profile with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        // Load the Start scene on successful registration
                        SceneManager.LoadSceneAsync("Start");
                        warningRegisterText.text = ""; 
                    }
                }
            }
        }
    }
}
