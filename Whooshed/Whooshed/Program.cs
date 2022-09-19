using System;
using System.Collections.Generic;
using System.IO;


namespace Whooshed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Argument of file location is missing!");
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Welcome to Voynich Hotel booking program.\nPlease, select menu item you need");
                    Console.WriteLine();
                    Console.WriteLine("1. Show aviable room list");
                    Console.WriteLine("2. Book a room");
                    Console.WriteLine("3. Exit");
                    string path = args[0];
                    List<Room> rooms = GetRooms(path);
                    ConsoleKeyInfo choice = Console.ReadKey(true);
                    if (choice.KeyChar == '1')
                    {
                        try
                        {
                            if (rooms != null)
                            {
                                Console.Clear();
                                Console.WriteLine("Aviable room list:\n");
                                Console.WriteLine("Room\tRank\tStatus");
                                foreach (Room r in rooms)
                                {
                                    Console.WriteLine("{0}:\t{1}\t{2}", r.num, r.type, r.stat);
                                }
                            } else
                            {
                                Console.WriteLine("Something went wrong while I was reading a file...\n");
                            }
                            Console.WriteLine("Press any key to go back to the menu");
                            Console.ReadKey(true);
                            Console.Clear();
                        }
                        catch (InvalidCastException e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ERROR: ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("File contains wrong data");
                        }
                        catch (IOException e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ERROR: ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("File cannot be opened. Is it protected file?");
                        }
                    }
                    else if (choice.KeyChar == '2')
                    {
                        Console.WriteLine();
                        long outpt;
                        
                        while (true)
                        {
                            bool flag_room = false;
                            ConsoleKeyInfo ki = new ConsoleKeyInfo();
                            Console.Clear();
                            Console.WriteLine("Please enter a number of a room you want to book");
                            Console.WriteLine("Remember that you can book only \"aviable\" and \"empty\" rooms\n");
                            Console.WriteLine("Room\tRank\tStatus");
                            foreach (Room r in rooms)
                            {
                                Console.WriteLine("{0}:\t{1}\t{2}", r.num, r.type, r.stat);
                            }
                            Console.WriteLine();
                            Console.WriteLine("You can enter \"q\" key to cancel room booking");
                            string roomnum = Console.ReadLine();
                            int t = 0;
                            if (roomnum != null && int.TryParse(roomnum, out t) && roomnum.ToLower() != "q")
                            {
                                foreach (Room r in rooms)
                                {
                                    if (t == r.num && (r.stat == "available" || r.stat == "empty"))
                                    {
                                        flag_room = true;
                                    }
                                }
                            } else if (roomnum.ToLower() == "q")
                            {
                                break;
                            }
                            if (flag_room)
                            {
                                Console.Clear();
                                Console.WriteLine("Please enter tyour phone number below to confirm booking. Room will be booked for a while\n");
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                string num = Console.ReadLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                if (num.Length != 11 || !long.TryParse(num, out outpt))
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("This is not how phone number looks like. Try again.");
                                    Console.WriteLine();
                                    Console.ReadKey(true);
                                }
                                else
                                {
                                    Console.WriteLine("Room temporary booked for you, we will call you soon.\nRemember that if you didn't pick up, the book will be nullified");
                                    foreach (Room r in rooms)
                                    {
                                        if (r.num == t)
                                        {
                                            r.stat = "booked";
                                        }
                                    }
                                    irrelevant(path, rooms);
                                    break;
                                }
                            } else
                            {
                                Console.WriteLine("\nRoom you wanted to book is not available or not existing");
                                Console.WriteLine("Press any key to try again");
                                Console.ReadKey(true);
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine("Press any key to go back to the menu");
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                    else if (choice.KeyChar == '3')
                    {
                        break;
                    }
                }
            }
        }

        static void irrelevant(string path, List<Room> rooms)
        {
            while (true)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(path, false);
                    foreach (Room r in rooms)
                    {
                        sw.WriteLine("{0} {1} {2}", r.num, r.type, r.stat);
                    }
                    sw.Flush();
                    sw.Close();
                    break;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("Please do not move file with data anywhere while I'm running." +
                        "\nYour book of room has been nullified");
                }
                catch (IOException e)
                {
                    Console.WriteLine("This file is already opened, I can't write in it");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("{0}", e);
                    Console.WriteLine("Free file from other interactions and press any key to try again");
                    Console.ReadKey(true);
                }
            }
        }

        static List<Room> GetRooms(string path)
        {
            bool flag = false;
            List<Room> res = new List<Room>();
            try
            {
                string room;
                using (StreamReader sr = new StreamReader(path, true))
                {
                    while ((room = sr.ReadLine()) != null)
                    {
                        string[] prms = room.Split();
                        res.Add(new Room(int.Parse(prms[0]), prms[1], prms[2]));
                        flag = true;
                    }
                }
            }
            catch (FileNotFoundException e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong!\nProbably, File you looking for is not existing\nError: {0}", e);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            catch (OperationCanceledException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please don't delete data file while I'm reading it");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("I don't know what to do with it.\n Error: {0}", e);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            return flag == true ? res : null;
        }
    }

    /// <summary>
    /// Class of room in a hotel. Contains information about it's number and type
    /// </summary>
    public class Room
    {
        public int num;
        public string type;
        public string stat;

        public Room(int _num, string _type, string _stat)
        {
            num = _num;
            type = _type;
            stat = _stat;
        }
        public Room(int _num, string _type)
        {
            num = _num;
            type = _type;
        }
    }

}
