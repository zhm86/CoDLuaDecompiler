# Overview
[![Releases](https://img.shields.io/github/downloads/JariKCoding/CoDLuaDecompiler/total.svg)](https://github.com/JariKCoding/CoDLuaDecompiler/)

**CoDLuaDecompiler** is a lua decompiler for Lua Scripts from various Call Of Duty games with the Havok and LuaJit VM. It's main purpose is to provide access to scripts that Treyarch did not provide in the Call of Duty: Black Ops III Mod Tools and to give greater insight into how Treyarch and the other studios achieved certain things, rebuild menus from the game, etc.

Supports following games out of the box: **BlackOps2**, **BlackOps3**, **BlackOps4**, **BlackOpsColdWar**, **Ghosts**, **AdvancedWarfare**, **InfiniteWarfare**, **ModernWarfareRemastered**, **ModernWarfare2CR**, **ModernWarfare**, **WorldWar2** and **Vanguard**.

This is made from Katalash's DSLuaDecompiler and this wouldn't be possible without his repo that he put tons of work into. I was going to merge this but I made too many edits specifically for CoD.

### Why is this decompiler better than all my other ones?

- Proper loop detection
- SSA (Keeping track of different variables)
- ...

### What can be improved

- Still has some errors with different loops that need to be debugged

### How to Use 

- To decompile a couple/a single file(s) just drop it on the .exe
- To decompile whole folders open the program with the path as a parameter
- To decompile from a running game's memory, add **--export** as a parameter (Currently only Black Ops 3, 4, Cold War, World War 2, Modern Warfare and Vanguard are supported)

## Download

The latest version can be found on the [Releases Page](https://github.com/JariKCoding/CoDLuaDecompiler/releases).

## Requirements

* Windows 7 x86 and above
* .NET 5

## Credits

- DTZxPorter - Original lua disassembler to find the basics
- Scobalula - Utilities and general help
- Katalash & jam1garner - DSLuaDecompiler

## License 

CoDLuaDecompiler is licensed under the MIT license and its source code is free to use and modify. CoDLuaDecompiler comes with NO warranty, any damages caused are solely the responsibility of the user. See the LICENSE file for more information.