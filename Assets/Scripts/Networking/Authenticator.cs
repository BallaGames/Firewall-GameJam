using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Authenticator : MonoBehaviour
{
    private void Start()
    {
        InitialiseAuthService();
    }

    public async void InitialiseAuthService()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        await UnityServices.InitializeAsync();
    }
}
