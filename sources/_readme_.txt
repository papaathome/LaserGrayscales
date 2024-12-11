Solution LaserGrayscales notes.
==============================

Version:
-------

The current version number is set in LaserGrayscales.csproj, node Project.PropertyGroup.Version

Format for the version number is 'major.minor.build.bugfix'
The major number is incremented on new functiontionality or large changeover.
The minor number is incremented (in steps of 2) on breaking changes, even numbers are release versions, odd numbers are debug versions.
The build number is incremented on changes not adding new functionality (e.g. on optimising performance.)
The bugfix number is incremented on bugfixes.


Implementation details:
----------------------

The Settings class is the central object with all grayscale and application details or references to other configuration classes. Settings are loader on program start and saved (if changed) on program exit.

The Plotter class is responsible for generating a test script based on configuration details. It is responsible for executing all operations on an image in the right order in with the right context.

Configuration details and plotter actions are separated over several partial class files.

The main screen is driving all actions by user input, no scripting or unattended mode available.

On application close configuration files are stored if changes are made to it.
