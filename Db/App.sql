IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'app')
BEGIN
    CREATE DATABASE [app]
END