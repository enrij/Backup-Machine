# Add migration

```shell
cls~~~~
$migration = Read-Host -Prompt 'New migration name'
dotnet ef migrations add $migration --project .\src\BackupMachine.PoC --output-dir Infrastructure\Persistence\Migrations
dotnet ef database update --project .\src\BackupMachine.PoC

```

# Remove last migration

```shell
cls
dotnet ef migrations remove --project .\src\BackupMachine.PoC

```

# Update DB

```shell
cls
dotnet ef database update --project .\src\BackupMachine.PoC

```

# Drop DB

```shell
cls
dotnet ef database drop --project .\src\BackupMachine.PoC --force

```

# Redo last migration

```shell
cls
$migration = Read-Host -Prompt 'New migration name'
dotnet ef database drop --project .\src\BackupMachine.PoC --force
dotnet ef migrations remove --project .\src\BackupMachine.PoC
dotnet ef migrations add $migration --project .\src\BackupMachine.PoC --output-dir Infrastructure\Persistence\Migrations
dotnet ef database update --project .\src\BackupMachine.PoC

```

# Drop & crate DB

```shell
cls
dotnet ef database drop --project .\src\BackupMachine.PoC --force
dotnet ef database update --project .\src\BackupMachine.PoC

```
