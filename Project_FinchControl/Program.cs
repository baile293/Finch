using System;
using System.Collections.Generic;
using FinchAPI;
using System.IO;

namespace Project_FinchControl
{
    // **************************************************
    //
    // Title: Finch Control
    // Description: 
    // Application Type: Console
    // Author: Jamie
    // Date Created: 10/15/19
    // Last Modified: 
    //
    // **************************************************

    class Program
    {
        public enum Command
        {
            NONE,
            MOVEFOWARD,
            MOVEBACKWARD,
            STOPMOTORS,
            WAIT,
            TURNRIGHT,
            TURNLEFT,
            LEDON,
            LEDOFF,
            DONE
        }
        static void Main(string[] args)
        {
            SetTheme();
            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        static void SetTheme()
        {
            string dataPath = @"Data\Theme.txt";
            string foregroundColorString;
            ConsoleColor foregroundColor;

            //
            // read and convert foreground color to enum
            //
            foregroundColorString = File.ReadAllText(dataPath);
            Enum.TryParse(foregroundColorString, out foregroundColor);
            Console.ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// display welcome screen
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFinch Control");
            Console.WriteLine();
            DisplayContinuePrompt();
        }
        /// <summary>
        /// display closing screen
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Finch Control!");
            Console.WriteLine();
            DisplayContinuePrompt();
        }
        static void DisplayMenuScreen()
        {
            Finch finchRobot = new Finch();
            bool finchRobotConnected = false;
            bool quitApplication = false;
            string menuChoice;
            do
            {
                //
                // get user menu choice
                //
                DisplayScreenHeader("Menu");
                Console.WriteLine("a) Connect Finch Robot");
                Console.WriteLine("b) Talent Show");
                Console.WriteLine("c) Data Recorder");
                Console.WriteLine("d) Alarm System");
                Console.WriteLine("e) User Programming");
                Console.WriteLine("f) DisplayGetDataRecorderFrequency");
                Console.WriteLine("q) Disconnect Finch Bot");
                Console.WriteLine("Enter choice");
                menuChoice = Console.ReadLine().ToLower();
                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        finchRobotConnected = DisplayConnectFinchRobot(finchRobot);
                        break;
                    case "b":
                        if (finchRobotConnected)
                        {
                            DisplayTalentShow(finchRobot);
                            DisplayContinuePrompt();
                        }
                        else
                        {
                            Console.WriteLine("Finch robot is not connected. Please return to Main Menu and connect.");
                            DisplayContinuePrompt();
                        }
                        break;
                    case "c":
                        DisplayDataRecord(finchRobot);
                        DisplayContinuePrompt();
                        break;
                    case "d":
                        DisplayAlarmSystem(finchRobot);
                        DisplayContinuePrompt();
                        break;
                    case "e":
                        DisplayUserProgramming(finchRobot);
                        DisplayContinuePrompt();
                        break;
                    case "f":
                        DisplayGetDataRecorderFrequency(finchRobot);
                        DisplayContinuePrompt();
                        break;
                    case "q":
                        DisplayDisconnectFinchRobot(finchRobot);
                        quitApplication = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("please enter a letter for the menu choice");
                        DisplayContinuePrompt();
                        break;
                }
            } while (!quitApplication);
        }
        #region USER PROGRAMMING

        static void DisplayFinchCommands(List<Command> commands)
        {
            DisplayScreenHeader("Finch Commands");

            Console.WriteLine("Commands:");
            Console.WriteLine();

            foreach (Command command in commands)
            {
                Console.WriteLine(command);
            }
        }

        static void  DisplayGetFinchCommands(List<Command> commands)
        {
            string userResponse;
            Command command = Command.MOVEFOWARD;
            DisplayScreenHeader("Finch Robot Commands");

            while (command != Command.DONE)
            {
                Console.Write("Enter Command:");
                userResponse = Console.ReadLine().ToUpper();
                Enum.TryParse(userResponse, out command);

                commands.Add(command);
            }

            // echo command list

            DisplayContinuePrompt();
        }

        static (int motorSpeed, int ledBrightness, int waitSeconds) DisplayGetCommandParameters()
        {
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;

            DisplayScreenHeader("Command Parameters");

            Console.Write("Enter Motor Speed [0 - 255]:");
            commandParameters.motorSpeed = int.Parse(Console.ReadLine());

            Console.Write("Enter LED Brightness [0 - 255]:");
            commandParameters.ledBrightness = int.Parse(Console.ReadLine());

            Console.Write("Enter Wait Command in Seconds:");
            commandParameters.waitSeconds = int.Parse(Console.ReadLine());

            // todo echo values to user

            DisplayContinuePrompt();

            return commandParameters;
        }

        static void DisplayUserProgramming(Finch finchRobot)
        {
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameter;
            commandParameter.motorSpeed = 0;
            commandParameter.ledBrightness = 0;
            commandParameter.waitSeconds = 0;
            List<Command> commands = new List<Command>();
            string menuChoice;
            bool quitApplication = false;

            do
            {
                DisplayScreenHeader("User Programming");

                //
                // get user menu choice
                //
                Console.WriteLine("a) Set Command Paramenters");
                Console.WriteLine("b) Add Commands");
                Console.WriteLine("c) View Commands");
                Console.WriteLine("d) Execute Commands");
                Console.WriteLine("e) Write Commands to Data File");
                Console.WriteLine("e) Read Commands from Data File");
                Console.WriteLine("q) Return to Main Menu");
                Console.Write("Enter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        commandParameter = DisplayGetCommandParameters();
                        break;

                    case "b":
                        DisplayGetFinchCommands(commands);
                        break;

                    case "c":
                        DisplayFinchCommands(commands);
                        break;

                    case "d":

                        break;

                    case "e":
                        DisplayWriteUserProgrammingData(commands);
                        break;
                    case "f":
                        commands = DisplayReadUserProgrammingData();
                        break;
                    case "q":
                        quitApplication = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("Please enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitApplication);
        }

        static List<Command> DisplayReadUserProgrammingData()
        {
            string dataPath = @"Data\Data.txt";
            List<Command> commands = new List<Command>();
            string[] commandsString;

            DisplayScreenHeader("Read Data form File");

            Console.WriteLine("Ready to read from the date file.");
            DisplayContinuePrompt();

            commandsString = File.ReadAllLines(dataPath);

            //
            // creat list of Command Enum
            //

            Command command;
            foreach (string commandString in commandsString)
            {
                Enum.TryParse(commandString, out command);
                commands.Add(command);
            }

            DisplayContinuePrompt();

            return commands;
        }

        static void DisplayWriteUserProgrammingData(List<Command> commands)
        {
            string dataPath = @"Data\Data.txt";

            List<string> commandsString = new List<string>();

            DisplayScreenHeader("Write Data to File");

            //
            // create a list of command strings
            //
            foreach (Command command in commands)
            {
                commandsString.Add(command.ToString());
            }

            Console.WriteLine("Ready to write to the date file.");
            DisplayContinuePrompt();

            File.WriteAllLines(dataPath, commandsString.ToArray());

            DisplayContinuePrompt();
        }
        static void DisplayExecuteFinchCommands(
            Finch finchRobot, 
            List<Command> commands,
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameter)
        {
            int motorSpeed = commandParameter.motorSpeed;
            int ledBrightness = commandParameter.ledBrightness;
            int waitSeconds = commandParameter.waitSeconds;

            DisplayScreenHeader("Execute Finch Commands");

            //info and pause
            Console.ReadKey();

            foreach (Command command in commands)
            {
                switch (command)
                {
                    case Command.NONE:
                        break;
                    case Command.MOVEFOWARD:
                        finchRobot.setMotors(motorSpeed, motorSpeed);
                        break;
                    case Command.MOVEBACKWARD:
                        finchRobot.setMotors(-motorSpeed, -motorSpeed);
                        break;
                    case Command.STOPMOTORS:
                        finchRobot.setMotors(0, 0);
                        break;
                    case Command.WAIT:
                        finchRobot.wait(waitSeconds * 1000);
                        break;
                    case Command.TURNRIGHT:
                        break;
                    case Command.TURNLEFT:
                        break;
                    case Command.LEDON:
                        break;
                    case Command.LEDOFF:
                        break;
                    case Command.DONE:
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        static int DisplayGetNumberOfDataPoints(Finch finchRobot)
        {
            int numberOfDataPoints;

            DisplayScreenHeader("Get number of data points");

            Console.Write("Enter the number of data points.");
            int.TryParse(Console.ReadLine(), out numberOfDataPoints);

            DisplayMenuScreen();

            return numberOfDataPoints;
        }

        static void DisplayAlarmSystem(Finch finchRobot)
        {
            string alarmType;
            int maxSeconds;
            double threshold;
            bool thresholdExceeded;

            DisplayScreenHeader("Alarm System");

            alarmType = DisplayGetAlarmType();
            maxSeconds = DisplayGetMaxSeconds();
            threshold = DisplayGetThreshold(finchRobot, alarmType);

            //warn the user

            thresholdExceeded = MonitorLightLevels(finchRobot, threshold, maxSeconds);

            if (thresholdExceeded)
            {
                if (alarmType == "string")
                {
                    Console.WriteLine("Maximum Light Level Exceeded");
                }
                else
                {
                    Console.WriteLine("Maximum Temperature Exceeded");
                }
            }
            else
            {
                Console.WriteLine("Maximum Monitoring Time Exceeded");
            }
            
            DisplayMenuScreen();
        }

        static bool MonitorLightLevels(Finch finchRobot, double threshold, int maxSeconds)
        {
            bool thresholdExceeded = false;
            double seconds = 0;
            int currentLightLevel;
            

            while (!thresholdExceeded && seconds <= maxSeconds)
            {
                currentLightLevel = finchRobot.getLeftLightSensor();

                DisplayScreenHeader("Monitor Light Levels");
                Console.WriteLine($"Maximum Light Level: {(int)threshold}");
                Console.WriteLine($"Current Light Level: {finchRobot.getLeftLightSensor()}");

                if (currentLightLevel >= threshold)
                {
                    thresholdExceeded = true;
                }

                finchRobot.wait(500);
                seconds += 0.5;
            }

            return thresholdExceeded;
        }

        static double DisplayGetThreshold(Finch finchRobot, string alarmType)
        {
            double threshold;

            DisplayScreenHeader("Threshold Value");

            switch (alarmType)
            {
                case "light":
                    Console.WriteLine($"Current Light Level: {finchRobot.getLeftLightSensor()}");
                    Console.Write("Enter Maximum Light Level:");
                    threshold = double.Parse(Console.ReadLine());
                    break;

                default:
                    throw new FormatException();
                    break;
            }

            DisplayContinuePrompt();

            return threshold;
        }
        static int DisplayGetMaxSeconds()
        {
            //validate user input!
            Console.Write("Maximum Seconds: ");
            return int.Parse(Console.ReadLine());
        }

        static string DisplayGetAlarmType()
        {
            //validate user input!
            Console.Write("Alarm Type [light or tempterature]:");
            return Console.ReadLine();
        }

        static void DisplayDataRecord(Finch finchRobot)
        {
            double frequency;
            int numberOfDataPoints;

            DisplayScreenHeader("Data Recorder");

            frequency = DisplayGetDataRecorderFrequency(finchRobot);
            numberOfDataPoints = DisplayGetNumberOfDataPoints(finchRobot);

            //
            // instantiate (create) array
            //
            double[] temperatures = new double[numberOfDataPoints];

            // warn the user before recording
            DisplayGetDataReadings(numberOfDataPoints, frequency, temperatures, finchRobot);

            DisplayDataRecorderData(temperatures);

            DisplayContinuePrompt();
        }

        static void DisplayDataRecorderData(double[] temperatures)
        {
            DisplayScreenHeader("Temperatures");

            // provide some info to the user

            for (int index = 0; index < temperatures.Length; index++)
            {
                Console.WriteLine($"Temperature {index + 1}: {temperatures[index]}");
            }

            DisplayContinuePrompt();
        }

        static void DisplayGetDataReadings(
            int numberOfDataPoints, 
            double frequencyOfDataPoints,
            double[] temperatures,
            Finch finchRobot)
        {
            DisplayScreenHeader("Get Temperature Recordings");

            // prompt the user
            DisplayContinuePrompt();

            //
            // get temperatures
            //
            for (int index = 0; index < numberOfDataPoints; index++)
            {
                temperatures[index] = finchRobot.getTemperature();
                int milliSeconds = (int)(frequencyOfDataPoints * 1000);
                finchRobot.wait(milliSeconds);
                Console.WriteLine($"Temperature {index + 1}: {temperatures[index]}");
            }

            DisplayContinuePrompt();
        }

        static double DisplayGetDataRecorderFrequency(Finch finchRobot)
        {
            double frequency;
            DisplayScreenHeader("Get Frequency of Recording");
            Console.Write("Enter Frequency [seconds]");
            // userResponse = Console.Readline();
            // double.TryParse(userResponse, out frequency);
            double.TryParse(Console.ReadLine(), out frequency);

            return frequency;
        }

        static void DisplayTalentShow(Finch finchRobot)
        {
            DisplayScreenHeader("Talent Show");

            Console.WriteLine("The Finch robot will now show off its talent");

            for (int lightLevel = 0; lightLevel < 255; lightLevel++)
            {
                finchRobot.setLED(lightLevel, 0, 0);
            }
            DisplayContinuePrompt();
            //
            //move finch
            //
            finchRobot.setMotors(255, 255);
            finchRobot.wait(1000);
            finchRobot.setMotors(0, 0);
            finchRobot.setMotors(200, -200);
            finchRobot.wait(2000);
            finchRobot.setMotors(150, 200);
            finchRobot.wait(1500);
            finchRobot.setMotors(200, 150);
            finchRobot.wait(1500);
            finchRobot.setMotors(-255, 255);
            finchRobot.wait(3000);
            finchRobot.setMotors(100, 0);
            finchRobot.wait(2000);
            finchRobot.setMotors(0, 100);
            finchRobot.wait(2000);
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2000);
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2000);
            finchRobot.setMotors(0, 0);
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2000);
            finchRobot.setMotors(0, 0);
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2000);
            finchRobot.setMotors(0, 0);
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2000);
            finchRobot.setMotors(0, 0);
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2000);
            finchRobot.setMotors(0, 0);

            finchRobot.setMotors(0, 0);
            //
            //red LED
            //
            finchRobot.setLED(255, 0, 0);
            finchRobot.noteOn(340);
            finchRobot.wait(4000); //milliseconds
            finchRobot.noteOff();
            //
            //LED and play sound
            //
            finchRobot.setLED(0, 255, 0);
            finchRobot.noteOn(340);
            finchRobot.wait(6000); //milliseconds
            finchRobot.noteOff();

            //
            //LED and play sound
            //
            finchRobot.setLED(0, 0, 255);
            finchRobot.noteOn(340);
            finchRobot.wait(6000); //milliseconds
            finchRobot.noteOff();

            //
            //play note for 2 seconds
            //
            finchRobot.setLED(255, 255, 255);
            finchRobot.noteOn(340);
            finchRobot.wait(5000); //milliseconds
            finchRobot.noteOff();
            Console.ReadKey();

            DisplayContinuePrompt();
        }
        static bool DisplayConnectFinchRobot(Finch finchRobot)
        {
            bool robotConnected;
            DisplayScreenHeader("Connect to Finch Robot");
            Console.WriteLine("About to connect to finch robot. Please be sure the USB cable is connected to the robot and computer now.");
            DisplayContinuePrompt();
            robotConnected = finchRobot.connect();
            if (robotConnected)
            {
                Console.WriteLine("The Finch robot is now connected,");
                finchRobot.setLED(0, 255, 0);
                finchRobot.noteOn(15000);
                finchRobot.wait(1000);
                finchRobot.noteOff();
            }
            else
            {
                Console.WriteLine("unable to connect to the Finch robot.");
            }
            DisplayContinuePrompt();
            return robotConnected;
        }
        static void DisplayDisconnectFinchRobot(Finch finchRobot)
        {
            DisplayScreenHeader("Disconnect FInch Robot");
            Console.WriteLine("About to disconnect from the Finch robot.");
            DisplayContinuePrompt();
            finchRobot.disConnect();
            Console.WriteLine("The Finch robot is now disconnected.");
            DisplayContinuePrompt();
        }
        #region HELPER METHODS
        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
        /// <summary>
        /// display screen header
        /// </summary>
        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }
        #endregion
    }
}