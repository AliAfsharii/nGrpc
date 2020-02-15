using nGrpc.Client;
using System.Threading.Tasks;
using UnityEngine;

public class ChannelProvider : MonoBehaviour
{
    public static ChannelProvider Instance { get; private set; }

    [SerializeField]
    string Host;
    [SerializeField]
    int Port;
    public GrpcChannel GrpcChannel { get; } = new GrpcChannel();

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void OnDestroy()
    {
        await GrpcChannel.Disconnect();
    }

    public async Task ChannelConnect()
    {
        await GrpcChannel.Connect(Host, Port);
    }
}
