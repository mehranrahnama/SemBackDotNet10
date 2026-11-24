using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SemBackDotNet10.actionDomain.controller;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true).AddJsonFile("appsettings.Development.json", true)
    .AddJsonFile("appsettings.Development.Local.json", true).AddEnvironmentVariables().Build();
var connection = new SqlConnectionStringBuilder(config.GetConnectionString("DefaultConnection")) { ConnectTimeout = 5, MinPoolSize = 0 };
if (!string.IsNullOrWhiteSpace(config["Database:Username"]) && !string.IsNullOrWhiteSpace(config["Database:Password"]))
{
    connection.UserID = config["Database:Username"];
    connection.Password = config["Database:Password"];
    connection.IntegratedSecurity = false;
}
using var db = new SemDbContext(new DbContextOptionsBuilder<SemDbContext>().UseSqlServer(connection.ConnectionString).Options);
var sample = await db.Database.SqlQuery<Sample>($"""
    SELECT TOP(1) CAST(c.actionplancatalogid AS bigint) AS Id, CAST(l.tenantid AS bigint) AS TenantId
    FROM dbo.tblactionplancatalog c JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
    WHERE l.tenantid > 0 ORDER BY c.actionplancatalogid
    """).FirstOrDefaultAsync() ?? throw new Exception("No catalog sample found.");
var runtime = new ServiceRuntime(db, null!, null!, new TestUser(true), TimeProvider.System, config);
var levels = new PlaningLevelUserServiceImpl(runtime);
var tenant = new TenantContext();
tenant.SetTenant(sample.TenantId);
var controller = new ActionCatalogDatesController(runtime, levels, tenant);
if (await controller.GetCatalog(sample.Id, default) is not OkObjectResult ||
    await controller.GetDates(sample.Id, default) is not OkObjectResult ||
    await controller.GetDates(0, default) is not BadRequestObjectResult)
    throw new Exception("Catalog response validation failed.");
var otherTenant = new TenantContext();
otherTenant.SetTenant(sample.TenantId + 100000);
var otherController = new ActionCatalogDatesController(runtime, levels, otherTenant);
if (await otherController.GetCatalog(sample.Id, default) is not NotFoundResult ||
    await otherController.GetDates(sample.Id, default) is not NotFoundResult)
    throw new Exception("Tenant isolation validation failed.");
var unprivilegedRuntime = new ServiceRuntime(db, null!, null!, new TestUser(false), TimeProvider.System, config);
var unprivileged = new ActionCatalogDatesController(unprivilegedRuntime, new PlaningLevelUserServiceImpl(unprivilegedRuntime), tenant);
try
{
    await unprivileged.GetDates(sample.Id, default);
    throw new Exception("Unauthorized user was not rejected.");
}
catch (SemBackDotNet10.Common.exception.UnAuthorizedException) { }
Console.WriteLine("Real catalog and date queries, invalid ID, tenant isolation, and level authorization passed.");

public sealed class Sample { public long Id { get; set; } public long TenantId { get; set; } }
public sealed class TestUser(bool isAdmin) : ICurrentUser { public long Id => -1; public bool IsAdmin => isAdmin; }
