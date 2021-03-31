using System;
using System.IO;
using InfinityScript;
using static InfinityScript.GSCFunctions;
using System.Globalization;

namespace SetSpawn
{
    public class SetSpawn : BaseScript
    {
        public static readonly string _mapname = GetMapName();
        private static string FolderSpawns = "scripts\\SetSpawn";
        private static string FileLogin = "scripts\\SetSpawn\\SetSpawn.txt";
        private static string FolderMaps = "scripts\\SetSpawn\\Maps";
        public int verify;


        public SetSpawn()
        {
            this.ServerStart();
            this.PlayerConnected += new Action<Entity>(this.playerConnected);
        }
        private static string GetMapName() => GetDvar("mapname");

        private void ServerStart()
        {
            if (!Directory.Exists(SetSpawn.FolderSpawns))
            {
                Directory.CreateDirectory(SetSpawn.FolderSpawns);
            }
            if (!Directory.Exists(SetSpawn.FolderMaps))
            {
                Directory.CreateDirectory(SetSpawn.FolderMaps);
            }
            if (!File.Exists(FileLogin))
            {
                File.Create(FileLogin).Close();
                File.WriteAllLines(FileLogin, new string[]
                {
                    "Password=MRX450"
                });
            }
        }

        private static void onPlayerSpawn(Entity player)
        {
            string CurrMap = _mapname;
            string str1 = (string)null;
            string str2 = (string)null;
            string str3 = (string)null;
            string GetMap = (string)null;

            if (_mapname == null)
            {
                Print("Failed to find the map...");
            }
            else
            {
                foreach (string str in File.ReadAllLines("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt"))
                {
                    if (str.StartsWith("Map"))
                    {
                        GetMap = str.Split(new char[1]
                        {
                             '='
                        })[1];
                    }
                }
                if (GetMap == CurrMap)
                {
                    foreach (string str4 in File.ReadAllLines("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt"))
                    {
                        if (str4.StartsWith("PosX"))
                        {
                            str1 = str4.Split(new char[1]
                            {
                                '='
                            })[1];
                        }
                    }
                    foreach (string str5 in File.ReadAllLines("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt"))
                    {
                        if (str5.StartsWith("PosY"))
                        {
                            str2 = str5.Split(new char[1]
                            {
                                 '='
                            })[1];
                        }
                    }
                    foreach (string str6 in File.ReadAllLines("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt"))
                    {
                        if (str6.StartsWith("PosZ"))
                        {
                            str3 = str6.Split(new char[1]
                            {
                                 '='
                            })[1];
                        }
                    }
                    float x = Convert.ToSingle(str1, new CultureInfo("en-US"));
                    float y = Convert.ToSingle(str2, new CultureInfo("en-US"));
                    float z = Convert.ToSingle(str3, new CultureInfo("en-US"));

                    player.SetOrigin(new Vector3(x, y, z));
                }
            }
        }

        private void playerConnected(Entity player)
        {
            verify = 0;

            player.SpawnedPlayer += () => onPlayerSpawn(player);

            onPlayerSpawn(player);

            AfterDelay(4000, delegate
            {
                Utilities.RawSayAll("^7Console: ^2SetSpawn ^7developed by ^:MRX450 ^7and a special thanks to ^:LiteralLySugaR");
            });
        }

        public override BaseScript.EventEat OnSay2(Entity player, string name, string message)
        {
            string[] Array = message.Split(' ');

            string password = "";
            foreach (string text in File.ReadAllLines(FileLogin))
            {
                if (text.StartsWith("Password"))
                {
                    password = text.Split(new char[1]
                    {
                        '='
                    })[1];
                }
            }


            if (Array[0].Equals("!sslogin"))
            {
                if (Array.Length == 1)
                {
                    Utilities.RawSayTo(player, "^7Usage: ^1!login ^2<password>");
                    return BaseScript.EventEat.EatGame;
                }
                if (Array[1] == password)
                {
                    verify = 1;
                    Utilities.RawSayTo(player, "^7Console: ^7You have been successfully logged in!");
                    return BaseScript.EventEat.EatGame;
                }
                else
                {
                    Utilities.RawSayTo(player, "^7Console: ^7Incorrect password!");
                    return BaseScript.EventEat.EatGame;
                }
            }
            if (Array[0] == "!setspawn")
            {
                if (verify == 1)
                {
                    if (!File.Exists("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt"))
                    {
                        File.Create("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt").Close();
                    }

                    StreamWriter spawn = new StreamWriter("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt");
                    spawn.WriteLine("Map=" + _mapname);
                    spawn.WriteLine("PosX=" + player.Origin.X);
                    spawn.WriteLine("PosY=" + player.Origin.Y);
                    spawn.WriteLine("PosZ=" + player.Origin.Z);
                    spawn.WriteLine($"Set by {player.Name} at ({DateTime.Now})");
                    spawn.Flush();
                    spawn.Close();
                    Utilities.RawSayTo(player, "^7Console: ^2Spawn ^7was ^2successfully ^7added^2!!!");
                    return BaseScript.EventEat.EatGame;
                }
                else
                {
                    Utilities.RawSayTo(player, "^7Console: ^2You need do login to use SetSpawn");
                    return BaseScript.EventEat.EatGame;
                }
            }
            return EventEat.EatNone;
        }
    }
}