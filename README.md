# LaserGrayscales

Generate gray scales in G-code for CNC laser engravers, c# vs2022, .NET 8 WPF application.

Laser engraving as function of X and Y where one axis is changing speed and the other is changing laser power.

<p align="center">
  <img alt="LaserGrayscales started" src=".\doc\bootscreen.png">
</p>

## Introduction

LaserGrayscales is a tool to test the effects of your laser at power levels 1 through 255 at speeds ranging from 1 mm/s to 70 mm/s or 60 mm/min to 4200 mm/min. This is a good way to see how your laser will react to different materials at different speeds and power levels.

While it is possible to generate test code for each step, in power and speed, this will generate a file of about 16 KB, using an area of 28 by 14 cm (11 by 5 1/2 inch) and a run time of about one hour.

Another approach is to generate an general overview in large steps to find corse location of the ideal spot and then zoom in on a smaller area with finer steps. This will save a lot of time and material.

With LaserGrayscales both approaches are possible.

For more details see the LaserGrayscales manual.

See the [full scale test script](LaserGrayscaleTest-fullscale.nc) for an example of a generated script.


## Related Projects

See: [Laser Calibration Power vs Feedrate (For Marlin/Repetier/RepRap)](https://www.thingiverse.com/thing:3349071)
Not being able to use this G-code source on a Stepcraft DL445 laser with UCCNC g-code interpreter it became the initiator for LaserGrayscales.

See [gcode_tpgen](https://github.com/vector76/gcode_tpgen)
Found after realising the first version of LaserGrayscales. Not tested. It may become a good source of ideas on further testing of materials and your laser equipment.

see [gnea/grbl laser mode](https://github.com/gnea/grbl/wiki/Grbl-v1.1-Laser-Mode)
Good background information on laser operations in GRBL and in general.


## Build, test and installation of release versions.

Building is done with Microsoft Visual Studio Community 2022. Testing of release versions is done on a windows 10 system and a windows 7 system.

### Actions for building.

Open the solution file ('.\sources\LaserGrayscales.sln') select 'Release' and then 'Rebuild'.
The results are available in '.\sources\LaserGrayscales\bin\Release\net8.0-windows'

### Actions for installing.

No special actions are required. LaserGrayscales needs all files from its release directory and nothing more. You can copy the executable (and all the files and subdirectoriew with it) to any convenient location.

On first startup your system might ask for a specific Microsoft Runtime system, please follow the Microsoft directions to install it.


## Security

For directions on decurity issues see [SECURITY](SECURITY.md)

## Dependencies

Grayscales is realised with Visual Studio 2022 in a .NET 8 application using WPF with the help of two external packages.

Building is done with Microsoft Visual Studio Community 2022 (VisualStudio.17.Release/17.11.3+35303.130).


| NuGet package | descption |
| ------------- | --------- |
| Log4Net | logging |
| Caliburn Micro | MVVM framework |


## License

Copyright (c) A.H.M. Steenveld. All rights reserved.

Licensed under the [MIT No Attribution](LICENSE.txt) license.

