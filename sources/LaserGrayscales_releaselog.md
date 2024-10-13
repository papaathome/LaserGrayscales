LaserGrayscales Release Log
===========================


LaserGrayscales is a tool to test the effects of your laser at power levels 1 through 255 at speeds ranging from 1 mm/s to 70 mm/s or 60 mm/min to 4200 mm/min. This is a good way to see how your laser will react to different materials at different speeds and power levels.

While it is possible to generate test code for each step, in power and speed, this will generate a file of about 16 KB, using an area of 28 by 14 cm (11 by 5 1/2 inch) and a run time of about one hour.

Another approach is to generate an general overview in large steps to find corse location of the ideal spot and then zoom in on a smaller area with finer steps. This will save a lot of time and material.

With LaserGrayscales both approaches are possible. For more details see the LaserGrayscales manual.

In this file details about released versions and the development path for LaserGrayscales is given.


Content
-------
* Development path.
* Known problems.
* Releases.


Development path
----------------

Requirement status:
 * ? Requested, may move to a following release.
 * . Not yet implemented.
 * x Implemented but partial, untested or broken.
 * v Implemented and tested.

Version: 0.1.0.0, development, debug.
Version 0.2.0.0, development release requirements:
Release date: not yet available.
 * x documentation available
 * ? github sources publicly available.
 * v load and safe configuration file.
 * v generate test script file.
 * v button [generate] reports result and last successfull action.

Version: x.y.0.0, wishlist.
 * ? button [generate] enabled when the configuration is valid.
 * ? icon for Grayscales.
 * ? swap X and Y axis.
 * ? load/store different configurations.
 * ? Set Z height.
 * ? Set G0 speed.
 * ? Set F scale in mm/sec or mm/min (GUI only)
 * ? Set (0, 0) location.
 * ? Set laser power command using S or Q
 * ? Set comment mode using ';' or '(' and ')'
 * ? Report min/max at design time in GUI.
 * ? Add a border and grid (scaled)
 * ? Add form for settings.
 * ? Add lead in and lead out for acceleration compensation.
 * ? Add side rulers pattern.
 * ? Add up and down ruler pattern for backlash checking.
 * ? Add image encoding.


Known problems
--------------

Issues and bugs are not yet recorded (which is an issue on its own).
For the time being, known problems are recorded here.

 3 . Path to the test script file must be edited 'by hand', path selector not (yet) implemented.
 2 v Button [generate] does not report results to the user.
 1 . Button [generate] is always enabled, also when the configuration is not valid.


Releases
--------

Version 0.2.0.0, development, stable.
Version 0.1.0.0, development, debug.
Not yet released, unmet requirements.
 * . documentation available
 * . github sources publicly available.
 * v load and safe configuration file.
 * v generate test script file.

