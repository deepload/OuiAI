IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Identity')
BEGIN
    CREATE DATABASE OuiAI_Identity;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Projects')
BEGIN
    CREATE DATABASE OuiAI_Projects;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Social')
BEGIN
    CREATE DATABASE OuiAI_Social;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Notifications')
BEGIN
    CREATE DATABASE OuiAI_Notifications;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OuiAI_Search')
BEGIN
    CREATE DATABASE OuiAI_Search;
END
