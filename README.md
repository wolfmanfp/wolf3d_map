# Wolfenstein 3D - Map for DOSBox
## About
Realtime map for Wolfenstein 3D and Tristania 3D.  
The doors don't show up for some reason. Everything else works.  
Tested with DOSBox Staging 0.83-RC1.
## Run
The program uses DOSBox Staging's [REST API](https://www.dosbox-staging.org/releases/release-notes/0.83.0-rc1/#http-api-for-modding-external-tools), introduced in version 0.83-RC1. To enable it, you need to edit your `dosbox-staging.conf` like this:
```
[webserver]
webserver_enabled = on
webserver_port = 8080
```
If you set a custom value for `webserver_port`, you can edit that in `wolf3d_map.ini`. 
## Build
The project requires .NET Framework 4.5 to build, and an IDE such as Visual Studio or JetBrains Rider.