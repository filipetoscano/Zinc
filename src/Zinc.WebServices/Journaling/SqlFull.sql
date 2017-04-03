
insert into ZN_SERVICE_JOURNAL
(
    [Application], ExecutionId, Method, ActivityId,
    AccessToken, RequestXml, ResponseXml, ErrorXml,
    MomentStart, MomentEnd
)
values
(
    @Application, @ExecutionId, @Method, @ActivityId,
    @AccessToken, @RequestXml, @ResponseXml, @ErrorXml,
    @MomentStart, @MomentEnd
);
