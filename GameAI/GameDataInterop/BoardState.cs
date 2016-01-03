using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataInterop
{
    public enum BoardElementBaseType
    {
        Unknown,
        Gem_Red,
        Gem_Blue,
        Gem_Green,
        Gem_Light,
        Gem_Dark,
        Gem_Health,
        Token_Blocker,
        Token_Poison,
        Transitory_Empty,
        OutOfBounds,
    }

    public enum Alignment_HorizontalAndVertical
    {
        Horizontal,
        Vertical,
    }

    public enum Direction_HorizontalAndVertical
    {
        Up,
        Down,
        Left,
        Right,
    }

    public class CellIndex
    {
        public int m_nRow;
        public int m_nColumn;

        public int GetCompositeIdentifier()
        {
            System.Diagnostics.Debug.Assert(m_nRow >= 0);
            System.Diagnostics.Debug.Assert(m_nRow < 32000);
            System.Diagnostics.Debug.Assert(m_nColumn >= 0);
            System.Diagnostics.Debug.Assert(m_nColumn < 32000);
            return (m_nRow << 16) + m_nColumn;
        }

        public CellIndex GetNeighboringCellIndex(Direction_HorizontalAndVertical eDirection, int nOffset = 1)
        {
            switch (eDirection)
            {
                case Direction_HorizontalAndVertical.Up:
                    return new CellIndex { m_nColumn = m_nColumn, m_nRow = (m_nRow - nOffset) };
                case Direction_HorizontalAndVertical.Down:
                    return new CellIndex { m_nColumn = m_nColumn, m_nRow = (m_nRow + nOffset) };
                case Direction_HorizontalAndVertical.Left:
                    return new CellIndex { m_nColumn = (m_nColumn - nOffset), m_nRow = m_nRow };
                case Direction_HorizontalAndVertical.Right:
                    return new CellIndex { m_nColumn = (m_nColumn + nOffset), m_nRow = m_nRow };
                default:
                    throw new Exception("Case not handled in switch.");
            }
        }
    }

    public class BoardElement
    {
        public CellIndex m_CellIndex;
        public BoardElementBaseType m_eBoardElementBaseType;

        public bool IsValidNonTransientToken()
        {
            switch (m_eBoardElementBaseType)
            {
                case BoardElementBaseType.Gem_Red:
                case BoardElementBaseType.Gem_Blue:
                case BoardElementBaseType.Gem_Green:
                case BoardElementBaseType.Gem_Light:
                case BoardElementBaseType.Gem_Dark:
                case BoardElementBaseType.Gem_Health:
                case BoardElementBaseType.Token_Blocker:
                case BoardElementBaseType.Token_Poison:
                    return true;
                default:
                    return false;
            }
        }
        public bool IsOutOfBounds()
        {
            return (m_eBoardElementBaseType == BoardElementBaseType.OutOfBounds);
        }
        public bool IsEmpty()
        {
            return (m_eBoardElementBaseType == BoardElementBaseType.Transitory_Empty);
        }
    }

    public class BoardState
    {
        protected static BoardElement m_oBoardElement_OutOfBounds = new BoardElement
        {
            m_CellIndex = new CellIndex { m_nColumn = -1, m_nRow = -1 },
            m_eBoardElementBaseType = BoardElementBaseType.OutOfBounds,
        };

        public BoardElement[,] m_oBoardElementArray;

        public void InitializeEmptyBoard_DefaultSize()
        {
            m_oBoardElementArray = new BoardElement[6, 5];
            for (var nColumn = m_oBoardElementArray.GetLowerBound(0); nColumn <= m_oBoardElementArray.GetUpperBound(0); ++nColumn)
            {
                for (var nRow = m_oBoardElementArray.GetLowerBound(1); nRow <= m_oBoardElementArray.GetUpperBound(1); ++nRow)
                {
                    m_oBoardElementArray[nColumn, nRow] = new BoardElement
                    {
                        m_CellIndex = new CellIndex { m_nColumn = nColumn, m_nRow = nRow },
                        m_eBoardElementBaseType = BoardElementBaseType.Transitory_Empty,
                    };
                }
            }
        }

        public int GetNumColumns()
        {
            return m_oBoardElementArray.GetUpperBound(0) + 1;
        }
        public int GetNumRows()
        {
            return m_oBoardElementArray.GetUpperBound(1) + 1;
        }

        public BoardElement GetBoardElementByCellIndex(CellIndex oCellIndex)
        {
            if (oCellIndex.m_nColumn < m_oBoardElementArray.GetLowerBound(0))
            {
                return m_oBoardElement_OutOfBounds;
            }
            if (oCellIndex.m_nColumn > m_oBoardElementArray.GetUpperBound(0))
            {
                return m_oBoardElement_OutOfBounds;
            }
            if (oCellIndex.m_nRow < m_oBoardElementArray.GetLowerBound(1))
            {
                return m_oBoardElement_OutOfBounds;
            }
            if (oCellIndex.m_nRow > m_oBoardElementArray.GetUpperBound(1))
            {
                return m_oBoardElement_OutOfBounds;
            }

            return m_oBoardElementArray[oCellIndex.m_nColumn, oCellIndex.m_nRow];
        }
    }
}
