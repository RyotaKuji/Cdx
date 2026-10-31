#%USERPROFILE%\Documents\PowerShell\Microsoft.PowerShell_profile.ps1

function cdx {
	param(
		[string]$path = (Get-Location)
	)

	$exe = "C:\WorkSpace\Prog\DotNet\Cdx\Cdx\bin\Release\net10.0-windows\publish\Cdx.exe"
	$outFile = Join-Path $env:TEMP "cdx-result-$PID.txt"
	Remove-Item $outFile -ErrorAction SilentlyContinue

	& $exe "--current-directory" $path "--write-result" $outFile
	$code = $LASTEXITCODE

	if ($code -eq 0 -and (Test-Path $outFile)) {
		$target = (Get-Content $outFile -Raw).Trim()
		Remove-Item $outFile -ErrorAction SilentlyContinue
		
		if (-not [string]::IsNullOrWhiteSpace($target)) {
			if (Test-Path -LiteralPath $target -PathType Container) {
				Set-Location -LiteralPath $target
			}
			elseif (Test-Path -LiteralPath $target -PathType Leaf) {
				Set-Location -LiteralPath (Split-Path $target)
				Start-Process -FilePath "cmd.exe" -ArgumentList ('/c start "" "' + $target + '"')
			}
		}
	}
}
