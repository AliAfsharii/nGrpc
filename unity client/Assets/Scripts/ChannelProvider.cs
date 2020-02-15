using nGrpc.Client;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChannelProvider : MonoBehaviour
{
    public static ChannelProvider Instance { get; private set; }

    [SerializeField]
    string Host;
    [SerializeField]
    int Port;
    GrpcChannel _grpcChannel;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<GrpcChannel> GetChannel()
    {
        if (_grpcChannel == null)
        {
            _grpcChannel = new GrpcChannel();
            await _grpcChannel.Connect(Host, Port);
        }
        return _grpcChannel;
    }
}
