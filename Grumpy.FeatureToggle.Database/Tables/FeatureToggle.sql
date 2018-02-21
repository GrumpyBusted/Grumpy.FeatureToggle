CREATE TABLE [dbo].[FeatureToggle]
(
    [Id] INT NOT NULL IDENTITY, 
    [ServiceName] NVARCHAR(256) NOT NULL, 
    [UserName] NVARCHAR(20) NOT NULL, 
    [MachineName] NVARCHAR(15) NOT NULL, 
    [Feature] NVARCHAR(256) NOT NULL, 
    [Priority] INT NOT NULL, 
    [Enabled] BIT NOT NULL, 
    CONSTRAINT [PK_FeatureToggle] PRIMARY KEY ([Id])
)

GO

CREATE UNIQUE INDEX [IX_FeatureToggle_Feature] ON [dbo].[FeatureToggle] ([Feature], [ServiceName], [UserName], [MachineName], [Priority])
     
