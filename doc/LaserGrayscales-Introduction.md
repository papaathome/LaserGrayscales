# LaserGrayscales


Generate gray scales in G-code for CNC laser engravers, c# vs2022, .NET 8 WPF application.

Laser engraving as function of X and Y while changing Z-height, speed and laser power for test and check purposes.

Full documentation of LaserGrayscales [available](LaserGrayscales.pdf). This introduction will explain some basics, for full details see the documentation.

**Warning**: Generating a test script does imply that a file is generated. It does **not** imply that a _valid_ file is generated. Always check the content of a generated test script before you run it! Remember, LaserGrayscales is still in beta versions.

<p align="center">
  <img alt="LaserGrayscales started" src=".\source\images\Grayscales-v0.1.3.png">
</p>


## Introduction.

LaserGrayscales is a tool to test the effects of your laser at power levels 1 through 255 at speeds ranging from 1 mm/s to 70 mm/s or 60 mm/min to 4200 mm/min (the values mentioned here are configurable). This is a good way to see how your laser will react to different materials at different speeds and power levels.

While it is possible to generate test code for each step, in power and speed, this will generate a file of about 16 KB, using an area of 28 by 14 cm (11 by 5 1/2 inch) and a run time of about one hour.

Another approach is to generate an general overview in large steps to find corse location of the ideal spot and then zoom in on a smaller area with finer steps. This will save a lot of time and material.

With LaserGrayscales both approaches are possible.

LaserGrayscales was started 'out of necessity' of having no proper test tools (and not finding any) for a new laser engraver tool. On the internet a [g-code script](https://www.thingiverse.com/thing:3349071) is available but its use of g-code is targeting different elements of the industry standard RS274 format than what was acceptable for some g-code interpreter. Changing the script manually was time consuming and just for one script where more testing is required.


## Properties

* Can draw lines, (empty) boxed and (grey level filled) squares.
* Can draw in any size and repeat in any quantity (in a 'x by y' rectangular form).
* Can line up dawings next to each other or in groups.
* Can vary Z height, speed and laser power for each dawing.
* Can create test patterns in g-code conforming to RS274D
* Will save the g-code at any location.
* Will check that used g-code is within limits of target CNC machine capabilities.
* Will load a default test setting at program start.
* Will create default configuration files if none are found.
* Will save changed settings on program close.
* Can save and load test settings at any time to any location.
* Independent configuration for target CNC machine.
* Independent configuration for target G-Code interpreter.
* Independent configuration for a test.
* Independent configurations can quickly be combined for other targets 


### How it works.

For a full explanation see the documentation.

Groups of parallel lines are drawn with variations in laser power and speed. The lines are drawn in the X direction while being parallel in the Y direction.

Variation in the X and Y direction can be in laser power, speed and heigth, the variations in each direction are added together for each variable.

The laser is continuesly on while traversing over one X line and off while traveling to the next X line.

The lines are grouped in the Y direction with a configurable width between each line. The number of lines in one group is configurable and between each group there is a configurable gap.

Starting and ending values of each mode are limited to a (configurable) minimum and maximum, defaults for Laser Power is 0 to 255, speed 1 to 70 mm/sec. Height is by default constant but it is possible to change that as well.

The starting value can be less than or greater than the ending value, the generated g-code is moving from start value to end value with constant increments.

The user supplied script intro, header and footer is added to the G-Code.

The generated code is for UCCNC but with the use of 'standard' G-Codes. The default header explain all code used for managing the laser equipment.

All configuration and the last settings used are available in configuration files. Editing these files with a simple text editor is possible but not needed. All values can be edited in the various tabs.

A separate configuration file is used for the target machine, the target G-Code interpreter, the specific test and the application.


## The test script

A test script file contains several sections:
 - An intro (including a remark with which version of LaserGrayscale the script was generated).
 - A header for preparing script settings.
 - The body of the scipt, this contains G-code for all engraving.
 - A footer for terminating the script.

Code is generated with clear comments. It should be relative easy to follow what is done if you understand the basic G-code syntax.

### Example snippet from generated g-code.
```
( This file is generated with LaserGrayscales, v0.3.0.0, 2024-12-13 09:54:20 )
( GCode using UCCNC conventions or RS-274 interpretation )
( Test and check the content before you use it. Use at your own risk, protect your eyes! )

( This file will test the effect of your laser at power levels 1 through 255 at speeds ranging from 1 mm/s to 70 mm/s or 60 mm/min to 4200 mm/min. )
( This is a good way to see how your laser will react to different materials at different speeds & power levels. )

(   >>> ALWAYS WEAR PROPER EYE PROTECTION WHEN WORKING WITH LASERS <<<   )

( Metric: all GCode lengths in 'mm' and speeds in 'mm/min' )

...

( TestPatternSquarse4x4 uses X from 0 to 4.5 mm and Y from 0 to 4.5 mm )

G90                      ( absolute positions )
M3                       ( laser enabled )
( Header start )
( Header end )

M10 Q0
G0 X0 Y0 F2400           ( To outer pattern part #x=1, #y=1 )
M10 Q100
G1 X2 Z0 F1200
G1 Y2
G1 X0
G1 Y0
M10 Q0
G0 Y0.5
M10 Q100
G1 X2
M10 Q0
G0 X0
G0 Y1
...
M10 Q120
G1 X4.5
M10 Q0
G0 X2.5
G0 Y3.5
M10 Q120
G1 X4.5
M10 Q0
G0 X2.5
G0 Y4
M10 Q120
G1 X4.5
M10 Q0
G0 X2.5
M11                      ( laser off )
M5                       ( laser disabled )
( Footer )
G0 X0 Y0 Z0 ( go home )
M30    ( Program end, rewind )
( Footer end )
```


## Text and buttons.

One screen provides all text and buttons. The screen uses several tabs to arrange data in sensible groups of settings. Each tab, text and button is explained in the documentation.


## Logging.
For general purposes and debugging log files are created for each run. The log files are plain text and 'informative' only. The amount of details in each log, up to the point of generating or not of the log, are specified in the file 'LaserGrayscales.log4net.config'.

Log4Net is a standard software tool for logging. If you want to change the log settings please do a search on the internet for find the information on how to do it.

If you do not want to generate any log file make the following change to 'LaserGrayscales.log4net.config' In the node 'root' you find a node 'level', almost at the end of the file. Change this line as desired to one of the following settings.

For all logging on.
```
<root>
    <!-- Levels = [ OFF, FATAL, ERROR, WARN, INFO, DEBUG, ALL ] -->
    <level value="ALL" />
    ...
</root>
```


For all logging off.
```
<root>
    <!-- Levels = [ OFF, FATAL, ERROR, WARN, INFO, DEBUG, ALL ] -->
    <level value="OFF" />
    ...
</root>
```
When switched off a log file is still generated but nothing is writen to it, the file will have a size of 0 bytes.
