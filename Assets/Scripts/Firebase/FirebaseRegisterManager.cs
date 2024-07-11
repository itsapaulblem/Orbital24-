using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase; 
using Firebase.Auth; 
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

/**
 * Manages user registration with Firebase Authentication.
 */
public class FirebaseRegisterManager : MonoBehaviour
{
    [Header("Firebase")]
    /**
     * The dependency status of Firebase.
     */
    public DependencyStatus dependencyStatus;
    /**
     * The Firebase authentication instance.
     */
    public FirebaseAuth auth; 
    /**
     * The current Firebase user.
     */
    public FirebaseUser user; 

    [Header("Register")]
    /**
     * The input field for the username.
     */
    public TMP_InputField usernameRegisterField;
    /**
     * The input field for the email.
     */
    public TMP_InputField emailRegisterField; 
    /**
     * The input field for the password.
     */
    public TMP_InputField passwordRegisterField;
    /**
     * The input field for verifying the password.
     */
    public TMP_InputField passwordRegisterVerifyField; 
    /**
     * The text component to display warnings.
     */
    public TMP_Text warningRegisterText; 
    /**
     * The text component to display email-related messages.
     */
    public TMP_Text emailText; 
    /**
     * The text component to display success messages.
     */
    public TMP_Text successText; 

    /**
     * The email menu game object.
     */
    public GameObject EmailMenu; 

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

    /**
     * Initializes Firebase authentication.
     */
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    /**
     * Called when the register button is pressed.
     */
    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    /**
     * Coroutine to handle user registration.
     * 
     * @param _email The email address to register with.
     * @param _password The password to register with.
     * @param _username The username to register with.
     */
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
                        EmailMenu.SetActive(true);
                        warningRegisterText.text = ""; 
                        SendEmailForVerification();
                    }
                }
            }
        }
    }

    /**
     * Sends an email verification request to the user.
     */
    public void SendEmailForVerification(){
        StartCoroutine(SendEmailForVerificationAsync());
    }

    /**
     * Coroutine to send an email verification request to the user.
     */
    private IEnumerator SendEmailForVerificationAsync(){
        if (user!= null){
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(()=> sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception!= null){
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError) firebaseException.ErrorCode; 
                emailText.text = "Unknown Error: Please try again later"; 
                
                switch(error){
                    case AuthError.Cancelled:
                      emailText.text = "Email Verification was cancelled"; 
                        break; 
                    case AuthError.Unimplemented:
                      emailText.text = "Too many request";
                        break;
                    case AuthError.Failure: 
                       emailText.text = "The email you entered is invalid";
                        break;

                }
            }
            else{
               successText.text = $"Email has successfully sent to {user.Email}";
            }
        }
    }
}