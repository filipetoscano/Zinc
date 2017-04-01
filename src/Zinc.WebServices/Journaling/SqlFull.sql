
insert into ZN_SERVICE_JOURNAL
(
    ExecutionId, Method, ActivityId, AccessToken,
    RequestXml, ResponseXml, ErrorXml, MomentStart,
    MomentEnd
)
values
(
    @ExecutionId, @Method, @ActivityId, @AccessToken,
    @RequestXml, @ResponseXml, @ErrorXml, @MomentStart,
    @MomentEnd
);
