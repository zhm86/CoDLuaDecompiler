# Overview
[![Releases](https://img.shields.io/github/downloads/JariKCoding/CoDHVKDecompiler/total.svg)](https://github.com/JariKCoding/CoDHVKDecompiler/)

**CoDHVKDecompiler** is a lua decompiler for Havok Scripts from various Call Of Duty games. It's main purpose is to provide access to scripts that Treyarch did not provide in the Call of Duty: Black Ops III Mod Tools and to give greater insight into how Treyarch and the other studios achieved certain things, rebuild menus from the game, etc.

Supports following games out of the box: **BlackOps2**, **BlackOps3**, **BlackOps4**, **Ghosts**, **AdvancedWarfare**, **InfiniteWarfare**, **ModernWarfareRemastered** and **WorldWar2**

This is made from Katalash's DSLuaDecompiler and this wouldn't be possible without his repo that he put tons of work into. I was going to merge this but I made too many edits specifically for CoD

### Why is this decompiler better than all my other ones?

- Proper loop detection
- SSA (Keeping track of different variables)

### What can be improved

- Not all files get decompiled yet
- Still has some errors with different loops that need to be debugged

### How to Use 

- To decompile a couple/a single file(s) just drop it on the .exe
- To decompile whole folders open the program with the path as a parameter

## Download

The latest version can be found on the [Releases Page](https://github.com/JariKCoding/CoDHVKDecompiler/releases).

## Requirements

* Windows 7 x86 and above
* .NET Core 3.1

## Credits

- DTZxPorter - Original lua disassembler to find the basics
- Scobalula - Utilities and general help
- Katalash & jam1garner - DSLuaDecompiler

## License 

CoDHVKDecompiler is licensed under the MIT license and its source code is free to use and modify. CoDHVKDecompiler comes with NO warranty, any damages caused are solely the responsibility of the user. See the LICENSE file for more information.