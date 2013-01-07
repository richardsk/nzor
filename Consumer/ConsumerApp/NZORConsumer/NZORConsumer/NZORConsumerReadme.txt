Installation of example NZOR Consumer Client Application.
----------------------------------------------------------------------------------
Requirements:
Windows OS with .NET framework 4.0
MySQL 5.5

1.  Downaload and install MySQL 5.5, from http://dev.mysql.com/downloads/.  Make sure you install the .NET connector.
2.  Start MySQL Workbench (or other MySQL management tool).
3.  Run the SQL script file db-create.txt on the Consumer database/schema.  This file is located in the program files folder for the example application.
4.  Add an account with login name: nzor, host: localhost and password: nzor
5.  Add Schema Privileges for the nzor user to have all rights to the consumer database/schema.
5.  Run the NZOR Consumer application from the Start menu.


ODBC Connection.
------------------------------------------------------------------------------------
Depending on the download and installation file that was chosen, the MySQL installation is likely to have an ODBC connector installed as part of the package.

To configure an ODBC connection to the NZOR MySQL Consumer database, do the following:
1.  Go to Start > Control Panel > Data Sources (ODBC).
2.  Create a new System Data Source.
3.  Select the My SQL Driver.
4.  Set the relevant settings and close the popup.
5.  You can now connect to this ODBC data source from applications such as Microsoft Access.
