param(
    [string]$BUILD_ENV,
    [Parameter(Mandatory = $true)][string]$APP_VERSION)

$str = (az resource list --tag ard-resource-id=shared-storage | ConvertFrom-Json)
$AccountName = $str.Name
$ContainerName = "apps"

# Generate SAS upfront
$AccountKey = (az storage account keys list -g $str.ResourceGroup -n $AccountName | ConvertFrom-Json)[0].value
$end = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
$start = (Get-Date).ToString("yyyy-MM-dd")
$sas = (az storage container generate-sas -n $ContainerName --account-name $AccountName --account-key $AccountKey --permissions racwl --expiry $end --start $start --https-only | ConvertFrom-Json)
if (!$sas -or $LastExitCode -ne 0) {
    throw "An error has occured. Unable to generate sas."
}

Push-Location Db
if ($BUILD_ENV -eq 'dev') {
    $dbFileName = "Migrations-$APP_VERSION-dev.sql"
    $dacpac = "cch-$APP_VERSION-dev.dacpac"
}
else {
    $dbFileName = "Migrations-$APP_VERSION.sql"
    $dacpac = "cch-$APP_VERSION.dacpac"
}

$url = "https://$AccountName.blob.core.windows.net/$ContainerName/" + $dbFileName + "?$sas"    
azcopy_v10 copy "Migrations.sql" $url --overwrite=false

$url = "https://$AccountName.blob.core.windows.net/$ContainerName/" + $dacpac + "?$sas"
azcopy_v10 copy "cch.dacpac" $url --overwrite=false
Pop-Location