# TASKI API

## SQL SERVER

```powershell

  $sa_password = "[SA PASSWORD HERE]"

    docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -e "MSSQL_PID=Evaluation" -p 1433:1433 -v sqlvolume:/var/opt/mssql --name sqlserver --hostname sqlpreview -d --rm  mcr.microsoft.com/mssql/server:2022-preview-ubuntu-22.04
```

## ustawianie connection string dla secret managera

```powershell
$sa_password = "[SA PASSWORD HERE]"
dotnet user-secrets set "ConnectionStrings:TaskiContext" "Server=localhost; Database=Taski; User Id=sa; Password=$sa_password;TrustServerCertificate=True"
```

## Update Database

dotnet ef database update
