using System;
using System.IO;
using InfinityScript;
using System.Globalization;

namespace SetSpawn
{
    public class SetSpawn : BaseScript
    {
        private static string FolderSpawns = "scripts\\SetSpawn";
        private static string FileLogin = "scripts\\SetSpawn\\SetSpawn.txt";
        private static string FolderMaps = "scripts\\SetSpawn\\Maps";
        public int verify;


        public SetSpawn()
        {
            this.ServerStart();
            this.PlayerConnected += new Action<Entity>(this.playerConnected);
        }


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


        private void onPlayerSpawn(Entity player)
        {
            string _mapname = Call<string>("getdvar", new Parameter[]
            {
                "mapname"
            });

            string CurrMap = _mapname;
            string PoxX = (string)null;
            string PoxY = (string)null;
            string PoxZ = (string)null;
            string GetMap = (string)null;

            if (_mapname == null)
            {
                Log.Write(LogLevel.Info, "Failed to find the map...");
                //Console.WriteLine("Failed to find the map...");
                //Utilities.RawSayTo(player, "^7Console: Failed to find the map...");
            }
            else
            {
                foreach (string str in File.ReadAllLines("scripts\\SetSpawn\\Maps\\" + _mapname + ".txt"))
                {
                    GetMap = str.StartsWith("Map") ? str.Split(new char[1] { '=' })[1] : GetMap;
                    PoxX = str.StartsWith("PosX") ? str.Split(new char[1] { '=' })[1] : PoxX;
                    PoxY = str.StartsWith("PosY") ? str.Split(new char[1] { '=' })[1] : PoxY;
                    PoxZ = str.StartsWith("PosZ") ? str.Split(new char[1] { '=' })[1] : PoxZ;
                }
                if (GetMap == CurrMap)
                {
                    float x = Convert.ToSingle(PoxX, new CultureInfo("en-US"));
                    float y = Convert.ToSingle(PoxY, new CultureInfo("en-US"));
                    float z = Convert.ToSingle(PoxZ, new CultureInfo("en-US"));

                    player.Call("setorigin", new Parameter[]
                    {
                        new Vector3(x, y, z)
                    });
                }
            }
        }

        private void playerConnected(Entity player)
        {
            verify = 0;

            player.SpawnedPlayer += () => onPlayerSpawn(player);

            onPlayerSpawn(player);

            base.OnInterval(4000, delegate
            {
                base.Call("iprintln", new Parameter[]
                {
                    "^7Console: ^2SetSpawn ^7developed by ^:MRX450 ^7and a special thanks to ^:LiteralLySugaR"
                });
                return false;
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
                    Utilities.RawSayTo(player, "^7Console: ^7You have been successfully logged in!");
                    verify = 1;
                    return BaseScript.EventEat.EatGame;
                }
                else
                {
                    Utilities.RawSayTo(player, "^7Console: ^7Incorrect password!");
                    return BaseScript.EventEat.EatGame;
                }
            }
            string _mapname = base.Call<string>("getdvar", new Parameter[]
            {
                "mapname"
            });
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
                    Utilities.RawSayTo(player, "^7Console: ^2You need do login to use setspawn");
                    return BaseScript.EventEat.EatGame;
                }
            }
            return EventEat.EatNone;
        }
    }
}