﻿using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.Common.Models;
using System.Threading.Tasks;

namespace nGrpc.Client.GrpcServices
{
    public class ProfileGrpcSerivce 
    {
        private readonly GrpcChannel _grpcChannel;

        public ProfileGrpcSerivce(GrpcChannel grpcChannel)
        {
            _grpcChannel = grpcChannel;
        }

        public async Task<RegisterRes> RegisterRPC(RegisterReq req)
        {
            var res = await _grpcChannel.CallRpc(ProfileDescriptors.RegisterDesc, req);
            return res;
        }

        public async Task<LoginRes> LoginRPC(LoginReq req)
        {
            var res = await _grpcChannel.CallRpc(ProfileDescriptors.LoginDesc, req);

            // Cache header
            if (res != null)
            {
                _grpcChannel.PlayerCredential = new PlayerCredentials { PlayerId = res.PlayerId, SessionId = res.SessionId };
            }

            return res;
        }
    }
}