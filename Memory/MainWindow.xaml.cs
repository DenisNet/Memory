using System.Windows;

namespace Memory
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //das Spielfeld erstellen
            new MemoryFeld(spielfeld);
        }
    }
}
