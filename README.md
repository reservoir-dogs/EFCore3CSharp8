# EFCore3CSharp8

I migrate a project from ef core 2.2.6 to 3.0.1.

I corrected many issues and deliver to in production, but performances are downgraded.
The I/O are become more important than before.

I have an entity like that :

<!-- language: c# -->

    public class Foo
    {
        public long Id { get; set; }

        public string Required { get; set; }

        public string? Nullable { get; set; }
    }


When I execute, the following queries :

<!-- language: c# -->

    var message = applicationDbContext.Foos.Where(s => s.Id == 123).FirstOrDefault();
    message = applicationDbContext.Foos.Where(s => s.Required == "123").FirstOrDefault();
    message = applicationDbContext.Foos.Where(s => s.Nullable == "123").FirstOrDefault();
    
    long id = 123;
    string code = "123";
    message = applicationDbContext.Foos.Where(s => s.Id == id).FirstOrDefault();
    message = applicationDbContext.Foos.Where(s => s.Required == code).FirstOrDefault();
    message = applicationDbContext.Foos.Where(s => s.Nullable == code).FirstOrDefault();

In 2.2.6, I had this log

<!-- language: sql -->

    Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (56ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [s].[Id], [s].[Nullable], [s].[Required]
    FROM [Foos] AS [s]
    WHERE [s].[Id] = CAST(123 AS bigint)
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (10ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [s].[Id], [s].[Nullable], [s].[Required]
    FROM [Foos] AS [s]
    WHERE [s].[Required] = N'123'
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (9ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [s].[Id], [s].[Nullable], [s].[Required]
    FROM [Foos] AS [s]
    WHERE [s].[Nullable] = N'123'
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (29ms) [Parameters=[@__id_0='123'], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [s].[Id], [s].[Nullable], [s].[Required]
    FROM [Foos] AS [s]
    WHERE [s].[Id] = @__id_0
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (9ms) [Parameters=[@__code_0='123' (Size = 4000)], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [s].[Id], [s].[Nullable], [s].[Required]
    FROM [Foos] AS [s]
    WHERE [s].[Required] = @__code_0
    The thread 0x2c14 has exited with code 0 (0x0).
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executed DbCommand (10ms) [Parameters=[@__code_0='123' (Size = 4000)], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [s].[Id], [s].[Nullable], [s].[Required]
    FROM [Foos] AS [s]
    WHERE [s].[Nullable] = @__code_0

Now, I have this :

<!-- language: sql -->

    Microsoft.EntityFrameworkCore.Database.Command: Information: Executing DbCommand [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [f].[Id], [f].[Nullable], [f].[Required]
    FROM [Foos] AS [f]
    WHERE [f].[Id] = CAST(123 AS bigint)
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executing DbCommand [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [f].[Id], [f].[Nullable], [f].[Required]
    FROM [Foos] AS [f]
    WHERE [f].[Required] = N'123'
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executing DbCommand [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [f].[Id], [f].[Nullable], [f].[Required]
    FROM [Foos] AS [f]
    WHERE ([f].[Nullable] = N'123') AND [f].[Nullable] IS NOT NULL
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executing DbCommand [Parameters=[@__id_0='123'], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [f].[Id], [f].[Nullable], [f].[Required]
    FROM [Foos] AS [f]
    WHERE ([f].[Id] = @__id_0) AND @__id_0 IS NOT NULL
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executing DbCommand [Parameters=[@__code_0='123' (Size = 4000)], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [f].[Id], [f].[Nullable], [f].[Required]
    FROM [Foos] AS [f]
    WHERE ([f].[Required] = @__code_0) AND @__code_0 IS NOT NULL
    Microsoft.EntityFrameworkCore.Database.Command: Information: Executing DbCommand [Parameters=[@__code_0='123' (Size = 4000)], CommandType='Text', CommandTimeout='30']
    SELECT TOP(1) [f].[Id], [f].[Nullable], [f].[Required]
    FROM [Foos] AS [f]
    WHERE (([f].[Nullable] = @__code_0) AND ([f].[Nullable] IS NOT NULL AND @__code_0 IS NOT NULL)) OR ([f].[Nullable] IS NULL AND @__code_0 IS NULL)

EF do a bad interpretation any variables in lambdas expressions about type of string not nullable and add useless parameters which modify the execution plan.

Why EF Core 3.0 in a C# 8.0 context, do it check the nullability of parameters which can't to be ?