using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBreference;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Forgot Password")]
    public TMP_InputField emailForgotPasswordField;
    public GameObject ForgotPasswordScreen;
    public GameObject LoginScreen;
    public TMP_Text warningPasswordText;
    public TMP_Text confirmPasswordText;

    public string defaultSceneName = "Cutscene1";

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

        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        if (auth == null)
        {
            Debug.LogError("Failed to initialize FirebaseAuth.");
        }
    }

    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void Back()
    {
        ForgotPasswordScreen.SetActive(false);
        LoginScreen.SetActive(true);
    }

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

            Debug.Log("Loading last scene and player progress...");
            string lastScene = PlayerPrefsManager.LoadLastScene();
            if (string.IsNullOrEmpty(lastScene))
            {
                lastScene = defaultSceneName;
            }

            PlayerPrefsManager.LoadCoords();
            PlayerPrefsManager.LoadKills();
            SceneManager.LoadScene(lastScene);
        }
    }

    private IEnumerator SendVerificationEmail(FirebaseUser user)
    {
        var emailTask = user.SendEmailVerificationAsync();

        yield return new WaitUntil(() => emailTask.IsCompleted);

        if (emailTask.Exception != null)
        {
            Debug.LogError($"Failed to send verification email: {emailTask.Exception}");
        }
        else
        {
            Debug.Log("Verification email sent successfully.");
        }
    }

    public void ShowForgotPasswordScreen()
    {
        ForgotPasswordScreen.SetActive(true);
        LoginScreen.SetActive(false);
    }

    public void ResetPasswordButton()
    {
        StartCoroutine(SendPasswordResetEmail(emailForgotPasswordField.text));
    }

    private IEnumerator SendPasswordResetEmail(string _email)
    {
        var resetTask = auth.SendPasswordResetEmailAsync(_email);

        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.Exception != null)
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
