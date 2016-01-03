using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameUI_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameData.GameLevelState m_GameLevelState;

        public MainWindow()
        {
            InitializeComponent();

            m_GameLevelState = new GameData.GameLevelState();
            m_GameLevelState.InitializeNewLevel_Default();

            m_GameLevelState.m_BoardProcessing.OutputBoardStateToDebugTrace(m_GameLevelState.m_BoardState);

            //List<GameDataInterop.GameState.BoardGemMatch> oBoardGemMatchList = m_GameLevelState.m_BoardProcessing.FindAllMatches(m_GameLevelState.m_BoardState);
        }
    }
}
