using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    public class MoveProcessing
    {
        public GameLevelState m_GameLevelState;

        public GameDataInterop.GameState.BoardMoveResult ExecuteBoardMove(GameDataInterop.BoardState oBoardState, GameDataInterop.GameState.BoardMoveSequence oBoardMoveSequence)
        {
            throw new NotImplementedException();
        }

        public void ExecuteBoardCascade(GameDataInterop.BoardState oBoardState)
        {
            ExecuteBoardCascade_Consolidate(oBoardState);
            ExecuteBoardCascade_Skyfall(oBoardState);
        }

        public void ExecuteBoardCascade_Consolidate(GameDataInterop.BoardState oBoardState)
        {
            // For each column, "move" the populated cells "down" to fill the empty cells
            for (var nColumn = oBoardState.m_oBoardElementArray.GetLowerBound(0); nColumn <= oBoardState.m_oBoardElementArray.GetUpperBound(0); ++nColumn)
            {
                int nCurrentShiftRequired = 0;
                for (var nRow = oBoardState.m_oBoardElementArray.GetUpperBound(1); nRow <= oBoardState.m_oBoardElementArray.GetLowerBound(1); --nRow)
                {
                    GameDataInterop.BoardElement oCurrentCell = oBoardState.m_oBoardElementArray[nColumn, nRow];

                    bool bCellIsEmpty = false;
                    switch (oCurrentCell.m_eBoardElementBaseType)
                    {
                        case GameDataInterop.BoardElementBaseType.Transitory_Empty:
                            bCellIsEmpty = true;
                            break;
                        case GameDataInterop.BoardElementBaseType.Unknown:
                        case GameDataInterop.BoardElementBaseType.OutOfBounds:
                            throw new ApplicationException("Invalid cell state detected.");
                        default:
                            // Something [valid] in cell
                            break;
                    }

                    if (bCellIsEmpty)
                    {
                        ++nCurrentShiftRequired;
                        continue;
                    }

                    if (nCurrentShiftRequired > 0)
                    {
                        // "move" the current cell down
                        oCurrentCell.m_CellIndex.m_nRow += nCurrentShiftRequired;
                        oBoardState.m_oBoardElementArray[nColumn, oCurrentCell.m_CellIndex.m_nRow] = oCurrentCell;
                        oBoardState.m_oBoardElementArray[nColumn, nRow] = new GameDataInterop.BoardElement
                        {
                            m_CellIndex = new GameDataInterop.CellIndex { m_nColumn = nColumn, m_nRow = nRow },
                            m_eBoardElementBaseType = GameDataInterop.BoardElementBaseType.Transitory_Empty,
                        };
                    }
                }

                // Note: If we get to the "top" of a column and nCurrentShiftRequired is positive, that's okay
            }
        }

        public void ExecuteBoardCascade_Skyfall(GameDataInterop.BoardState oBoardState)
        {
            if (m_GameLevelState == null)
            {
                throw new InvalidOperationException("GameLevelState must be set.");
            }

            // This is "easy" with no animation... just populate empty cells
            for (var nColumn = oBoardState.m_oBoardElementArray.GetLowerBound(0); nColumn <= oBoardState.m_oBoardElementArray.GetUpperBound(0); ++nColumn)
            {
                for (var nRow = oBoardState.m_oBoardElementArray.GetUpperBound(1); nRow >= oBoardState.m_oBoardElementArray.GetLowerBound(1); --nRow)
                {
                    GameDataInterop.BoardElement oCurrentCell = oBoardState.m_oBoardElementArray[nColumn, nRow];

                    if (!oCurrentCell.IsEmpty())
                    {
                        continue;
                    }

                    GameDataInterop.BoardElement oNewCell = m_GameLevelState.GenerateNewBoardElement_Skyfall_Default(oCurrentCell.m_CellIndex);
                    System.Diagnostics.Debug.Assert(oNewCell != null);

                    oBoardState.m_oBoardElementArray[nColumn, nRow] = oNewCell;
                }
            }
        }
    }
}
