
insert into SVC_JOURNAL ( ExecutionId, Method, ActivityId, MessageXml, Direction, Moment )
values ( @ExecutionId, @Method, @ActivityId, @Response, 1, @MomentEnd );
