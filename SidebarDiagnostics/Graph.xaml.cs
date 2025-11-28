using System.Windows.Input;
using SidebarDiagnostics.Models;
using SidebarDiagnostics.Windows;
using System.ComponentModel;
using SidebarDiagnostics.Style;

namespace SidebarDiagnostics
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : FlatWindow
    {
        public Graph(Sidebar sidebar)
        {
            InitializeComponent();

            DataContext = Model = new GraphModel();
            Model.BindData(sidebar.Model.MonitorManager);
            
            Show();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Model?.PlotModel?.ResetAllAxes();
                OPGraph.InvalidatePlot(true);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DataContext = null;

            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }
        }

        public GraphModel Model { get; private set; }
    }
}
