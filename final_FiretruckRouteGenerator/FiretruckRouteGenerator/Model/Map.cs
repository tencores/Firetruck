using System.Collections.Generic;

namespace FiretruckRouteGenerator.Model
{
    public class AdjCorners
    {
        public int Src { get; set; }
        public int Dst { get; set; }
    }

    public class Map
    {
        public const int FIRESTATION_LOCATION = 1;

        /// <summary>
        /// Adjacency List
        /// Key: Streetcorner
        /// Value: All the streetcorners that are reachable from the key streetcorner
        ///
        /// We need Dictionary, not List, because in a config file some corners may be missed if there are no open streets through those corners
        /// So, we cannot use list index as a corner
        /// </summary>
        private readonly Dictionary<int, List<int>> _map; //each street  corner and the adj vertives it has makes up the map

        public int FireLocation { get;  }
        public int NumberOfCorners { get;  }

        public Map(int fireLocation, IEnumerable<AdjCorners> adjCorners)
        {
            FireLocation = fireLocation;

            var maxCorner = 0;
            foreach (var c in adjCorners)
            {
                maxCorner = c.Src > maxCorner ? c.Src : maxCorner;
                maxCorner = c.Dst > maxCorner ? c.Dst : maxCorner;
            }
            NumberOfCorners = maxCorner;

            _map = new Dictionary<int, List<int>>();
            foreach (var e in adjCorners)
            {
                if (!_map.ContainsKey(e.Src)) _map.Add(e.Src, new List<int>());
                if (!_map.ContainsKey(e.Dst)) _map.Add(e.Dst, new List<int>());

                _map[e.Src].Add(e.Dst);
                _map[e.Dst].Add(e.Src);
            }
        }

        public IList<int> this[int i] => _map[i];
    }
}