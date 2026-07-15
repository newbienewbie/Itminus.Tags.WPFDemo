

Get-ChildItem -Filter paket-files -Recurse | Remove-Item -Force -Recurse
Get-ChildItem -Filter bin -Recurse | Remove-Item -Force -Recurse
Get-ChildItem -Filter obj -Recurse | Remove-Item -Force -Recurse

if(Test-Path -Path .vs) 
{
    Remove-Item -Path .vs -Force -Recurse
}
