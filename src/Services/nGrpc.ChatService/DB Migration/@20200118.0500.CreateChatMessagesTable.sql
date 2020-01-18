create table ChatMessages(
	id int,
	roomName text not null,
	data jsonb not null,
	primary key (id, roomname)
);

create index chat_message_room_name_index on chatmessages(roomname);