using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * FirebaseLoginManager is a script that handles user login and password reset functionality using Firebase Authentication.
 */
public class FirebaseLoginManager : MonoBehaviour
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
    /**
     * The Firebase database reference.
     */
    public DatabaseReference DBreference;

    [Header("Login")]
    /**
     * The email input field for login.
     */
    public TMP_InputField emailLoginField;
    /**
     * The password input field for login.
     */
    public TMP_InputField passwordLoginField;
    /**
     * The warning text for login errors.
     */
    public TMP_Text warningLoginText;
    /**
     * The confirmation text for successful login.
     */
    public TMP_Text confirmLoginText;

    [Header("Forgot Password")]
    /**
     * The email input field for forgot password.
     */
    public TMP_InputField emailForgotPasswordField;
    /**
     * The forgot password screen game object.
     */
    public GameObject ForgotPasswordScreen;
    /**
     * The login screen game object.
     */
    public GameObject LoginScreen;
    /**
     * The warning text for password reset errors.
     */
    public TMP_Text warningPasswordText;
    /**
     * The confirmation text for successful password reset.
     */
    public TMP_Text confirmPasswordText;

    /**
     * Initializes the Firebase authentication and database.
     */
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

    /**
     * Initializes the Firebase authentication and database.
     */
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        if (auth == null)
        {
            Debug.LogError("Failed to initialize FirebaseAuth.");
        }
    }

    /**
     * Clears the login fields.
     */
    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    /**
     * Goes back to the login screen from the forgot password screen.
     */
    public void Back()
    {
        ForgotPasswordScreen.SetActive(false);
        LoginScreen.SetActive(true);
    }

    /**
     * Handles the login button click.
     */
    public void LoginButton()
    {
        string email = emailLoginField.text.Trim();
        string password = passwordLoginField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email or Password input is empty.");
            return;
        }

        StartCoroutine(LoginRoutine(email, password));
    }

    /**
     * Logs in the user with the provided email and password.
     * 
     * @param _email The email address of the user.
     * @param _password The password of the user.
     */
    private IEnumerator LoginRoutine(string _email, string _password)
    {
        if (auth == null)
        {
            yield break;
        }

        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError($"Failed to login with {_email}: {loginTask.Exception}");

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
            user = loginTask.Result.User;

            if (!user.IsEmailVerified)
            {
                if (warningLoginText != null)
                {
                    warningLoginText.text = "Please verify your email address before logging in.";
                    StartCoroutine(SendVerificationEmail(user));
                }
                else
                {
                    Debug.LogError("Warning Login Text is not set.");
                }
                yield break;
            }

            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";

            yield return new WaitForSeconds(2);
            confirmLoginText.text = "";

            LoginScreen.SetActive(false);
            ClearLoginFields();

            Debug.Log("Loading last scene and player progress");
            if (PlayerPrefsManager.CheckCutscene(1))
            {
                SceneManager.LoadScene("Cutscene1");
            }
            else
            {
                string lastScene = PlayerPrefsManager.LoadLastScene();
                PlayerPrefsManager.LoadCoords();
                // TODO: change to last scene
                SceneManager.LoadScene("Cutscene1");
            }
        }
    }

    /**
     * Sends a verification email to the user.
     * 
     * @param user The Firebase user to send the verification email to.
     */
    private IEnumerator SendVerificationEmail(FirebaseUser user)
    {
        var emailTask = user.SendEmailVerificationAsync();

        yield return new WaitUntil(() => emailTask.IsCompleted);

        if (emailTask.Exception!= null)
        {
            Debug.LogError($"Failed to send verification email: {emailTask.Exception}");
        }
        else
        {
            Debug.Log("Verification email sent successfully.");
        }
    }

    /**
     * Shows the forgot password screen.
     */
    public void ShowForgotPasswordScreen()
    {
        ForgotPasswordScreen.SetActive(true);
        LoginScreen.SetActive(false);
    }

    /**
     * Handles the reset password button click.
     */
    public void ResetPasswordButton()
    {
        StartCoroutine(SendPasswordResetEmail(emailForgotPasswordField.text));
    }

    /**
     * Sends a password reset email to the user.
     * 
     * @param _email The email address of the user to send the password reset email to.
     */
    private IEnumerator SendPasswordResetEmail(string _email)
    {
        var resetTask = auth.SendPasswordResetEmailAsync(_email);

        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.Exception!= null)
        {
            Debug.LogError($"Failed to send password reset email: {resetTask.Exception}");

            FirebaseException firebaseEx = resetTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Password reset failed";
            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                    message = "Invalid email address.";
                    break;

                case AuthError.UserNotFound:
                    message = "User with this email does not exist.";
                    break;

                case AuthError.NetworkRequestFailed:
                    message = "Network error. Please check your connection and try again.";
                    break;

                default:
                    message = "Failed to reset password. Please try again later.";
                    break;
            }

            warningPasswordText.text = message;
        }
        else
        {
            Debug.Log("Password reset email sent successfully.");
            confirmPasswordText.text = "Password reset email sent. Please check your email.";

            yield return new WaitForSeconds(3);

            ForgotPasswordScreen.SetActive(false);
            LoginScreen.SetActive(true);
        }
    }
}