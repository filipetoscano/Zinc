

create table ZN_SERVICE_JOURNAL
(
    ExecutionId uniqueidentifier,
    Method nvarchar(100),
    ActivityId uniqueidentifier,
    MessageXml xml,
    Direction bit,
    Moment datetime
) on [PRIMARY];


create table ZN_SOAP_JOURNAL
(
    [Application] varchar(100) not null,
    ActivityId uniqueidentifier not null,
    AccessToken varchar(500) null,
    ExecutionId uniqueidentifier not null,
    [Action] nvarchar(200) not null,
    Direction bit not null,
    XmlMessage xml null,
    Moment datetime not null
) on [PRIMARY];


create table ZN_REST_JOURNAL
(
    [Application] varchar(100) not null,
    ActivityId uniqueidentifier not null,
    AccessToken varchar(500) null,
    ExecutionId uniqueidentifier not null,
    Method varchar(10) not null,
    URI varchar(500) not null,
    Direction bit not null,
    StatusCode varchar(50) null,
    JsonMessage nvarchar(max) null,
    Moment datetime not null
) on [PRIMARY];

/* eof */