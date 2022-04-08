# Add migration

```shell
cls
$migration = Read-Host -Prompt 'New migration name'
dotnet ef migrations add $migration --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console --output-dir Infrastructure\Persistence\Migrations
dotnet ef database update --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console

```

# Remove last migration

```shell
cls
dotnet ef migrations remove --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console

```

# Update DB

```shell
cls
dotnet ef database update --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console

```

# Drop DB

```shell
cls
dotnet ef database drop --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console --force

```

# Redo last migration

```shell
cls
$migration = Read-Host -Prompt 'New migration name'
dotnet ef database drop --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console --force
dotnet ef migrations remove --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console
dotnet ef migrations add $migration --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console --output-dir Infrastructure\Persistence\Migrations
dotnet ef database update --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console

```

# Drop & crate DB

```shell
cls
dotnet ef database drop --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console --force
dotnet ef database update --project .\src\BackupMachine.Infrastructure --startup-project .\src\BackupMachine.Console

```
