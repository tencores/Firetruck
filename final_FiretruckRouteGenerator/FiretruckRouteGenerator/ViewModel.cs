using FiretruckRouteGenerator.Annotations;
using FiretruckRouteGenerator.Model;
using FiretruckRouteGenerator.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FiretruckRouteGenerator
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly OutputWriter _writer = new OutputWriter();

        private readonly List<List<string>> _caseToRoutes = new List<List<string>>();
        private readonly List<int> _caseToFireLocation = new List<int>();

        public BindingList<int> Cases => new BindingList<int>(Enumerable.Range(1, _caseToRoutes.Count).ToList());

        private int _selectedCase;
        public int SelectedCase
        {
            get => _selectedCase;
            set
            {
                _selectedCase = value;

                Routes.Clear();
                foreach (var r in _caseToRoutes[value - 1])
                    Routes.Add(r);

                OnPropertyChanged("TotalRoutes");
            }
        }

        public BindingList<string> Routes { get; } = new BindingList<string>();
        public int TotalRoutes => Routes.Count;

        public ViewModel()
        {
            GenerateCases();
        }

        public void GenerateCases(string configPath = null)
        {
            try
            {
                var maps = MapReader.ReadMaps(configPath);

                _caseToRoutes.Clear();
                _caseToFireLocation.Clear();

                foreach (var (map, i) in maps.Select((m, i) => (m, i)))
                {
                    _caseToFireLocation.Add(map.FireLocation);

                    var routes = new List<List<int>>();

                    FindRoutes(map, new List<int> { Map.FIRESTATION_LOCATION }, new bool[map.NumberOfCorners], routes);

                    _caseToRoutes.Add(new List<string>(routes.Select(r => string.Join("", r.Select(c => $"{c,-4}")))));
                }

                if (_caseToRoutes.Count > 0) SelectedCase = 1;
                OnPropertyChanged("Cases");

                SaveLog();
            }
            catch (MapConfigFormatException)
            {
                // In case the error is thrown while parsing the default file
                // just do not load anything else 
                if (configPath is null) return;
                throw;
            }
        }

        // Since the whole task is a 'find path in graph' task, the following code is just the Depth-First Search algorithm
        private void FindRoutes(Map map, List<int> route, IList<bool> visited, ICollection<List<int>> foundRoutes)
        {
            var v = route.Last();

            if (v == map.FireLocation)
            {
                foundRoutes.Add(route);
                return;
            }

            visited[v - 1] = true;

            foreach (var u in map[v].Where(u => !visited[u - 1]))
            {
                FindRoutes(map, new List<int>(route) { u }, visited, foundRoutes);
            }

            visited[v - 1] = false;
        }

        private const int SINGULAR_NUMBER = 1;
        public void SaveLog(string logFilePath = null)
        {
            var logWriter = logFilePath is null ? _writer : new OutputWriter(logFilePath, false);
            logWriter.PrintStartSession();

            foreach (var (routes, i) in _caseToRoutes.Select((c, i) => (c, i)))
            {
                logWriter.WriteLine($"CASE {i + 1}:");

                foreach (var r in routes)
                {
                    logWriter.WriteLine(r);
                }

                var singular = routes.Count == SINGULAR_NUMBER;
                logWriter.WriteLine($"There {(singular ? "is" : "are")} {routes.Count} route{(singular ? "" : "s")} from firestation to streetcorner {_caseToFireLocation[i]}.");
            }

            logWriter.PrintEndSession();
            if (!(logFilePath is null)) logWriter.Dispose();
        }

        public void Close() => _writer.Dispose();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}