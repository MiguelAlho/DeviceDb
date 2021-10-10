using FluentMigrator;

namespace DeviceDb.Api.Adaptors.Sql.Migrations;

[Migration(2021_10_10_2337)]
public class AddDeviceTable : Migration
{
    public override void Up()
        => Create.Table("Device")
            .WithColumn("Id").AsGuid().PrimaryKey().Unique().NotNullable()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("BrandId").AsString(100).NotNullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable();
    public override void Down()
        => Delete.Table("Device");
}