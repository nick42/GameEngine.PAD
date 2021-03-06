﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    public class MoveProcessing
    {
        public GameLevelState m_GameLevelState;

        public GameDataInterop.GameState.BoardMoveSequence Debug_GenRandomMoveSequence(int nNumMoves)
        {
            VLRTech.Util.Checks.Runtime.AssertThrow(nNumMoves >= 0);

            Random oRandomGenerator = new Random();

            GameDataInterop.GameState.BoardMoveSequence oBoardMoveSequence = new GameDataInterop.GameState.BoardMoveSequence();
            GameDataInterop.CellIndex oStartCellIndex = new GameDataInterop.CellIndex { m_nColumn = oRandomGenerator.Next(0, m_GameLevelState.m_BoardState.GetNumColumns()), m_nRow = oRandomGenerator.Next(0, m_GameLevelState.m_BoardState.GetNumRows()) };

            oBoardMoveSequence.m_StartCellIndex = oStartCellIndex;

            Array oSwapDirectionArray = Enum.GetValues(typeof(GameDataInterop.GameState.EDirection));
            for (int i=0; i < nNumMoves; i++)
            {
                GameDataInterop.BoardElement oBoardElement_Start = m_GameLevelState.m_BoardState.GetBoardElementByCellIndex(oStartCellIndex);
                VLRTech.Util.Checks.Runtime.AssertThrow(oBoardElement_Start.IsValidNonTransientToken());

                GameDataInterop.GameState.EDirection eSwapDirection = GameDataInterop.GameState.EDirection.Unknown;
                while (true)
                {
                    // Note: Using '1' as lower bound, to skip 'Unknown' enum value
                    GameDataInterop.GameState.EDirection eSwapDirection_Potential = (GameDataInterop.GameState.EDirection)oSwapDirectionArray.GetValue(oRandomGenerator.Next(1, oSwapDirectionArray.GetLength(0)));
                    GameDataInterop.BoardElement oBoardElement_SwapTarget = m_GameLevelState.m_BoardState.GetBoardElementByCellIndex(oBoardElement_Start.m_CellIndex.GetNeighboringCellIndex(eSwapDirection_Potential));
                    if (oBoardElement_SwapTarget.IsOutOfBounds())
                    {
                        continue;
                    }

                    // Found a valid swap
                    eSwapDirection = eSwapDirection_Potential;
                    break;
                }

                GameDataInterop.GameState.BoardMoveStep oMoveStep = new GameDataInterop.GameState.BoardMoveStep
                {
                    m_StartCellIndex = oStartCellIndex,
                    m_eDirection = eSwapDirection,
                };
                oBoardMoveSequence.m_oMoveStepList.Add(oMoveStep);

                // Update start cell index to new move target
                oStartCellIndex = m_GameLevelState.m_BoardState.GetBoardElementByCellIndex(oBoardElement_Start.m_CellIndex.GetNeighboringCellIndex(eSwapDirection)).m_CellIndex;
            }

            return oBoardMoveSequence;
        }

        public void ExecuteBoardMoveStep_BoardUpdateOnly(GameDataInterop.BoardState oBoardState, GameDataInterop.GameState.BoardMoveStep oBoardMoveStep)
        {
            VLRTech.Util.Checks.Runtime.AssertThrow(oBoardMoveStep.m_StartCellIndex != null);
            VLRTech.Util.Checks.Runtime.AssertThrow(oBoardMoveStep.m_eDirection != GameDataInterop.GameState.EDirection.Unknown);

            GameDataInterop.BoardElement oBoardElement_Swap1 = oBoardState.GetBoardElementByCellIndex(oBoardMoveStep.m_StartCellIndex);
            GameDataInterop.BoardElement oBoardElement_Swap2 = oBoardState.GetBoardElementByCellIndex(oBoardMoveStep.m_StartCellIndex.GetNeighboringCellIndex(oBoardMoveStep.m_eDirection));
            VLRTech.Util.Checks.Runtime.AssertThrow(!oBoardElement_Swap1.IsOutOfBounds());
            VLRTech.Util.Checks.Runtime.AssertThrow(!oBoardElement_Swap2.IsOutOfBounds());

            oBoardState.SwapCells(oBoardElement_Swap1, oBoardElement_Swap2);
        }

        public GameDataInterop.GameState.BoardMoveResult ExecuteBoardMoveSequence_Complete(GameDataInterop.BoardState oBoardState, GameDataInterop.GameState.BoardMoveSequence oBoardMoveSequence)
        {
            GameDataInterop.GameState.BoardMoveResult oBoardMoveResult = new GameDataInterop.GameState.BoardMoveResult();

            // TODO: Copy board state, so we can leave unmodified in the event of an error while processing the move

            // Execute the move sequence
            // Check correctness of move sequence as we go

            GameDataInterop.CellIndex oCurrentCellIndex = oBoardMoveSequence.m_StartCellIndex;

            foreach (var oMoveStep in oBoardMoveSequence.m_oMoveStepList)
            {
                VLRTech.Util.Checks.Runtime.AssertThrow(oCurrentCellIndex.Equals(oMoveStep.m_StartCellIndex));

                ExecuteBoardMoveStep_BoardUpdateOnly(oBoardState, oMoveStep);

                oCurrentCellIndex = oCurrentCellIndex.GetNeighboringCellIndex(oMoveStep.m_eDirection);
            }

            // Collect initial matches

            List<GameDataInterop.GameState.BoardGemMatch> oBoardMatchList = m_GameLevelState.m_BoardProcessing.FindAllMatches(oBoardState);

            oBoardMoveResult.m_oBoardMatchList_PreCascade = oBoardMatchList;

            int nCascadeIteration = 0;
            while (true)
            {
                // Initial check, in case we didn't produce any pre-cascade matches
                if (oBoardMatchList.Count() == 0)
                {
                    break;
                }

                ++nCascadeIteration;

                ExecuteBoardClearMatches(oBoardState, oBoardMatchList);
                ExecuteBoardCascade_Consolidate(oBoardState);
                ExecuteBoardCascade_Skyfall(oBoardState);

                oBoardMatchList = m_GameLevelState.m_BoardProcessing.FindAllMatches(oBoardState);
                if (oBoardMatchList.Count() == 0)
                {
                    // We're done
                    break;
                }

                if (oBoardMoveResult.m_oBoardMatchList_PostCascade == null)
                {
                    oBoardMoveResult.m_oBoardMatchList_PostCascade = new List<GameDataInterop.GameState.BoardGemMatch>();
                }

                // Mark all matches in this generation with the applicable iteration number, 
                // and add to results list
                foreach (var oBoardGemMatch in oBoardMatchList)
                {
                    oBoardGemMatch.m_nCascadeIteration = nCascadeIteration;
                    oBoardMoveResult.m_oBoardMatchList_PostCascade.Add(oBoardGemMatch);
                }
            }

            // TODO: Calculate point total

            return oBoardMoveResult;
        }

        public void ExecuteBoardClearMatches(GameDataInterop.BoardState oBoardState, List<GameDataInterop.GameState.BoardGemMatch> oBoardMatchList)
        {
            foreach (var oBoardGemMatch in oBoardMatchList)
            {
                foreach (var oBoardElementInMatch in oBoardGemMatch.m_oMatchElementList)
                {
                    GameDataInterop.BoardElement oBoardElement_FoundInMatch = oBoardState.GetBoardElementByCellIndex(oBoardElementInMatch.m_oBoardElement.m_CellIndex);
                    VLRTech.Util.Checks.Runtime.AssertThrow(oBoardElement_FoundInMatch.IsValidNonTransientToken());

                    oBoardElement_FoundInMatch.m_eBoardElementBaseType = GameDataInterop.BoardElementBaseType.Transitory_Empty;
                }
            }
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
