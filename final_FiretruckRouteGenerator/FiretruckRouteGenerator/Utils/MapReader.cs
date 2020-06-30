using FiretruckRouteGenerator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiretruckRouteGenerator.Utils
{
    public class MapConfigFormatException : Exception
    {
        public MapConfigFormatException(string message = "Invalid config file format") : base(message) { }
    }

    public class MapReader
    {
        private static readonly string CONFIG_FILE = @"Resources\Config.txt";
        private static readonly string DEFAULT_PATH = Path.Combine(Environment.CurrentDirectory, CONFIG_FILE);

        private static readonly string END_CASE = "0 0";
        private static readonly int MINIMUM_CORNER = 1;
        private static readonly int MAXIMUM_CORNER = 20;
        private static readonly char CORNERS_SEPARATOR = ' ';

        public static List<Map> ReadMaps(string pathToConfig = null)
        {
            pathToConfig = pathToConfig ?? DEFAULT_PATH;

            var result = new List<Map>();

            try
            {
                using (var r = new StreamReader(pathToConfig))
                {
                    while (!r.EndOfStream)
                    {
                        result.Add(ReadMap(r));
                    }
                }

                return result;
            }
            catch
            {
                throw new MapConfigFormatException();
            }
        }

        private static Map ReadMap(StreamReader r)
        {
            var fire = int.Parse(r.ReadLine());
            var streets = new List<AdjCorners>();

            while (!r.EndOfStream)
            {
                var line = r.ReadLine().Trim();

                if (END_CASE.Equals(line)) break;

                var corners = line
                    .Split(CORNERS_SEPARATOR)
                    .Select(int.Parse)
                    .ToArray();

                if (corners.Any(i => i < MINIMUM_CORNER || i > MAXIMUM_CORNER))
                    throw new MapConfigFormatException();

                streets.Add(new AdjCorners { Src = corners[0], Dst = corners[1] });
            }

            return new Map(fire, streets);
        }
    }
}