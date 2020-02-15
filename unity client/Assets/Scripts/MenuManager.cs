using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    Text PlayerNameText;
    [SerializeField]
    InputField NewNameInput;

    ProfileGrpcSerivce _profileGrpcSerivce;

    async void Start()
    {
        _profileGrpcSerivce = new ProfileGrpcSerivce(ChannelProvider.Instance.GrpcChannel);
        GetPlayerDataRes res = await _profileGrpcSerivce.GetPlayerDataRPC(new GetPlayerDataReq());
        UpdatePlayerNameText(res.PlayerData.Name);
    }

    void UpdatePlayerNameText(string name)
    {
        PlayerNameText.text = $"Current Name : {name}";
    }


    public async void Event_ChangeNameButton()
    {
        ChangeNameReq req = new ChangeNameReq
        {
            NewName = NewNameInput.text
        };
        var res = await _profileGrpcSerivce.ChangeNameRPC(req);
        UpdatePlayerNameText(res.PlayerData.Name);
    }
}
