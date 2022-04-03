# Add migration

```shell
$migration = Read-Host -Prompt 'New migration name'
dotnet ef migrations add $migration --project .\src\BackupMachine.PoC --output-dir Infrastructure\Persistence\Migrations
```

# Remove last migration

```shell
dotnet ef migrations remove --project .\src\BackupMachine.PoC
```

# Update DB

```shell
dotnet ef database update --project .\src\BackupMachine.PoC
```

# Drop DB

```shell
dotnet ef database drop --project .\src\BackupMachine.PoC --force
```

# Redo last migration

```shell
$migration = Read-Host -Prompt 'New migration name'
dotnet ef database drop --project .\src\BackupMachine.PoC --force
dotnet ef migrations remove --project .\src\BackupMachine.PoC
dotnet ef migrations add $migration --project .\src\BackupMachine.PoC --output-dir Infrastructure\Persistence\Migrations
dotnet ef database update --project .\src\BackupMachine.PoC

```

# Drop & crate DB

```shell
dotnet ef database drop --project .\src\BackupMachine.PoC --force
dotnet ef database update --project .\src\BackupMachine.PoC

```
