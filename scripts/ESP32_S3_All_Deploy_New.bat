REM Deploy new firmware and erase existing structure. Need to be in boot mode

set COM_PORT=COM7
rem Update ESP32 with flash rom
rem nanoff --target ESP32_S3_ALL --serialport %COM_PORT% --deploy --image "ShockTracker.Device.Engine\bin\Debug\ShockTracker.Device.Engine.bin"
rem nanoff --nanodevice --update --serialport %COM_PORT%

nanoff --update --target ESP32_S3_ALL --serialport %COM_PORT%  --masserase

