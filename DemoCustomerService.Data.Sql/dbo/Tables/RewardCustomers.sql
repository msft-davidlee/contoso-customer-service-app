﻿CREATE TABLE [dbo].[RewardCustomers] (
    [MemberId]  VARCHAR (10) NOT NULL,
    [FirstName] VARCHAR (25) NULL,
    [LastName]  VARCHAR (25) NULL,
    [Points]    INT          NULL,
    PRIMARY KEY CLUSTERED ([MemberId] ASC)
);

