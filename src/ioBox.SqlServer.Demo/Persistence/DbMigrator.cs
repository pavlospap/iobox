using Dapper;

namespace IOBox.SqlServer.Demo.Persistence;

internal class DbMigrator(IDbContext dbContext) : IDbMigrator
{
    public void MigrateDb()
    {
        using var connection = dbContext.CreateConnection();

        var sql = @"
            IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'App')
            BEGIN
                EXEC('CREATE SCHEMA App');
            END

            IF NOT EXISTS (
                SELECT * FROM sys.objects 
                WHERE object_id = OBJECT_ID(N'App.Members')
            )
            BEGIN
                CREATE TABLE App.Members (
                    Id int IDENTITY NOT NULL,
                    Email NVARCHAR(50) NOT NULL,
                    HasBeenWelcomed tinyint NOT NULL,
                    HasBeenGivenBonus tinyint NOT NULL,
                    CONSTRAINT PK_Members PRIMARY KEY (Id)
                );

                CREATE UNIQUE INDEX UX_Members_Email 
                    ON App.Members(Email);
            END";

        connection.Execute(sql);
    }
}
