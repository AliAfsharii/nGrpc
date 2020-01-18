create table ServerLogs(
	id serial8 primary key,
	message text,
	message_template text,
	level varchar,
	raise_date timestamp without time zone,
	exception text,
	properties jsonb,
	props_test jsonb,
	machine_name text
);