# Encryption in Microsoft.Data.Sqlite
One of the frequently asked questions about [Microsoft.Data.Sqlite](https://github.com/aspnet/Microsoft.Data.Sqlite) is: How do I encrypt a database? I think that one of the main reasons for this is because [System.Data.SQLite](http://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki) comes with an unsupported, Windows-only encryption codec that can be used by specifying `Password` (or `HexPassword`) in the connection string. The official releases of SQLite, however, don't come with encryption.

[SEE](http://www.hwaci.com/sw/sqlite/see.html), [SQLCipher](https://www.zetetic.net/sqlcipher/), [SQLiteCrypt](http://sqlite-crypt.com/index.htm) & [wxSQLite3](https://github.com/utelle/wxsqlite3) are just some of the solutions I've found that can encrypt SQLite database files. They all seem to follow the same general pattern, so this post applies to all of them.

## Bring Your Own Library
The first step is to add a version of the native `sqlite3` library to you application that has encryption. `Microsoft.Data.Sqlite` depends on [SQLitePCL.raw](https://github.com/ericsink/SQLitePCL.raw) which makes it very easy to use SQLCipher.

```
Install-Package Microsoft.Data.Sqlite.Core
Install-Package SQLitePCLRaw.bundle_sqlcipher
```
SQLitePCL.raw also enables you to bring your own build of SQLite, but we won't cover that in this post.

## Specify the Key
To enable encryption, Specify the key immediately after opening the connection. Do this by issuing a `PRAGMA key` statement. It may be specified as either a string or BLOB literal. SQLite, unfortunately, doesn't support parameters in `PRAGMA` statements.

``` C#
connection.Open();
using (var command = connection.CreateCommand())
{
    command.Parameters.Clear();
    command.CommandText = $"PRAGMA key = '{Password}';";
    command.ExecuteNonQuery();

    return connection;
}

// Interact with the database here
```

## Rekey the Database
If you want to change the encryption key of a database, issue a `PRAGMA rekey` statement. To decrypt the database, specify `NULL`, ex: `PRAGMA rekey='';`.

``` C#
connection.Open();
using (var command = connection.CreateCommand())
{
    command.Parameters.Clear();
    command.CommandText = $"PRAGMA key='{Password}';";
    command.ExecuteNonQuery();

    command.CommandText = $"PRAGMA rekey='{NewPassword}';";
    command.ExecuteNonQuery();
}
```