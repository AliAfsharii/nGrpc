using nGrpc.Client.GrpcServices;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    ProfileGrpcSerivce _profileGrpcSerivce;

    async void Start()
    {
        _profileGrpcSerivce = new ProfileGrpcSerivce(ChannelProvider.Instance.GrpcChannel);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
