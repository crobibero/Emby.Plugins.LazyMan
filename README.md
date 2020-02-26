# Emby.Plugins.LazyMan

LazyMan Channel for Emby

Emby caches the game streams, so you (the Emby admin) must run the "Refresh Internet Channels Task" 

`Admin interface -> Scheduled Tasks -> Internet Channels -> Refresh Internet Channels`


Get started with LazyMan at https://reddit.com/r/LazyMan

Hostsfile directions: https://www.reddit.com/r/LazyMan/wiki/hostsfile

You must edit your hosts file to use this plugin!
for docker either use `--add-host` or `extra_hosts`

##### NOTE: The offical docker does not work for IP binding! use https://hub.docker.com/r/linuxserver/emby in place of it.

Steps to install:
1. Download [latest release](https://github.com/crobibero/Emby.Plugins.LazyMan/releases/latest).
2. Extract Emby.Plugins.LazyMan.dll to the Emby plugins directory
3. Add host entries
4. Restart Emby

docker compose sample:

```
emby:
    container_name: emby
    image: linuxserver/emby:latest
    restart: always
    extra_hosts:
        - "mf.svc.nhl.com:{ip}"
        - "mlb-ws-mf.media.mlb.com:{ip}"
        - "playback.svcs.mlb.com:{ip}"
```
