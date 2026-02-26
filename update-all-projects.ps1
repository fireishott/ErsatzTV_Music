$files = Get-ChildItem -Recurse -Filter "*.csproj"

foreach ($file in $files) {
    Write-Host "Processing $($file.Name)" -ForegroundColor Cyan
    $content = Get-Content $file.FullName -Raw
    
    # Replace any net8.0 with net10.0
    if ($content -match "<TargetFramework>net8.0</TargetFramework>") {
        $newContent = $content -replace '<TargetFramework>net8.0</TargetFramework>', '<TargetFramework>net10.0</TargetFramework>'
        $newContent | Out-File $file.FullName -Encoding UTF8 -Force
        Write-Host "  Updated $($file.Name) to net10.0" -ForegroundColor Green
    }
    
    # Also check for net9.0 and update to net10.0 if needed
    if ($content -match "<TargetFramework>net9.0</TargetFramework>") {
        $newContent = $content -replace '<TargetFramework>net9.0</TargetFramework>', '<TargetFramework>net10.0</TargetFramework>'
        $newContent | Out-File $file.FullName -Encoding UTF8 -Force
        Write-Host "  Updated $($file.Name) from net9.0 to net10.0" -ForegroundColor Green
    }
}

Write-Host "All project files updated to .NET 10.0" -ForegroundColor Green
