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

            for (int i=0; i < 100; ++i)
            {
                System.Diagnostics.Debug.WriteLine("Pre match-check board state:");
                m_GameLevelState.m_BoardProcessing.OutputBoardStateToDebugTrace(m_GameLevelState.m_BoardState);

                for (int j=0; j < 10; j++)
                {
                    System.Diagnostics.Debug.WriteLine("Current board state:");
                    m_GameLevelState.m_BoardProcessing.OutputBoardStateToDebugTrace(m_GameLevelState.m_BoardState);

                    List<GameDataInterop.GameState.BoardGemMatch> oBoardGemMatchList = m_GameLevelState.m_BoardProcessing.FindAllMatches(m_GameLevelState.m_BoardState);

                    System.Diagnostics.Debug.WriteLine(String.Format("Found {0} matches during board processing.", oBoardGemMatchList.Count()));

                    var oMoveSequence = m_GameLevelState.m_MoveProcessing.Debug_GenRandomMoveSequence(100);

                    GameDataInterop.GameState.BoardMoveResult oBoardMoveResult = m_GameLevelState.m_MoveProcessing.ExecuteBoardMoveSequence_Complete(m_GameLevelState.m_BoardState, oMoveSequence);

                    System.Diagnostics.Debug.WriteLine(
                        String.Format("Processed random move sequence; {0} matches pre-cascade, {1} post-cascade.", 
                        oBoardMoveResult.m_oBoardMatchList_PreCascade.Count(), 
                        (oBoardMoveResult.m_oBoardMatchList_PostCascade != null) ? oBoardMoveResult.m_oBoardMatchList_PostCascade.Count() : 0 ));
                }
            }
        }
    }
}
