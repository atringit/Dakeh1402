IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [AboutUss] (
    [id] int NOT NULL IDENTITY,
    [description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_AboutUss] PRIMARY KEY ([id])
);

GO

CREATE TABLE [Categorys] (
    [id] int NOT NULL IDENTITY,
    [registerPrice] bigint NOT NULL,
    [expirePrice] bigint NOT NULL,
    [espacialPrice] bigint NOT NULL,
    [parentCategoryId] int NULL,
    [name] nvarchar(200) NOT NULL,
    [image] nvarchar(500) NULL,
    [laderPrice] bigint NOT NULL,
    [emergencyPrice] bigint NOT NULL,
    [staticemergencyPriceId] nvarchar(max) NULL,
    [staticregisterPriceId] nvarchar(max) NULL,
    [staticexpirePriceId] nvarchar(max) NULL,
    [staticespacialPriceId] nvarchar(max) NULL,
    [staticladerPriceId] nvarchar(max) NULL,
    CONSTRAINT [PK_Categorys] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Categorys_Categorys_parentCategoryId] FOREIGN KEY ([parentCategoryId]) REFERENCES [Categorys] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Cities] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(50) NULL,
    CONSTRAINT [PK_Cities] PRIMARY KEY ([id])
);

GO

CREATE TABLE [ContactUss] (
    [id] int NOT NULL IDENTITY,
    [phone] nvarchar(50) NULL,
    [pageTelegramUrl] nvarchar(200) NULL,
    [pageInstagramUrl] nvarchar(200) NULL,
    [pageTwitterUrl] nvarchar(200) NULL,
    [email] nvarchar(200) NULL,
    [androidVersion] nvarchar(20) NULL,
    CONSTRAINT [PK_ContactUss] PRIMARY KEY ([id])
);

GO

CREATE TABLE [DiscountCodes] (
    [id] int NOT NULL IDENTITY,
    [code] int NOT NULL,
    [count] int NOT NULL,
    [remain] int NOT NULL,
    [price] bigint NOT NULL,
    CONSTRAINT [PK_DiscountCodes] PRIMARY KEY ([id])
);

GO

CREATE TABLE [Informations] (
    [id] int NOT NULL IDENTITY,
    [title] nvarchar(500) NOT NULL,
    [description] nvarchar(max) NOT NULL,
    [Link] nvarchar(max) NULL,
    CONSTRAINT [PK_Informations] PRIMARY KEY ([id])
);

GO

CREATE TABLE [PaymentRequestAttemps] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [NoticeId] bigint NOT NULL,
    [FactorId] int NOT NULL,
    [pursheType] int NOT NULL,
    CONSTRAINT [PK_PaymentRequestAttemps] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [RoleNameFa] nvarchar(1000) NOT NULL,
    [RoleNameEn] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Rules] (
    [id] int NOT NULL IDENTITY,
    [description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Rules] PRIMARY KEY ([id])
);

GO

CREATE TABLE [Settings] (
    [id] int NOT NULL IDENTITY,
    [wrongWord] nvarchar(2000) NOT NULL,
    [countExpireDate] int NULL,
    [countExpireDateIsespacial] int NULL,
    [countExpireDateEmergency] int NULL,
    [showPriceForCars] bit NOT NULL,
    [AutoAccept] bit NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY ([id])
);

GO

CREATE TABLE [Sliders] (
    [id] int NOT NULL IDENTITY,
    [image] nvarchar(500) NULL,
    [link] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_Sliders] PRIMARY KEY ([id])
);

GO

CREATE TABLE [StaticPrices] (
    [id] int NOT NULL IDENTITY,
    [price] bigint NOT NULL,
    [code] nvarchar(max) NULL,
    CONSTRAINT [PK_StaticPrices] PRIMARY KEY ([id])
);

GO

CREATE TABLE [Stirs] (
    [id] int NOT NULL IDENTITY,
    [description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Stirs] PRIMARY KEY ([id])
);

GO

CREATE TABLE [Provinces] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(50) NULL,
    [cityId] int NOT NULL,
    CONSTRAINT [PK_Provinces] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Provinces_Cities_cityId] FOREIGN KEY ([cityId]) REFERENCES [Cities] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [InformationMedias] (
    [Id] int NOT NULL IDENTITY,
    [Image] nvarchar(max) NULL,
    [InformationId] int NOT NULL,
    CONSTRAINT [PK_InformationMedias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InformationMedias_Informations_InformationId] FOREIGN KEY ([InformationId]) REFERENCES [Informations] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Areas] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(50) NULL,
    [provinceId] int NOT NULL,
    CONSTRAINT [PK_Areas] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Areas_Provinces_provinceId] FOREIGN KEY ([provinceId]) REFERENCES [Provinces] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Users] (
    [id] int NOT NULL IDENTITY,
    [cellphone] nvarchar(11) NOT NULL,
    [password] nvarchar(200) NULL,
    [passwordShow] nvarchar(20) NULL,
    [token] nvarchar(100) NULL,
    [roleId] int NOT NULL,
    [code] nvarchar(6) NULL,
    [isCodeConfirmed] bit NOT NULL,
    [oTPDate] datetime2 NOT NULL,
    [provinceId] int NULL,
    [adminRole] nvarchar(50) NULL,
    [IsBlocked] bit NOT NULL,
    [PushNotifToken] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Users_Provinces_provinceId] FOREIGN KEY ([provinceId]) REFERENCES [Provinces] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Users_Roles_roleId] FOREIGN KEY ([roleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AdminsInCities] (
    [id] int NOT NULL IDENTITY,
    [userid] int NOT NULL,
    [cityId] int NOT NULL,
    CONSTRAINT [PK_AdminsInCities] PRIMARY KEY ([id]),
    CONSTRAINT [FK_AdminsInCities_Cities_cityId] FOREIGN KEY ([cityId]) REFERENCES [Cities] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AdminsInCities_Users_userid] FOREIGN KEY ([userid]) REFERENCES [Users] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Banner] (
    [Id] bigint NOT NULL IDENTITY,
    [title] nvarchar(max) NULL,
    [notConfirmDescription] nvarchar(1000) NULL,
    [Link] nvarchar(1000) NULL,
    [adminConfirmStatus] int NOT NULL,
    [userId] int NOT NULL,
    [code] nvarchar(max) NULL,
    [createDate] datetime2 NOT NULL,
    [expireDate] datetime2 NOT NULL,
    [expireDateIsespacial] datetime2 NULL,
    [countView] int NOT NULL,
    [isSpecial] bit NOT NULL,
    [isEmergency] bit NOT NULL,
    [ExpireDateEmergency] datetime2 NULL,
    [isPaid] bit NOT NULL,
    [AdminUserAccepted] nvarchar(max) NULL,
    [AcceptedDate] datetime2 NULL,
    CONSTRAINT [PK_Banner] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Banner_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Notices] (
    [id] bigint NOT NULL IDENTITY,
    [movie] nvarchar(500) NULL,
    [image] nvarchar(500) NULL,
    [title] nvarchar(200) NOT NULL,
    [price] bigint NOT NULL,
    [lastPrice] bigint NOT NULL,
    [description] nvarchar(1000) NULL,
    [notConfirmDescription] nvarchar(1000) NULL,
    [link] nvarchar(1000) NULL,
    [adminConfirmStatus] int NOT NULL,
    [userId] int NOT NULL,
    [code] nvarchar(max) NULL,
    [createDate] datetime2 NOT NULL,
    [expireDate] datetime2 NOT NULL,
    [expireDateIsespacial] datetime2 NOT NULL,
    [categoryId] int NOT NULL,
    [countView] int NOT NULL,
    [cityId] int NOT NULL,
    [provinceId] int NOT NULL,
    [areaId] int NOT NULL,
    [isSpecial] bit NOT NULL,
    [isEmergency] bit NOT NULL,
    [ExpireDateEmergency] datetime2 NULL,
    [isPaid] bit NOT NULL,
    [deletedAt] datetime2 NULL,
    [AdminUserAccepted] nvarchar(max) NULL,
    [AcceptedDate] datetime2 NULL,
    CONSTRAINT [PK_Notices] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Notices_Areas_areaId] FOREIGN KEY ([areaId]) REFERENCES [Areas] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notices_Categorys_categoryId] FOREIGN KEY ([categoryId]) REFERENCES [Categorys] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Notices_Cities_cityId] FOREIGN KEY ([cityId]) REFERENCES [Cities] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notices_Provinces_provinceId] FOREIGN KEY ([provinceId]) REFERENCES [Provinces] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notices_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [UsersToDiscountCodes] (
    [id] int NOT NULL IDENTITY,
    [DiscountCodeId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_UsersToDiscountCodes] PRIMARY KEY ([id]),
    CONSTRAINT [FK_UsersToDiscountCodes_DiscountCodes_DiscountCodeId] FOREIGN KEY ([DiscountCodeId]) REFERENCES [DiscountCodes] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UsersToDiscountCodes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AdminsInProvices] (
    [id] int NOT NULL IDENTITY,
    [userId] int NULL,
    [adminsInCityId] int NULL,
    [provinceId] int NULL,
    CONSTRAINT [PK_AdminsInProvices] PRIMARY KEY ([id]),
    CONSTRAINT [FK_AdminsInProvices_AdminsInCities_adminsInCityId] FOREIGN KEY ([adminsInCityId]) REFERENCES [AdminsInCities] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AdminsInProvices_Provinces_provinceId] FOREIGN KEY ([provinceId]) REFERENCES [Provinces] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AdminsInProvices_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [BannerImage] (
    [Id] bigint NOT NULL IDENTITY,
    [CreateDate] datetime2 NOT NULL,
    [UpdateDate] datetime2 NULL,
    [FileLocation] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [BannerId] bigint NOT NULL,
    CONSTRAINT [PK_BannerImage] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BannerImage_Banner_BannerId] FOREIGN KEY ([BannerId]) REFERENCES [Banner] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AdminActivities] (
    [id] int NOT NULL IDENTITY,
    [userId] int NULL,
    [date] datetime2 NOT NULL,
    [activityType] int NOT NULL,
    [noticeId] bigint NULL,
    CONSTRAINT [PK_AdminActivities] PRIMARY KEY ([id]),
    CONSTRAINT [FK_AdminActivities_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AdminActivities_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Factors] (
    [id] int NOT NULL IDENTITY,
    [userId] int NOT NULL,
    [noticeId] bigint NULL,
    [bannerId] bigint NULL,
    [state] int NOT NULL,
    [factorKind] int NOT NULL,
    [createDatePersian] nvarchar(50) NOT NULL,
    [totalPrice] bigint NOT NULL,
    CONSTRAINT [PK_Factors] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Factors_Banner_bannerId] FOREIGN KEY ([bannerId]) REFERENCES [Banner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Factors_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Factors_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Messages] (
    [id] bigint NOT NULL IDENTITY,
    [text] nvarchar(max) NULL,
    [date] datetime2 NOT NULL,
    [rreceiverId] int NOT NULL,
    [ssenderId] int NOT NULL,
    [MessageType] int NOT NULL,
    [ItemId] int NOT NULL,
    [Noticeid] bigint NULL,
    [senderid] int NULL,
    [receiverid] int NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Messages_Notices_Noticeid] FOREIGN KEY ([Noticeid]) REFERENCES [Notices] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Messages_Users_receiverid] FOREIGN KEY ([receiverid]) REFERENCES [Users] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Messages_Users_senderid] FOREIGN KEY ([senderid]) REFERENCES [Users] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [NoticeImages] (
    [id] bigint NOT NULL IDENTITY,
    [image] nvarchar(500) NULL,
    [noticeId] bigint NOT NULL,
    CONSTRAINT [PK_NoticeImages] PRIMARY KEY ([id]),
    CONSTRAINT [FK_NoticeImages_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [ReportNotices] (
    [id] int NOT NULL IDENTITY,
    [userId] int NOT NULL,
    [noticeId] bigint NOT NULL,
    [message] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ReportNotices] PRIMARY KEY ([id]),
    CONSTRAINT [FK_ReportNotices_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ReportNotices_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [UserFavorites] (
    [id] int NOT NULL IDENTITY,
    [noticeId] bigint NOT NULL,
    [userId] int NOT NULL,
    CONSTRAINT [PK_UserFavorites] PRIMARY KEY ([id]),
    CONSTRAINT [FK_UserFavorites_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserFavorites_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [VisitNotices] (
    [id] int NOT NULL IDENTITY,
    [noticeId] bigint NOT NULL,
    [date] datetime2 NOT NULL,
    [countView] int NOT NULL,
    CONSTRAINT [PK_VisitNotices] PRIMARY KEY ([id]),
    CONSTRAINT [FK_VisitNotices_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE CASCADE
);

GO

CREATE TABLE [VisitNoticeUsers] (
    [id] int NOT NULL IDENTITY,
    [noticeId] bigint NOT NULL,
    [userId] int NOT NULL,
    CONSTRAINT [PK_VisitNoticeUsers] PRIMARY KEY ([id]),
    CONSTRAINT [FK_VisitNoticeUsers_Notices_noticeId] FOREIGN KEY ([noticeId]) REFERENCES [Notices] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_VisitNoticeUsers_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [FactorItems] (
    [id] int NOT NULL IDENTITY,
    [price] bigint NOT NULL,
    [ProductId] bigint NOT NULL,
    [FactorId] int NULL,
    CONSTRAINT [PK_FactorItems] PRIMARY KEY ([id]),
    CONSTRAINT [FK_FactorItems_Factors_FactorId] FOREIGN KEY ([FactorId]) REFERENCES [Factors] ([id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FactorItems_Notices_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Notices] ([id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_AdminActivities_noticeId] ON [AdminActivities] ([noticeId]);

GO

CREATE INDEX [IX_AdminActivities_userId] ON [AdminActivities] ([userId]);

GO

CREATE INDEX [IX_AdminsInCities_cityId] ON [AdminsInCities] ([cityId]);

GO

CREATE INDEX [IX_AdminsInCities_userid] ON [AdminsInCities] ([userid]);

GO

CREATE INDEX [IX_AdminsInProvices_adminsInCityId] ON [AdminsInProvices] ([adminsInCityId]);

GO

CREATE INDEX [IX_AdminsInProvices_provinceId] ON [AdminsInProvices] ([provinceId]);

GO

CREATE INDEX [IX_AdminsInProvices_userId] ON [AdminsInProvices] ([userId]);

GO

CREATE INDEX [IX_Areas_provinceId] ON [Areas] ([provinceId]);

GO

CREATE INDEX [IX_Banner_userId] ON [Banner] ([userId]);

GO

CREATE INDEX [IX_BannerImage_BannerId] ON [BannerImage] ([BannerId]);

GO

CREATE INDEX [IX_Categorys_parentCategoryId] ON [Categorys] ([parentCategoryId]);

GO

CREATE INDEX [IX_FactorItems_FactorId] ON [FactorItems] ([FactorId]);

GO

CREATE INDEX [IX_FactorItems_ProductId] ON [FactorItems] ([ProductId]);

GO

CREATE INDEX [IX_Factors_bannerId] ON [Factors] ([bannerId]);

GO

CREATE INDEX [IX_Factors_noticeId] ON [Factors] ([noticeId]);

GO

CREATE INDEX [IX_Factors_userId] ON [Factors] ([userId]);

GO

CREATE INDEX [IX_InformationMedias_InformationId] ON [InformationMedias] ([InformationId]);

GO

CREATE INDEX [IX_Messages_Noticeid] ON [Messages] ([Noticeid]);

GO

CREATE INDEX [IX_Messages_receiverid] ON [Messages] ([receiverid]);

GO

CREATE INDEX [IX_Messages_senderid] ON [Messages] ([senderid]);

GO

CREATE INDEX [IX_NoticeImages_noticeId] ON [NoticeImages] ([noticeId]);

GO

CREATE INDEX [IX_Notices_areaId] ON [Notices] ([areaId]);

GO

CREATE INDEX [IX_Notices_categoryId] ON [Notices] ([categoryId]);

GO

CREATE INDEX [IX_Notices_cityId] ON [Notices] ([cityId]);

GO

CREATE INDEX [IX_Notices_provinceId] ON [Notices] ([provinceId]);

GO

CREATE INDEX [IX_Notices_userId] ON [Notices] ([userId]);

GO

CREATE INDEX [IX_Provinces_cityId] ON [Provinces] ([cityId]);

GO

CREATE INDEX [IX_ReportNotices_noticeId] ON [ReportNotices] ([noticeId]);

GO

CREATE INDEX [IX_ReportNotices_userId] ON [ReportNotices] ([userId]);

GO

CREATE INDEX [IX_UserFavorites_noticeId] ON [UserFavorites] ([noticeId]);

GO

CREATE INDEX [IX_UserFavorites_userId] ON [UserFavorites] ([userId]);

GO

CREATE INDEX [IX_Users_provinceId] ON [Users] ([provinceId]);

GO

CREATE INDEX [IX_Users_roleId] ON [Users] ([roleId]);

GO

CREATE INDEX [IX_UsersToDiscountCodes_DiscountCodeId] ON [UsersToDiscountCodes] ([DiscountCodeId]);

GO

CREATE INDEX [IX_UsersToDiscountCodes_UserId] ON [UsersToDiscountCodes] ([UserId]);

GO

CREATE INDEX [IX_VisitNotices_noticeId] ON [VisitNotices] ([noticeId]);

GO

CREATE INDEX [IX_VisitNoticeUsers_noticeId] ON [VisitNoticeUsers] ([noticeId]);

GO

CREATE INDEX [IX_VisitNoticeUsers_userId] ON [VisitNoticeUsers] ([userId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230724173931_AfterMerged', N'2.1.14-servicing-32113');

GO

