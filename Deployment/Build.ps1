param([string]$AcrName)

$ErrorActionPreference = "Stop"

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
}