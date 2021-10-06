param([string]$AcrName, [string]$AccountName, [string]$ContainerName, [string]$ResourceGroup)

$ErrorActionPreference = "Stop"

# Generate SAS upfront
$AccountKey = (az storage account keys list -g sharedservices-dev -n $AccountName | ConvertFrom-Json)[0].value
$end = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
$start = (Get-Date).ToString("yyyy-MM-dd")
$sas = (az storage container generate-sas -n $ContainerName --account-name $AccountName --account-key $AccountKey --permissions racwl --expiry $end --start $start --https-only | ConvertFrom-Json)
if (!$sas -or $LastExitCode -ne 0) {
    throw "An error has occured. Unable to generate sas."
}

Write-Host "SAS: $sas"

# Login to ACR
az acr login --name $AcrName
if ($LastExitCode -ne 0) {
    throw "An error has occured. Unable to login to acr."
}

$list = az acr repository list --name $AcrName | ConvertFrom-Json
if ($LastExitCode -ne 0) {
    throw "An error has occured. Unable to list from repository"
}

# Build your app with ACR build command

$namePrefix = "contoso-demo"
$apps = @(
    @{
        name = "$namePrefix-website";
        path = "DemoWebsite";
    },
    @{
        name = "$namePrefix-alternate-id-service";
        path = "DemoCustomerServiceAltId";
    },
    @{
        name = "$namePrefix-partner-api";
        path = "DemoPartnerAPI";
    },
    @{
        name = "$namePrefix-service-bus-shipping-func";
        path = "DemoServiceBusShippingFunc";
    },
    @{
        name = "$namePrefix-storage-queue-func";
        path = "DemoStorageShippingFunc";
    }
)

$version = "v1"
for ($i = 0; $i -lt $apps.Length; $i++) {
    $app = $apps[$i]

    $appName = $app.name
    $path = $app.path

    Write-Host "App: $appName"

    $imageName = "$appName`:$version"
    if (!$list -or !$list.Contains($imageName)) {
        az acr build --image $imageName -r $AcrName --file ./$path/Dockerfile .
    
        if ($LastExitCode -ne 0) {
            throw "An error has occured. Unable to build image."
        }
    }

    Push-Location $path

    $appFileName = "$appName$version.zip"
    dotnet publish -c Release -o out
    Compress-Archive out\* -DestinationPath $appFileName -Force

    $url = "https://$AccountName.blob.core.windows.net/$ContainerName/$appFileName?$sas"
    Write-Host "url: $url"
    azcopy_v10 copy $appFileName $url --overwrite=false

    if ($LastExitCode -ne 0) {
        throw "An error has occured. Unable to deploy zip."
    }

    Pop-Location
}