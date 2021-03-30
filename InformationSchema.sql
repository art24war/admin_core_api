USE [DATABASENAME];
 
-- list of all tables in the selected database
select * from INFORMATION_SCHEMA.TABLES;
    
-- list of all constraints in the selected database
select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS;

-- list of columns from all tables
select * from INFORMATION_SCHEMA.COLUMNS;

--list of all keys from tables
select * from INFORMATION_SCHEMA.KEY_COLUMN_USAGE;
