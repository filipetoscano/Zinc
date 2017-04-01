
update ZN_SERVICE_JOURNAL
set ResponseXml = @ResponseXml,
    ErrorXml = @ErrorXml,
    MomentEnd = @MomentEnd
where ExecutionId = @ExecutionId;
