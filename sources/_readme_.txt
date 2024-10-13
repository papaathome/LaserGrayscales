Solution LaserGrayscales notes.
==============================

Version:
-------

The current version number is set in LaserGrayscales.csproj, node Project.PropertyGroup.Version

The config file has a File Version number set in Config.FILE_VERSION

Format for both version numbers is 'major.minor.build.bugfix'
The major number is incremented on new functiontionality or large changeover.
The minor number is incremented (in steps of 2) on breaking changes, even numbers are release versions, odd numbers are debug versions.
The build number is incremented on changes not adding new functionality (e.g. on optimising performance.)
The bugfix number is incremented on bugfixes.

Development path:
----------------

Version 0.2x+1.0.0, development, debug
Version 0.2x.0.0, development, release

Version 1.2x+1.0.0, stable, debug
Version 1.2x.0.0, stable, release

See release log for more details.


Implementation details:
----------------------

The Config class is the central object with all grayscale and application details.

The Manager class is for coordinating actions. With this rather simple appliction there is not much in it.

The TestModel class is responsible for generating a test script based on configuration details.

The main screen is driving all actions on user input.

On application close the configuration is stored it if changes are made to it.

The debug log is (in my oppinion) fouled up by caliburn.micro log messages. The actual messages are writen at level 'Info' but with the CmLog4NetLogger class degraded to level 'Debug'. For normal logging use Log4Net ILogger InfoFormat(...), in general use the *Format(...) items from the ILogger API.
