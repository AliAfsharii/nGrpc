using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Grpc.Core;
using nGrpc.ServerCommon;
using nGrpc.Common;
using Nito.AsyncEx;

namespace nGrpc.Grpc
{
    public class ServerEventStreamsManager : IServerEventStreamsManager
    {
        readonly ILogger<ServerEventStreamsManager> _logger;

        public ServerEventStreamsManager(ILogger<ServerEventStreamsManager> logger)
        {
            _logger = logger;
        }


        readonly ConcurrentDictionary<int, ServerStreamWriter> StreamsDic = new ConcurrentDictionary<int, ServerStreamWriter>();

        public int StreamsCount
        {
            get
            {
                return StreamsDic.Count;
            }
        }

        public void AddStream(int playerId, IServerStreamWriter<ServerEventRes> streamWriter)
        {
            var ssw = new ServerStreamWriter(streamWriter);
            StreamsDic.AddOrUpdate(playerId, ssw, (a, b) => ssw);
        }

        public void RemoveStream(int playerId)
        {
            StreamsDic.TryRemove(playerId, out var a);
        }

        public async Task SendEventToPlayer(int playerId, ServerEventRes serverEvent)
        {
            if (StreamsDic.TryGetValue(playerId, out var streamWriter) == true)
            {
                try
                {
                    await streamWriter.WriteAsync(serverEvent);
                }
                catch (Exception e)
                {
                    StreamsDic.TryRemove(playerId, out var a);
                    _logger?.LogError(e, "ServerStreamWriter.WriteAsync Error, PlayerId:{pi}", playerId);
                }
            }
        }

        public async Task SendEventToPlayers(IEnumerable<int> playerIds, ServerEventRes serverEvent)
        {
            var tasks = playerIds.Select(async pi =>
            {
                if (StreamsDic.TryGetValue(pi, out var streamWriter) == true)
                {
                    try
                    {
                        await streamWriter.WriteAsync(serverEvent);
                    }
                    catch (Exception e)
                    {
                        StreamsDic.TryRemove(pi, out var a);
                        _logger?.LogInformation(e, "ServerStreamWriter.WriteAsync Error, PlayerId:{pi}", playerId);
                    }
                }
            });

            await Task.WhenAll(tasks);
        }

        public async Task SendEventToAllPlayers(ServerEventRes serverEvent)
        {
            var tasks = StreamsDic.Select(async pair =>
            {
                int playerId = pair.Key;
                ServerStreamWriter streamWriter = pair.Value;

                try
                {
                    await streamWriter.WriteAsync(serverEvent);
                }
                catch (Exception e)
                {
                    StreamsDic.TryRemove(playerId, out var a);
                    _logger?.LogError(e, "ServerStreamWriter.WriteAsync Error, PlayerId:{pi}", playerId);
                }
            });

            await Task.WhenAll(tasks);
        }


        class ServerStreamWriter
        {
            readonly AsyncLock _asyncLock = new AsyncLock();
            readonly IServerStreamWriter<ServerEventRes> _streamWriter;

            internal ServerStreamWriter(IServerStreamWriter<ServerEventRes> streamWriter)
            {
                _streamWriter = streamWriter;
            }

            internal async Task WriteAsync(ServerEventRes message)
            {
                using (await _asyncLock.LockAsync())
                {
                    await _streamWriter.WriteAsync(message);
                }
            }
        }
    }
}
