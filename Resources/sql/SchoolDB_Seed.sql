INSERT INTO [dbo].[Roles] ([Name])
VALUES
    ('ADMIN'),
    ('EMPLOYEE'),
    ('COMPANY'),
    ('GAMER');

INSERT INTO [dbo].[Capabilities] ([Name], [Description])
VALUES
    ('INSERT_COMPANY', 'Create a new company'),
    ('VIEW_COMPANIES', 'View company list and details'),
    ('VIEW_COMPANY', 'View company'),
    ('EDIT_COMPANY', 'Modify existing company'),
    ('DELETE_COMPANY', 'Remove a company'),
    ('VIEW_ONLY_COMPANY', 'View only own company details'),
    ('INSERT_GAMER', 'Create a new gamer'),
    ('VIEW_GAMERS', 'View gamer list and details'),
    ('VIEW_GAMER', 'View gamer'),
    ('EDIT_GAMER', 'Modify existing gamer'),
    ('DELETE_GAMER', 'Remove a gamer'),
    ('VIEW_ONLY_GAMER', 'View only own gamer details'),
    ('INSERT_GAME', 'Create a new game'),
    ('VIEW_GAMES', 'View game list and details'),
    ('VIEW_GAME', 'View game'),
    ('EDIT_GAME', 'Modify existing game'),
    ('DELETE_GAME', 'Remove a game');

INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'ADMIN';


INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'EMPLOYEE'
  AND c.[Name] IN ('VIEW_COMPANIES', 'VIEW_COMPANY',
                    'VIEW_GAMERS', 'VIEW_GAMER',
                    'VIEW_GAMES', 'VIEW_GAME');


INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'COMPANY'
  AND c.[Name] IN ('VIEW_ONLY_COMPANY');


INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'GAMER'
  AND c.[Name] IN ('VIEW_ONLY_GAMER');

