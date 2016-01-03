using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataInterop.GameState
{
    public enum EDirection
    {
        Unknown,
        Up,
        Down,
        Left,
        Right,
        Diagonal_UpLeft,
        Diagonal_UpRight,
        Diagonal_DownLeft,
        Diagonal_DownRight,
    }

    public class BoardMoveStep
    {
        // Note: Can be NULL; only used for checking logical correctness in sequence
        public CellIndex m_StartCellIndex;

        public EDirection m_eDirection;
    }

    public class BoardMoveSequence
    {
        public CellIndex m_StartCellIndex;
        public List<BoardMoveStep> m_oMoveStepList = new List<BoardMoveStep>();
    }
}
