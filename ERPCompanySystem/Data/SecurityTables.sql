CREATE TABLE LoginAttempts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IpAddress NVARCHAR(45) NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    AttemptTime DATETIME NOT NULL DEFAULT GETDATE(),
    Success BIT NOT NULL DEFAULT 0
)

CREATE TABLE IpBlocks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IpAddress NVARCHAR(45) NOT NULL,
    BlockedUntil DATETIME NOT NULL,
    Reason NVARCHAR(200) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
)

-- Create indexes for better performance
CREATE INDEX IX_LoginAttempts_IpAddress ON LoginAttempts(IpAddress)
CREATE INDEX IX_LoginAttempts_AttemptTime ON LoginAttempts(AttemptTime)
CREATE INDEX IX_IpBlocks_IpAddress ON IpBlocks(IpAddress)
CREATE INDEX IX_IpBlocks_BlockedUntil ON IpBlocks(BlockedUntil)
