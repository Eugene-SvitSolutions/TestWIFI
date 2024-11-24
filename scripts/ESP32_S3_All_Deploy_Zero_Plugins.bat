rem set COM_PORT=COM7
set COM_PORT=COM6
rem Update ESP32 with flash rom
rem nanoff --target ESP32_S3_ALL --serialport %COM_PORT% --deploy --image "ShockTracker.Device.Engine\bin\Debug\ShockTracker.Device.Engine.bin"
rem nanoff --nanodevice --update --serialport %COM_PORT%
rem nanoff --update --target ESP32_S3_ALL --serialport %COM_PORT%  --masserase --filedeployment C:\path\deploy.json

rem nanoff --update --target ESP32_S3_ALL --serialport %COM_PORT%  --masserase
nanoff --filedeployment C:\Src\TestAppWIFI\scripts\Deploy.Json -v d