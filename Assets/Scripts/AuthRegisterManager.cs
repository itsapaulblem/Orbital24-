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
    

    public void Awake(){
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available){
                InitializeFirebase();
            }
            else{
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase(){
        Debug.Log("Setting up Firebase Auth");

        // set the authentication instance object 
        auth = FirebaseAuth.DefaultInstance;
    }
    // function for the register button 
    public void RegisterButton(){
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }


    private IEnumerator Register(string _email, string _password, string _username){
        if (_username == ""){
            warningRegisterText.text = "Missing Username";
        }

        else if (passwordRegisterField.text != passwordRegisterVerifyField.text){
            warningRegisterText.text = "Password does not match!";
        }
        else{
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate : () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null){
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError) firebaseEx.ErrorCode;

                string message = " Register failed!";
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
            else{
                AuthResult result = RegisterTask.Result; 
                user = result.User; 
                if (user!= null)
                {
                    UserProfile profile = new UserProfile {DisplayName = _username};
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null){
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError) firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }

                    else{
                        SceneManager.LoadSceneAsync("Start");
                         warningRegisterText.text = ""; 
                    }
                }
            }
        }
    }
    
}
