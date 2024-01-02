# Resonite-Mods<!-- omit in toc -->

<img align="right" width="256" height="256" src="https://git.ljoonal.xyz/ljoonal/Resonite-Mods/raw/Logo.png"/>

[![Discord](https://img.shields.io/discord/901126079857692714?label=discord&logo=discord&style=flat)](https://discord.gg/2WR6rGVzht)
[![Latest release](https://img.shields.io/badge/dynamic/json.svg?label=release&url=https://git.ljoonal.xyz/api/v1/repos/ljoonal/Resonite-Mods/releases&query=$[0].tag_name&style=flat&logo=gitea)](https://lj.munally.com/resonite/mods/releases)
[![AGPL-3](https://img.shields.io/badge/license-AGPL--3-black?style=flat&logo=open-source-initiative)](https://tldrlegal.com/license/gnu-affero-general-public-license-v3-(agpl-3.0))
[![Lines of code](https://img.shields.io/tokei/lines/git.ljoonal.xyz/ljoonal/Resonite-Mods?label=lines&style=flat&logo=C-Sharp)](.)

This repository contains some of my mods for [Resonite](https://store.steampowered.com/app/2519830/Resonite/) using [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).

Note that the license is AGPL, meaning you might be obligated to provide the source code (with any modifications you've made) for these mods if someone asks you when you're using them.
This is meant to stop people from creating private malicious clients.
The license might be changed in the future to a more permissive one.

## Warning<!-- omit in toc -->

No warranty is provided for these mods, and they're provided as-is.
Please have a look at the source code & build from source for maximal safety.
I also recommend mirroring this git repository if you want to make sure you always have access to the source code.

## Mod list<!-- omit in toc -->

If you want the feature enough to get the mod for it, you should probably also go give a thumbs up on the issue tracker if there is one.

- [Screenmode Tweaks](#screenmode-tweaks) (upstream issue not found)
- [Latest Log](#latest-log) (upstream issue not found)
- [Linux Fixes](#linux-fixes) ([#47](https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/47) & [#100](https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/100))
- [Privacy Shield](#privacy-shield) (upstream issue not found, [closest found: #224](https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/224))

### Screenmode Tweaks

Small tweaks to make screen (desktop) mode less miserable.

Current options include:

- Changing vSync
- Setting max focused FPS
- Setting max unfocused FPS

### Latest Log

Creates a symbolic link in the Resonite folder named `Latest.log` to the latest log file that was created on launch.

So you can keep following the file (with `tail -F Latest.log` on linux for example) without needing to constantly change the filename and such.

### Linux Fixes

Fixes the most annoying bugs that I come accross whilst playing Resonite on linux.

Currently implements:

- Centers mouse on context menu open
- Fixes being able to rotate the grabbed object
- Reverses the reverse mouse scroll direction.

### Privacy Shield

A few attempts at improving privacy slightly.

The main feature is making all requests (excluding local:// and resdb://) require you granting permission to it, instead of just logix ones.
It uses the same domain allowlist/blocklist as the normal logix requests.
So no more tracking pixels!

The other semi-sensible feature is spoofing the local timezone.
It's disabled by default, and any changes will require a restart.

Also includes a slightly questionable feature, FPS spoofing.
Questionable as it might also just break some things, doesn't really protect you, and can be summarized as:
> Your scientists were so preoccupied with whether or not they could, they didn't stop to think if they should.

The FPS spoofing allows sensible values (so pick like between 30-60), and disables with anything else (so -1 for example).
It doesn't spoof anything other than the Local User's FPS (so dT or the PerformanceMetrics component will leak your real FPS).
But it does at least currently provide some privacy from some session user manager type of UIs.
I'd kindly ask you to not to try to work around this if you're making tools, at least if your tool isn't opt-in.
Since I think that this is a somewhat decent way to opt-out of FPS tracking.

## For developers<!-- omit in toc -->

### Building<!-- omit in toc -->

Ensure that the required DLL's (listed in the `Directory.build.props` file and in the individual `.csproj` files) can be found from standard installation paths (check `Directory.build.props`).
Then use the `dotnet build` command to build.
A few examples include running `dotnet build ScreenmodeTweaks/ScreenmodeTweaks.csproj` to build ScreenmodeTweaks in development mode or `dotnet build -c Release Resonite-Mods.csproj` to build all the mods in release mode.

Alternatively you can try to open the folder in Visual Studio, but I cannot provide help for using that.
If you do want to improve the situation, do feel free to contribute!

### Contacting & contributing<!-- omit in toc -->

Contact me [on the FrooxEngine Modding discord server](https://discord.gg/vCDJK9xyvm), [elsewhere](https://ljoonal.xyz/contact), and/or possibly send me git patches if you've already written any code that you'd like to get merged.

Also if anyone from the Resonite team is reading this, do feel free to get in touch!
