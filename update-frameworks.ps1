$files = Get-ChildItem -Recurse -Filter "*.csproj"

foreach ($file in $files) {
    Write-Host "Processing $($file.FullName)" -ForegroundColor Yellow
    $content = Get-Content $file.FullName -Raw
    
    # Replace net10.0 with net8.0
    $newContent = $content -replace '<TargetFramework>net10.0</TargetFramework>', '<TargetFramework>net8.0</TargetFramework>'
    $newContent = $newContent -replace '<TargetFrameworks>net10.0</TargetFrameworks>', '<TargetFrameworks>net8.0</TargetFrameworks>'
    
    # Also update any preview references
    $newContent = $newContent -replace 'net10.0-preview', 'net8.0'
    
    # Save if changed
    if ($newContent -ne $content) {
        $newContent | Out-File $file.FullName -Encoding UTF8 -Force
        Write-Host "  Updated $($file.Name)" -ForegroundColor Green
    }
}

Write-Host "Done updating project files!" -ForegroundColor Green
