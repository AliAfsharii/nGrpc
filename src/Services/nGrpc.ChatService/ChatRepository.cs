using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace nGrpc.ChatService
{
    public class ChatRepository : IChatRepository
    {
        private readonly IDBProvider _dBProvider;

        public ChatRepository(IDBProvider dBProvider)
        {
            _dBProvider = dBProvider;
        }


        public async Task<List<ChatMessage>> GetLastChatMessages(string roomName, int lastChatsCount)
        {
            using (var conn = _dBProvider.GetConnection())
            {
                await conn.OpenAsync();
                string query = @"
select array_to_json(array_agg(data)) from
(select data from chatMessages
where roomName = @RoomName 
order by id desc
limit @Count) as a;";
                string json = await conn.QueryFirstOrDefaultAsync<string>(query, new { RoomName = roomName, Count = lastChatsCount });

                var res = json.ToObject<List<ChatMessage>>() ?? new List<ChatMessage>();
                return res.OrderBy(n => n.Id).ToList();
            }
        }

        public async Task SaveChats(List<ChatMessage> chatMessages)
        {
            if (chatMessages == null || chatMessages.Count == 0)
                return;

            using (var conn = _dBProvider.GetConnection())
            {
                await conn.OpenAsync();
                string query = "insert into chatMessages(id, roomName, data) values(@Id, @RoomName, @Data::jsonb);";
                var parametersList = chatMessages.Select(n => new { Id = n.Id, RoomName = n.RoomName, Data = n.ToJson() });
                await conn.ExecuteAsync(query, parametersList);
            }
        }
    }
}
