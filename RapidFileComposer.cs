using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using RAPCAM.Models.Fanuc;
using RAPCAM.Models.General;
using Rhino;

// Fucture translated code of Luke Johns from Visual Basic to C#
// we share our version of code for generating RAPID code 
// under Creative Commons Attribution-ShareAlike 3.0 Unported License.

//Ryan Luke Johns / September 2013 / www.gshed.com
//This work is licensed under the Creative Commons Attribution-ShareAlike 3.0 Unported License.
//To view a copy Of this license, visit http://creativecommons.org/licenses/By-sa/3.0/.
//Use of this code is at your own risk.  I accept no responsibility for damage or injury. Robots are dangerous:  get familiar with the code,
//run a simulation before physically executing the code, stay outside of the reach envelope, etc...
//
//VB component for generating robTargets given a list of Planes.  Uses the plane origin and frame to calculate the position and quaternion values
//of the robTarget.  In this component, it is assumed that configuration and external axes are not enabled.  To access those variables, use the
//advanced robTarget component.

namespace RAPCAM.BusinessProcess
{
    public static class RapidFileComposer
    {
        // this method takes lists of strings, meaning the speed for example can be variable throughout the cuttin plane. This for now will be fixed, but let
        // in RAPCAM development we will extend this so that it would be possible to set speed for specific parts of path manually or automatically by checking
        // the curvature of cutting path

        public static List<String> generateRapidFile(String moduleName,  List<Plane> planes, List<String> speed, List<String> zone, List<String> tool, List<String> workObject, List<String> variables)
        {
            List<String> rapidLines = new List<String>();

            String strHead = "MODULE " + moduleName;
            String strMain = "PROC main()";
            String strConfL = "ConfL \\Off;";
            String strConfJ = "ConfJ \\Off;";

            rapidLines.Add(strHead);
            rapidLines.Add("VAR confdata cf := [0,-1,-1,0];");
            rapidLines.Add("VAR extjoint ex := [9E9,9E9,9E9,9E9,9E9,9E9];");

            if(variables != null && variables.Count > 0)
            {
                foreach(String variable in variables)
                {
                    rapidLines.Add(variable);
                }
            }

            rapidLines.Add(strMain);
            rapidLines.Add(strConfL);
            rapidLines.Add(strConfJ);

            List<String> rapidCommands = moveL(planes, speed, zone, tool, workObject);
            foreach(String command in rapidCommands)
            {
                rapidLines.Add(command);
            }

            rapidLines.Add("ENDPROC");
            rapidLines.Add("ENDMODULE");

            return rapidLines;
        }

        private static List<String> moveL(List<Plane> planes, List<String> speed, List<String> zone, List<String> tool, List<String> workObject)
        {
            bool toolDataExists = (tool != null && tool.Count() > 0);
            bool workObjectExists = (workObject != null && workObject.Count() > 0);

            String toolData;
            String workObjectData;
            String speedData;
            String zoneData;
            List<String> robTargets = generateRapidLines(planes);
            List<String> commandList = new List<String>();
            
            for(int i = 0; i < robTargets.Count - 1; i++)
            {
                if (toolDataExists)
                {
                    if(i > tool.Count() - 1)
                    {
                        toolData = tool[tool.Count() - 1]; // set to last one that existed
                    }
                    else
                    {
                        toolData = tool[i]; // set to current one
                    }
                }
                else
                {
                    toolData = "tool0"; // default
                }

                if (workObjectExists)
                {
                    if (i > workObject.Count() - 1)
                    {
                        workObjectData = "\\WObj:=" + workObject[workObject.Count() - 1]; // set to last one that existed
                    }
                    else
                    {
                        workObjectData = "\\WObj:=" + workObject[i]; // set to current one
                    }
                }
                else
                {
                    workObjectData = String.Empty;
                }

                if(speed != null && speed.Count() > 0 && i > speed.Count() - 1)
                {
                    speedData = speed[speed.Count() - 1];
                }
                else
                {
                    speedData = speed[i];
                }

                if (zone != null && zone.Count() > 0 && i > zone.Count() - 1)
                {
                    zoneData = zone[zone.Count() - 1];
                }
                else
                {
                    zoneData = zone[i];
                }

                String strC = "MoveL " + robTargets[i] + "," + speedData + "," + zoneData + "," + toolData + workObjectData + ";";
                commandList.Add(strC);
            }

            return commandList;
        }

        private static List<String> generateRapidLines(List<Plane> planes)
        {
            Vector3d vectorUp = new Vector3d(0, 0, 1);
            Plane world = new Plane(new Point3d(0, 0, 0), vectorUp);
            List<String> formattedStrings = new List<String>();
            for (int i = 0; i < planes.Count(); i++)
            {
                Plane currentPlane = planes[i];
                Quaternion planeQuat = Quaternion.Rotation(world, currentPlane);
                String strQ = String.Format("{0:0.000000},{1:0.000000},{2:0.000000},{3:0.000000}", planeQuat.A, planeQuat.B, planeQuat.C, planeQuat.D);
                String strP = String.Format("{0:0.00},{1:0.00},{2:0.00}", currentPlane.OriginX, currentPlane.OriginY, currentPlane.OriginZ);
                String strT = "[[" + strP + "],[" + strQ + "],cf,ex]";
                formattedStrings.Add(strT);
            }

            return formattedStrings;
        }
    }
}