USE [SchoolMVC9];
GO
 
-- ============================================
-- 1. ROLES
-- ============================================
CREATE TABLE [dbo].[Roles] (
    [Id]        INT             IDENTITY(1, 1) NOT NULL,
    [Name]      NVARCHAR(50)    NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Roles_Name] UNIQUE ([Name])
);
GO
 
CREATE NONCLUSTERED INDEX [IX_Roles_Name]
    ON [dbo].[Roles]([Name] ASC);
GO
 
-- ============================================
-- 2. CAPABILITIES
-- ============================================
CREATE TABLE [dbo].[Capabilities] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [Name]          NVARCHAR(100)   NOT NULL,
    [Description]   NVARCHAR(255)   NULL,
    CONSTRAINT [PK_Capabilities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Capabilities_Name] UNIQUE ([Name])
);
GO
 
CREATE NONCLUSTERED INDEX [IX_Capabilities_Name]
    ON [dbo].[Capabilities]([Name] ASC);
GO
 
-- ============================================
-- 3. ROLES_CAPABILITIES (Many-to-Many)
-- ============================================
CREATE TABLE [dbo].[RolesCapabilities] (
    [RoleId]        INT NOT NULL,
    [CapabilityId]  INT NOT NULL,
    CONSTRAINT [PK_RolesCapabilities] PRIMARY KEY CLUSTERED ([RoleId], [CapabilityId]),
 
    CONSTRAINT [FK_RolesCapabilities_Roles]
        FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id])
        ON DELETE CASCADE,
 
    CONSTRAINT [FK_RolesCapabilities_Capabilities]
        FOREIGN KEY ([CapabilityId]) REFERENCES [dbo].[Capabilities]([Id])
        ON DELETE CASCADE
);
GO
 
CREATE NONCLUSTERED INDEX [IX_RolesCapabilities_CapabilityId]
    ON [dbo].[RolesCapabilities]([CapabilityId] ASC);
GO
 
-- ============================================
-- 4. USERS
-- ============================================
CREATE TABLE [dbo].[Users] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [Username]      NVARCHAR(50)    NOT NULL,
    [Email]         NVARCHAR(50)    NOT NULL,
    [Password]      NVARCHAR(60)    NOT NULL,
    [RoleId]        INT             NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
 
    CONSTRAINT [FK_Users_Roles]
        FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id])
        ON DELETE NO ACTION
);
GO
 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username]
    ON [dbo].[Users]([Username] ASC);
GO
 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email]
    ON [dbo].[Users]([Email] ASC);
GO
 
CREATE NONCLUSTERED INDEX [IX_Users_RoleId]
    ON [dbo].[Users]([RoleId] ASC);
GO
 
-- ============================================
-- 5. TEACHERS
-- ============================================
CREATE TABLE [dbo].[Companies] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [UserId]        INT             NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([Id] ASC),
 
    CONSTRAINT [FK_Companies_Users]
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);
GO
 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Companies_UserId]
    ON [dbo].[Companies]([UserId] ASC);
GO
 
-- ============================================
-- 6. STUDENTS
-- ============================================
CREATE TABLE [dbo].[Gamers] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [UserId]        INT             NOT NULL,
    CONSTRAINT [PK_Gamers] PRIMARY KEY CLUSTERED ([Id] ASC),
 
    CONSTRAINT [FK_Gamers_Users]
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);
GO
 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Gamers_UserId]
    ON [dbo].[Gamers]([UserId] ASC);
GO
 
-- ============================================
-- 7. COURSES
-- ============================================
CREATE TABLE [dbo].[Games] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [Description]   NVARCHAR(50)    NOT NULL,
    [CompanyId]     INT             NULL,
    CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED ([Id] ASC),
 
    CONSTRAINT [FK_Games_Companies]
        FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies]([Id])
);
GO
 
CREATE NONCLUSTERED INDEX [IX_Games_Description]
    ON [dbo].[Games]([Description] ASC);
GO
 
CREATE NONCLUSTERED INDEX [IX_Games_CompanyId]
    ON [dbo].[Games]([CompanyId] ASC);
GO
 
-- ============================================
-- 8. COURSES_STUDENTS (Many-to-Many)
-- ============================================
CREATE TABLE [dbo].[GamesGamers] (
    [GameId]      INT NOT NULL,
    [GamerId]     INT NOT NULL,
    CONSTRAINT [PK_GamesGamers] PRIMARY KEY CLUSTERED ([GameId], [GamerId]),
 
    CONSTRAINT [FK_GamesGamers_Games]
        FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games]([Id]),
 
    CONSTRAINT [FK_GamesGamers_Gamers]
        FOREIGN KEY ([GamerId]) REFERENCES [dbo].[Gamers]([Id])
);
GO
 
CREATE INDEX [IX_GamesGamers_GameId]
    ON [dbo].[GamesGamers]([GameId]);
GO
 
CREATE INDEX [IX_GamesGamers_GamerId]
    ON [dbo].[GamesGamers]([GamerId]);
GO