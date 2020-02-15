using UnityEngine;
using nGrpc.Client.GrpcServices;
using System.Threading.Tasks;
using nGrpc.Client;
using nGrpc.Common;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField]
    Text PlayerInfo;

    ProfileGrpcSerivce _profileGrpcSerivce;

    RegisterRes _registerRes
    {
        get
        {
            string json = PlayerPrefs.GetString("RegisterRes", null);
            var res = json.ToObject<RegisterRes>();
            return res;
        }

        set
        {
            PlayerPrefs.SetString("RegisterRes", value.ToJson());
        }
    }

    private void Start()
    {
        ShowPlayerInfo();
    }

    private void ShowPlayerInfo()
    {
        if (_registerRes != null)
            PlayerInfo.text = $"PlayerId: {_registerRes.PlayerId}";
        else
            PlayerInfo.text = $"There is no player.";
    }

    private async Task CreateProfileService()
    {
        if (_profileGrpcSerivce == null)
        {
            await ChannelProvider.Instance.ChannelConnect();
            _profileGrpcSerivce = new ProfileGrpcSerivce(ChannelProvider.Instance.GrpcChannel);
        }
    }

    public async void Event_RegisterButton()
    {
        await CreateProfileService();

        _registerRes = await _profileGrpcSerivce.RegisterRPC(new RegisterReq());
        ShowPlayerInfo();
        Debug.Log($"New player registered. RegisterRes:{_registerRes.ToJson()}");
    }

    public async void Event_LoginButton()
    {
        await CreateProfileService();

        LoginReq req = new LoginReq
        {
            PlayerId = _registerRes.PlayerId,
            SecretKey = _registerRes.SecretKey
        };
        LoginRes res = await _profileGrpcSerivce.LoginRPC(req);
        Debug.Log($"Login Done. PlayerId:{req.PlayerId}");

        SceneManager.LoadScene("MenuScene");
    }
}
