IF OBJECT_ID(N'EventTypes', N'U') IS NULL
BEGIN
    CREATE TABLE EventTypes (
        EventTypeId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL
    );

    INSERT INTO EventTypes (Name)
    VALUES 
    ('Wedding'),
    ('Conference'),
    ('Birthday'),
    ('Concert'),
    ('Corporate'),
    ('Workshop');
END

IF COL_LENGTH('Events', 'EventTypeId') IS NULL
BEGIN
    ALTER TABLE Events
    ADD EventTypeId INT NOT NULL DEFAULT 1;
END