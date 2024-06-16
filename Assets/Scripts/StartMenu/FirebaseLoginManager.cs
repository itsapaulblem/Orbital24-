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

    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField timeField;
    public TMP_InputField killField;
    public TMP_InputField deathField;
    public GameObject UserData_UI;
    public GameObject LoginScreen;

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
        if (PlayerPrefs.GetInt("ShowUserDataUI", 0) == 1)
        {
            ShowUserDataUI();
            PlayerPrefs.SetInt("ShowUserDataUI", 0);
        }
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

    public void SignOutButton()
    {
        auth.SignOut();
        LoginScreen.SetActive(true);
        UserData_UI.SetActive(false);
        ClearLoginFields();
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDataBase(usernameField.text));
        StartCoroutine(UpdateKills(int.Parse(killField.text)));
        StartCoroutine(UpdateDeaths(int.Parse(deathField.text)));
        StartCoroutine(UpdateTime(float.Parse(timeField.text)));
    }

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
            user = loginTask.Result.User;

            // Check if email is verified
            if (!user.IsEmailVerified)
            {
                if (warningLoginText != null)
                {
                    warningLoginText.text = "Please verify your email address before logging in.";
                }
                else
                {
                    Debug.LogError("Warning Login Text is not set.");
                }
                yield break; // Stop the login process
            }

            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);
            confirmLoginText.text = "";
            usernameField.text = user.DisplayName;

            UserData_UI.SetActive(true);
            LoginScreen.SetActive(false);
            ClearLoginFields();

            SceneManager.LoadScene("CutScene1");
        }
    }

    private IEnumerator UpdateUsernameAuth(string username)
    {
        UserProfile profile = new UserProfile { DisplayName = username };
        var profileTask = user.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => profileTask.IsCompleted);
        if (profileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {profileTask.Exception}");
        }
    }

    private IEnumerator UpdateUsernameDataBase(string username)
    {
        var dbTask = DBreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(username);
        yield return new WaitUntil(() => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {dbTask.Exception}");
        }
    }

    private IEnumerator UpdateKills(int kills)
    {
        var dbTask = DBreference.Child("users").Child(user.UserId).Child("kills").SetValueAsync(kills);
        yield return new WaitUntil(() => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {dbTask.Exception}");
        }
    }

    private IEnumerator UpdateDeaths(int deaths)
    {
        var dbTask = DBreference.Child("users").Child(user.UserId).Child("deaths").SetValueAsync(deaths);
        yield return new WaitUntil(() => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {dbTask.Exception}");
        }
    }

    private IEnumerator UpdateTime(float time)
    {
        var dbTask = DBreference.Child("users").Child(user.UserId).Child("time").SetValueAsync(time);
        yield return new WaitUntil(() => dbTask.IsCompleted);
        if (dbTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {dbTask.Exception}");
        }
    }

    private IEnumerator LoadUserData()
    {
        var dbTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();
        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {dbTask.Exception}");
        }
        else if (dbTask.Result.Value == null)
        {
            Debug.Log("No data found, setting default values.");
            killField.text = "0";
            deathField.text = "0";
            timeField.text = "0.00";

            killField.ForceLabelUpdate();
            deathField.ForceLabelUpdate();
            timeField.ForceLabelUpdate();
        }
        else
        {
            DataSnapshot snapshot = dbTask.Result;
            killField.text = snapshot.Child("kills").Value.ToString();
            deathField.text = snapshot.Child("deaths").Value.ToString();
            timeField.text = snapshot.Child("time").Value.ToString();

            killField.ForceLabelUpdate();
            deathField.ForceLabelUpdate();
            timeField.ForceLabelUpdate();
        }
    }

    private void ShowUserDataUI()
    {
        UserData_UI.SetActive(true);
        LoginScreen.SetActive(false);
    }

  
// GetData("StoryProgress")
// GetData("Inventory")
// GetData("Stats")
// StoreData("Key")
}
