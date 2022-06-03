namespace DbManipulationApp.Models
{
    public class SlownikDniViewModel
    {
        private string? _dzien_liturgiczny;
            private IEnumerable<SlownikDni> _slownikDni_list = Enumerable.Empty<SlownikDni>();
            public string Dzien_liturgiczny
            {
                get { return _dzien_liturgiczny; }
                set { _dzien_liturgiczny = value; }
            }
            public IEnumerable<SlownikDni> SlownikDni_list
            {
                get { return _slownikDni_list; }
                set { _slownikDni_list = value; }
            }
        }
    }

