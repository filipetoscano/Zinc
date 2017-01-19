
insert into SVC_JOURNAL ( ExecutionId, Method, ActivityId, MessageXml, Direction, Moment )
values ( @ExecutionId, @Method, @ActivityId, @Request, 0, @MomentStart );
