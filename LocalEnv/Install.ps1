param([Parameter(Mandatory = $true)]$Password)

docker pull mcr.microsoft.com/mssql/server:2019-latest
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$Password" `
	-p 1433:1433 --name sqldev -h sqldev `
	-d mcr.microsoft.com/mssql/server:2019-latest

docker pull mcr.microsoft.com/azure-storage/azurite
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 `
	--name strdev -h strdev `
	-d mcr.microsoft.com/azure-storage/azurite

Start-Sleep -s 10
sqlcmd -S localhost -U sa -P $Password -i ..\Db\App.sql
sqlcmd -S localhost -U sa -P $Password -i ..\Db\Migrations.sql