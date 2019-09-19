Function WaitFor {
  Param (
    [System.String]  $Name,
    [System.String]  $Address,
    [System.Int32]   $Port
  )
  Do
  {
    Write-Host "Waiting for $Name on [:$Port]"
    Start-Sleep 3
  } Until (Test-NetConnection $Address -Port $Port | ? { $_.TcpTestSucceeded } )
}

docker-compose up -d db
Waitfor -Name 'database' -Address 'localhost' -Port 5434

docker-compose up ps_schemaupdater

docker-compose up ps_api
