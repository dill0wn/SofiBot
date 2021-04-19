# Sofi Bot #

This is a Discord bot that send photos from your Photos.app library. Only works on macOS currently, but theoreticaally could work anywhere that a .photoslibrary exists.

## Installation / Setup ##

Prereqs
* python3.x
* Photos.app or .photoslibrary
* some dotnet stuff.

Install the requirements from requirements.txt (either in a virtualenv or not).
`pip install -r requirements.txt`


### Running / Building
Can build/run with `dotnet build` and `dotnet run`. Also using VSCode it's set up with launch.json.


### Publishing
There's a VSCode task to build for osx. However, if you want to do it manually, run:

```
dotnet publish -c Release --self-contained -r <runtime-id>
```
Replace `<runtime-id>` with one of the ids in [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).


### Installing As a Service
For macos, I have inclulded a sample plist to be modified and placed in ~/Libray/LaunchAgents. Rebooting will load it up, or can be done manually with 
```
launchctl load ~/Library/LaunchAgents/com.dill0wn.sofibot.plist
```