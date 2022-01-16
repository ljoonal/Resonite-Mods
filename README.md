# Neos-Plugins<!-- omit in toc -->

[![Discord](https://img.shields.io/discord/901126079857692714?label=discord&logo=discord&style=flat)](https://discord.gg/2WR6rGVzht)
[![Latest release](https://img.shields.io/badge/dynamic/json.svg?label=release&url=https://git.ljoonal.xyz/api/v1/repos/ljoonal/Neos-Mods/releases&query=$[0].tag_name&style=flat&logo=gitea)](https://neos.ljoonal.xyz/releases)
[![AGPL-3](https://img.shields.io/badge/license-AGPL--3-black?style=flat&logo=open-source-initiative)](https://tldrlegal.com/license/gnu-affero-general-public-license-v3-(agpl-3.0))
[![Lines of code](https://img.shields.io/tokei/lines/git.ljoonal.xyz/ljoonal/Neos-Mods?label=lines&style=flat&logo=C-Sharp)](.)

This repository contains some of my mods for [NeosVR](https://store.steampowered.com/app/740250/Neos_VR/) using [NeosModLoader](https://github.com/zkxs/NeosModLoader).

Note that the license is AGPL, meaning you'll have to provide the source code (with any modifications you've made) for these mods if someone asks you when you're using them.
This is meant to stop people from creating private malicious clients.
The license might be changed in the future to a more permissive one.

## Warning<!-- omit in toc -->

No warranty is provided for these mods, and they're provided as-is.
Please have a look at the source code & build from source for maximal safety.
I also recommend mirroring this git repository if you want to make sure you always have access to the source code.

## Mod list<!-- omit in toc -->

If you want the feature enough to get the mod for it, you should probably also go give a thumbs up on the issue tracker if there is one.

- [Screenmode Tweaks](#screenmode-tweaks)
- [Linux Fixes](#linux-fixes)

### Screenmode Tweaks

Small tweaks to make screen (desktop) mode less miserable.

Currently just has the option to disable VSync.

### Linux Fixes

Fixes the most annoying bugs that I come accross whilst playing NeosVR on linux.
Currently implements fixing grabbed object rotation.

## For developers<!-- omit in toc -->

### Building<!-- omit in toc -->

Ensure that the required DLL's (listed in the `Directory.build.props` file and in the individual `.csproj` files) can be found from standard installation paths (check `Directory.build.props`).
Then use the `dotnet build` command to build.
A few examples include running `dotnet build ScreenmodeTweaks/ScreenmodeTweaks.csproj` to build ScreenmodeTweaks in development mode or `dotnet build -c Release Neos-Plugins.csproj` to build all the plugins in release mode.

Alternatively you can try to open the folder in Visual Studio, but I cannot provide help for using that.
If you do want to improve the situation, do feel free to contribute!

### Contacting & contributing<!-- omit in toc -->

Contact me [on the Neos Modding discord server](https://discord.gg/vCDJK9xyvm), [elsewhere](https://ljoonal.xyz/contact), and/or possibly send me git patches if you've already written any code that you'd like to get merged.

Also if anyone from the Neos team is reading this, do feel free to get in touch!
