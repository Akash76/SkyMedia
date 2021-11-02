$registryKeyPath = "HKLM:\SOFTWARE\Policies\Microsoft\Windows\OOBE"
New-Item -Path $registryKeyPath -Force
New-ItemProperty -Path $registryKeyPath -PropertyType DWORD -Name "DisablePrivacyExperience" -Value 1 -Force
