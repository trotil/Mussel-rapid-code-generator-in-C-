C# CODE LICENSE

This work is Licensed under Creative Commons Attribution-ShareAlike 3.0 Unported License.
To view a copy Of this license, visit http://creativecommons.org/licenses/By-sa/3.0/.

Fucture (Matas Ubarevicius @ http://fucture.org) translated code of Luke Johns from Visual Basic to C#, we share our version of code for generating RAPID ABB robot code. Restrictions in original license code, which is included after also applies.


ORIGINAL CODE LICENSE

Ryan Luke Johns / September 2013 / www.gshed.com

This work is licensed under the Creative Commons Attribution-ShareAlike 3.0 Unported License.
To view a copy Of this license, visit http://creativecommons.org/licenses/By-sa/3.0/.

Use of this code is at your own risk.  I accept no responsibility for damage or injury. Robots are dangerous:  get familiar with the code, run a simulation before physically executing the code, stay outside of the reach envelope, etc...

VB component for generating robTargets given a list of Planes.  Uses the plane origin and frame to calculate the position and quaternion values of the robTarget.  In this component, it is assumed that configuration and external axes are not enabled.  To access those variables, use the advanced robTarget component.
