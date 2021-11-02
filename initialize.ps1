$registryKeyPath = "HKLM:\SOFTWARE\Policies\Microsoft\Windows\OOBE"
New-Item -Path $registryKeyPath -Force
New-ItemProperty -Path $registryKeyPath -PropertyType DWORD -Name "DisablePrivacyExperience" -Value 1 -Force

function GetBase10Id ($idLength, $idOffset) {
  $hostName = hostname
  $alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
  $base36Id = $hostName.Substring($hostName.Length - 6).ToUpper().ToCharArray()
  $base10Id = 0
  $raiseRower = 0
  [System.Array]::Reverse($base36Id)
  foreach ($idDigit in $base36Id) {
    $base10Id += $alphabet.IndexOf($idDigit) * [System.Math]::Pow(36, $raiseRower)
    $raiseRower += 1
  }
  if ($idOffset) {
    $base10Id += $idOffset
  }
  $base10Id = $base10Id.ToString()
  while ($base10Id.Length -lt $idLength) {
    $base10Id = "0$base10Id"
  }
  return $base10Id
}

$idOffset = 2200
$base10Id = GetBase10Id 4 $idOffset 
$hostName = "LAPRO$base10Id"
Rename-Computer -NewName $hostName -Restart
