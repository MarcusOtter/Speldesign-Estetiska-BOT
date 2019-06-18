Speldesign Estetiska BOT
-----------------------------
| Active | Inactive (see [#55](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues/55)) |
| ------ | -------- |
| [![Build status](https://ci.appveyor.com/api/projects/status/7u45ojhicarcph72?svg=true)](https://ci.appveyor.com/project/LeMorrow/speldesign-estetiska-bot) | [![codecov](https://codecov.io/gh/LeMorrow/Speldesign-Estetiska-BOT/branch/master/graph/badge.svg)](https://codecov.io/gh/LeMorrow/Speldesign-Estetiska-BOT) |

|  | About  |
| ---- | :----: |
| ![Speldesign estetiska BOT logo](https://images-ext-1.discordapp.net/external/_vHG8tWTHiVQWEugTxxvcR1BY-370reLXVgFw8CX9Mg/%3Fsize%3D64/https/cdn.discordapp.com/avatars/473916590732214272/39b2494dd3b7bcf578ccdc41284ee2f9.png) | A discord bot made with .NET Core and :coffee: for game design students from [![Uppsala estetiska gymnasium logo](https://i.imgur.com/ZrpBl85.png "Uppsala kommun") Uppsala estetiska gymnasium](https://estetiska.uppsala.se/blielev/speldesign/).<br>Currently being hosted on a Raspberry Pi 3 (named LadyBug :beetle:) running Raspbian GNU/Linux 9 (stretch). |

## Auto-deployment
Has the ability to update itself when there are new commits in this repo. The screenshot below is a showcase of this feature. Notice how the GitHub webhook mentions [1b5ce5a - Add uptime command](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/commit/1b5ce5a825f5543f152b92af4b9eadbce9cc08be). This command is then available after the bot has updated.

*:bulb: Curious about the implementation? Check out the commands [here](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/blob/master/SpeldesignBotCore/Modules/UpdateCommands.cs) and the shell/batch scripts [here](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/tree/master/Setup/shell)*.

![Auto-deployment showcase](https://i.imgur.com/kxgoAZF.png)

## Commands
![Bot banner](https://i.imgur.com/qSIetxq.png)

The bot currently has `13` working commands. There are many more features planned, some of which can be found [here](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues?q=is%3Aopen+is%3Aissue+label%3Aenhancement).

*:warning: These commands are subject to change and the usage may not be accurate by the time you are reading this.*

### `help`
A list of all the available commands, prefixed with the current prefix. If this is invoked from a server, the bot will send a direct message to you with the commands (if you have direct messages enabled).

### `help [command]` 
Planned feature. Will show more information about a particular command.

### `uptime`
How long the bot has been running.

### `info`
Information about this bot. Depending on the machine, this may also give you information about the available RAM and current CPU usage.

| Raspberry Pi (does not support PerformanceCounter)  | Lenovo (supports PerformanceCounter) |
| --------------------------------------------------- | ------------------------------------ |
| ![](https://i.imgur.com/R5o2kNh.png)                | ![](https://i.imgur.com/vFYOkxM.png) |

*Note: the development bot does not have a profile picture set, so it does not get embedded with a profile picture in the top-right corner.*

### `shutdown`
Make the bot log out and shut down.

### `prefix`
The current prefix of the bot. Also displays how to use the bot.

| Example usage                        |
| ------------------------------------ |
| ![](https://i.imgur.com/du0qbWs.png) |

### `setprefix`
Set the prefix of the bot.

| Example usage                        |
| ------------------------------------ |
| ![](https://i.imgur.com/SPQcOi6.png) |

### `addclass [@class] [names]`
Add a school class to the server.<br>
`@class` is the school class role.<br>
`names` is a comma-separated list with all the names in the class.

| Example usage                        |
| ------------------------------------ |
| ![](https://i.imgur.com/Bdm4xWd.png) |

### `removeclass [@class]`
Remove a school class from the server.<br>
`@class` is the school class role.

| Example usage                        |
| ------------------------------------ |
| ![](https://i.imgur.com/nmbjLB5.png) |

### `classes`
A list of all the classes. Also provides information about how many students are in each class, and how many of those students have joined discord.

| Example usage                        |
| ------------------------------------ |
| ![](https://i.imgur.com/ducNepQ.png) |

### `remainingstudents [@class]`
A list of students from a class that have not joined the discord server yet.<br>
`@class` is the school class role.

| Example usage                        |
| ------------------------------------ |
| *In server*<br>![](https://i.imgur.com/Em23RUG.png)<br>*In DMs*<br>![](https://i.imgur.com/uW3tPe9.png) |

### `checkupdate [upstream]`
Check if there is an update available from the specified upstream.
`upstream` is an optional parameter that defaults to origin/master. This is the branch that should be checked against.

### `update [upstream]`
Download changes and update the bot from the specified upstream. 
`upstream` is an optional parameter that defaults to origin/master. This is the branch that changes should be downloaded from.

### `draw [amount] [item]`
*:warning: This command is being reworked.*<br>
Draws items from a repository of items to give away. Used to give a prize to server contest winners.

## Registration
The bot has a whitelist system to prevent people that are not in the school from joining.

To join the server, users need to register is the following format: `@class Full Name`. If the bot can find this person in the specified class, they are nicknamed to their full name and assigned their school class role. If not, the bot will try and help the user.

| Unauthorized user                    | Wrong format                                                                  | Typo                                             |
| ------------------------------------ | ----------------------------------------------------------------------------- | ------------------------------------------------ |
| ![](https://i.imgur.com/gl5pmSc.png) | ![](https://i.imgur.com/G2dTf6d.png)<br>![](https://i.imgur.com/VzuDFnK.png) | ![](https://i.imgur.com/g3lqguk.png) |

*A typo is when the [levenshtein distance](https://en.wikipedia.org/wiki/Levenshtein_distance) between the input and an existing name is less than 5 letters. If it is more than that the user is considered "unauthorized" and does not get a name suggestion.*

## Message logging
If a logging channel is provided in the bot configuration, the bot will log messages in that channel. It also provides a link the message being logged.

### Sent messages
![](https://i.imgur.com/o2OxViy.png)

### Edited messages
![](https://i.imgur.com/QgsGKUd.png)
