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
        Gem_White,
        Gem_Black,
        Gem_Health,
        Token_Blocker,
        Token_Poison,
        OutOfBounds,
    }

    public class CellIndex
    {
        public int m_nRow;
        public int m_nColumn;
    }

    public class BoardElement
    {
        public CellIndex m_CellIndex;
        public BoardElementBaseType m_eBoardElementBaseType;
    }

    public class BoardState
    {
        public BoardElement[,] m_oBoardElementArray;

        public void InitializeEmptyBoard_DefaultSize()
        {
            m_oBoardElementArray = new BoardElement[6, 5];
            for (var nColumn = m_oBoardElementArray.GetLowerBound(0); nColumn <= m_oBoardElementArray.GetUpperBound(0); ++nColumn)
            {
                for (var nRow = m_oBoardElementArray.GetLowerBound(1); nRow <= m_oBoardElementArray.GetUpperBound(1); ++nRow)
                {
                    m_oBoardElementArray[nColumn, nRow].m_CellIndex = new CellIndex { m_nColumn = nColumn, m_nRow = nRow };
                }
            }
        }

        public BoardElement GetBoardElementByCellIndex(CellIndex oCellIndex)
        {
        }
    }
}
