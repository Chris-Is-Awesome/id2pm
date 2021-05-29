![ID2 Practice Mod](/Images/logo.png)

### [Download! Install instructions below!](https://github.com/Chris-Is-Awesome/id2pm/releases)

**Credits: Modded by [Chris is Awesome](https://github.com/Chris-Is-Awesome) with UI code from Matra**

This is a practice mod that adds features like savestates, fast travel (coming soon), cheats, a timer, and more. This mod **will be allowed** for speedruns thanks to its anticheat, so you don't have to uninstall mod to do runs. See speedrun rules (updating upon final release) for details. During a run, none of the mod's code will run.

If you would like to join the discssion on this mod and suggest features or submit bug reports, join the [Ittle Dew speedrunning server](https://discord.gg/FANbQE5FVG)!

## Installation

### Install
Download the `id2pm.zip` file in the latest version from [here](https://github.com/Chris-Is-Awesome/id2pm/releases). Extract its contents to the directory of your Ittle Dew 2 install: `[Steam games directory]\Ittle Dew 2` and that's it! Can configure hotkeys by reading below.

### Uninstall
Right click the game in Steam -> Properties -> Local Files -> Verify integrity of game files...
That will replace the game's code with vanilla code, but keep your mod folder stuff (hotkeys, settings, etc.) intact, so if you ever decide to reinstall, you'll be up and running again in no time!

## Hotkeys

Included in the files you download is a `mods/Practice Mod` folder with a file named `Hotkeys.json`. In here you can set up hotkeys for each command as well as change the hotkey for opening debug menu.

The format is as such:
```json
{
	"hotkeys": [
		{
			"name": "OpenDebugMenu",
			"key": "KeyCodeName"
		},
		{
			"name": "CommandToExecute",
			"key": "KeyCodeName"
		}
	]
}
```
`OpenDebugMenu` key defaults to F1, but feel free to change that. For the custom hotkeys, the `"name"` vlaue must  be given the exact command text you would like to execute, including arguments; just as you would type into debug menu. So for example: `"name": "speed 5"` or `"name": "god"`.

The key is the Unity `KeyCode` name for the key, see [this page](https://docs.unity3d.com/ScriptReference/KeyCode.html) for the names of keys.

Note that hotkeys will only run if the player has control (meaning they won't run while paused or during respawn or when player doesn't exist).

## Commands

There are a variety of commands to be used in debug menu. To open the debug menu, pause the game and press your `OpenDebugMenu` key (defualt `F1`). You can use the `help` command to get list of all commands and type `help [command name]` to get info on each command.

## Debugging

If posting a bug report, I may ask for the output log. The output log is a file in the root `ID2_Data` directory named `output_log.txt`. Contains a lot of useful debugging info to help me diagnose issues.