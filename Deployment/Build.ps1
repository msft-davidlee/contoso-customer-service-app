param(
    [string]$LOCATION,
    [string]$BUILD_ENV,
    [string]$RESOURCE_GROUP,
    [string]$GITHUB_REF,
    [string]$PREFIX)

$ErrorActionPreference = "Stop"

$deploymentName = "deploy" + (Get-Date).ToString("yyyyMMddHHmmss")

Write-Host "Attempting to run deployment $deploymentName"

$rgName = "$RESOURCE_GROUP-$BUILD_ENV"
$deployOutputText = (az deployment group create --name $deploymentName --resource-group $rgName --template-file Deployment/deploy.bicep --parameters `
        location=$LOCATION `
        prefix=$PREFIX `
        appEnvironment=$BUILD_ENV `
        branch=$GITHUB_REF)

$deployOutput = $deployOutputText | ConvertFrom-Json
$acrName = $deployOutput.properties.outputs.acrName.value

# Login to ACR
az acr login --name $acrName
if ($LastExitCode -ne 0) {
    throw "An error has occured. Unable to login to acr."
}

$list = az acr repository list --name $acrName | ConvertFrom-Json
if ($LastExitCode -ne 0) {
    throw "An error has occured. Unable to list from repository"
}

# Build your app with ACR build command

$apps = @(
    @{
        name = "demowebsite";
        path = "DemoWebsite";
    },
    @{
        name = "demo-alternate-id-service";
        path = "DemoCustomerServiceAltId";
    },
    @{
        name = "demo-partner-api";
        path = "DemoPartnerAPI";
    },
    @{
        name = "demo-service-bus-shipping-func";
        path = "DemoServiceBusShippingFunc";
    },
    @{
        name = "demo-storage-queue-func";
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
        az acr build --image $imageName -r $acrName --file ./$path/Dockerfile .
    
        if ($LastExitCode -ne 0) {
            throw "An error has occured. Unable to build image."
        }
    }
}