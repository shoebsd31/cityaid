# CityAid API cURL Commands Generator
# This PowerShell script reads the api-test-requests.json file and generates cURL commands

param(
    [string]$ConfigFile = "api-test-requests.json",
    [string]$OutputFile = "curl-commands.sh",
    [switch]$Windows
)

Write-Host "🚀 CityAid API cURL Commands Generator" -ForegroundColor Green
Write-Host "📄 Reading config from: $ConfigFile" -ForegroundColor Yellow

if (-not (Test-Path $ConfigFile)) {
    Write-Host "❌ Configuration file not found: $ConfigFile" -ForegroundColor Red
    exit 1
}

try {
    $config = Get-Content $ConfigFile | ConvertFrom-Json

    Write-Host "🎯 Target API: $($config.apiBaseUrl)" -ForegroundColor Cyan
    Write-Host "🧪 Generating commands for $($config.requests.Count) requests" -ForegroundColor Cyan

    $commands = @()

    # Add header
    $commands += "#!/bin/bash"
    $commands += "# CityAid API Test Commands"
    $commands += "# Generated on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    $commands += ""
    $commands += "API_BASE_URL=`"$($config.apiBaseUrl)`""
    $commands += ""

    # Add token variables
    $commands += "# JWT Tokens"
    foreach ($tokenKey in $config.tokens.PSObject.Properties.Name) {
        $tokenValue = $config.tokens.$tokenKey
        $tokenVar = $tokenKey.ToUpper().Replace("_", "_TOKEN_")
        $commands += "$tokenVar=`"$tokenValue`""
    }
    $commands += ""

    # Add utility functions
    $commands += "# Utility functions"
    $commands += "check_response() {"
    $commands += "    if [ `$? -eq 0 ]; then"
    $commands += "        echo `"✅ Request successful`""
    $commands += "    else"
    $commands += "        echo `"❌ Request failed`""
    $commands += "    fi"
    $commands += "    echo `"`""
    $commands += "}"
    $commands += ""

    # Generate commands for each request
    $caseIdVars = @{}
    $requestIndex = 1

    foreach ($request in $config.requests) {
        $commands += "# $($requestIndex). $($request.name)"
        $commands += "# $($request.description)"

        # Get token variable name
        $tokenVar = $request.token.ToUpper().Replace("_", "_TOKEN_")

        # Build endpoint URL
        $endpoint = $request.endpoint
        if ($endpoint -match '\{caseId\}' -and $request.dependsOn) {
            $dependentVar = $request.dependsOn -replace '[^a-zA-Z0-9]', '_'
            $endpoint = $endpoint -replace '\{caseId\}', "`$$($dependentVar)_CASE_ID"
        }

        $url = "`$API_BASE_URL$endpoint"

        # Build cURL command
        $curlCmd = "curl -X $($request.method.ToUpper()) `"$url`""
        $curlCmd += " -H `"Authorization: Bearer `$$tokenVar`""
        $curlCmd += " -H `"Content-Type: application/json`""

        # Add payload for POST/PUT/PATCH
        if ($request.payload -and ($request.method -in @("POST", "PUT", "PATCH"))) {
            $payload = $request.payload | ConvertTo-Json -Compress -Depth 10
            $payload = $payload -replace '"', '\"'
            $curlCmd += " -d `"$payload`""
        }

        $curlCmd += " -w `"\\nHTTP Status: %{http_code}\\n`""
        $curlCmd += " -s"

        # Add response parsing for case creation
        if ($request.method -eq "POST" -and $request.endpoint -eq "/cases") {
            $caseVarName = $request.name -replace '[^a-zA-Z0-9]', '_'
            $caseIdVars[$request.name] = $caseVarName

            $commands += "response_$caseVarName=`$($curlCmd)"
            $commands += "echo `"Response: `$response_$caseVarName`""
            $commands += "$($caseVarName)_CASE_ID=`$(echo `"`$response_$caseVarName`" | grep -o '`"id`":`"[^`"]*`"' | cut -d'`"' -f4)"
            $commands += "echo `"Created Case ID: `$$($caseVarName)_CASE_ID`""
            $commands += "check_response"
        } else {
            $commands += $curlCmd
            $commands += "check_response"
        }

        $commands += ""
        $requestIndex++
    }

    # Add summary
    $commands += "echo `"🎉 All API tests completed!`""
    $commands += "echo `"📋 Created Case IDs:`""
    foreach ($caseVar in $caseIdVars.Values) {
        $commands += "echo `"  `$$($caseVar)_CASE_ID`""
    }

    # Write to file
    $commands | Out-File -FilePath $OutputFile -Encoding UTF8

    # Make executable on Unix-like systems
    if (-not $Windows -and (Get-Command "chmod" -ErrorAction SilentlyContinue)) {
        chmod +x $OutputFile
    }

    Write-Host "✅ cURL commands generated successfully!" -ForegroundColor Green
    Write-Host "📁 Output file: $OutputFile" -ForegroundColor Yellow
    Write-Host "🚀 Run with: ./$OutputFile (Linux/Mac) or bash $OutputFile (Windows Git Bash)" -ForegroundColor Cyan

} catch {
    Write-Host "❌ Error generating cURL commands: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}