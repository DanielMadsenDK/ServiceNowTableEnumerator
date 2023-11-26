namespace SN.WidEnum;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Table : INotifyPropertyChanged
{

    private bool? _scanResult;

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Name { get; set; }
    public bool? ScanResult
    {
        get { return _scanResult; }
        set
        {
            if (value != null)
            {
                _scanResult = value;
                NotifyPropertyChanged();
            }
        }
    }
}