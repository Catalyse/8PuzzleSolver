using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace _8PuzzleSolver
{
    public class Program
    {
        public struct PuzzleStateTarget
        {
            public int n;
            public List<List<int>> start;
            public List<List<int>> goal;
        }

        public struct PuzzleState
        {
            public PuzzleState(List<List<int>> list, ValidMoves last)
            {
                state = new List<List<int>>(list);
                moved = last;
                this.path = new List<int>();
            }

            public PuzzleState(PuzzleState state)
            {
                this.state = new List<List<int>>(state.state);
                this.moved = state.moved;
                this.path = new List<int>();
            }

            public ValidMoves moved;
            public List<List<int>> state;
            public List<int> path;//0 = up// 1 = down// 2 = left// 3 = right
        }

        public static PuzzleState DeepCopy(PuzzleState state)
        {
            string deep = JsonConvert.SerializeObject(state);
            PuzzleState temp = JsonConvert.DeserializeObject<PuzzleState>(deep);
            temp.moved = new ValidMoves(false, false, false, false);
            return temp;
        }

        public struct ValidMoves
        {
            public ValidMoves(ValidMoves state)
            {
                up = state.up;
                down = state.down;
                left = state.left;
                right = state.right;
            }

            public ValidMoves(bool a, bool b, bool c, bool d)
            {
                up = a;
                down = b;
                left = c;
                right = d;
            }

            public bool up;
            public bool down;
            public bool left;
            public bool right;
        }

        public static PuzzleStateTarget target;
        public static List<PuzzleState> stateDatabase = new List<PuzzleState>();
        public static List<PuzzleState> queue = new List<PuzzleState>();
        public static List<PuzzleState> queueTemp = new List<PuzzleState>();
        public static int repetitions = 0;
        public const int BOUND = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("N Puzzle Solver Program");
            Console.WriteLine("By: Taylor May");
            while (true)
            {//input loop to get start information
                Console.WriteLine("Please enter the initial state json file name:");
                string filename;
                filename = Console.ReadLine();
                try
                {
                    target = ReadJson(System.IO.File.ReadAllText(filename));
                }
                catch { }
                Console.WriteLine("Reading File...");
                if(target.start != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid File Provided try again!");
                }
            }
            //now we solve
            Console.WriteLine("Starting Board:");
            DrawBoard(target.start);
            Console.WriteLine("");
            Console.WriteLine("Goal Board:");
            DrawBoard(target.goal);
            Console.WriteLine("");

            BackTracking1();

            Console.WriteLine("Backtracking: Depth First");
            Console.WriteLine("Puzzle Answer found after " + repetitions + " state checks.");
            Console.WriteLine("Number of moves to solve puzzle: " + stateDatabase.Count);
            Console.WriteLine("");
            Console.WriteLine("Instructions to solve:");
            DrawInstructions();
            Console.WriteLine("");
            Console.WriteLine("");

            GraphSearch();
            Console.ReadKey();
        }

        public static void BackTracking1()
        {
            PuzzleState startingState = new PuzzleState(target.start, new ValidMoves(false,false,false,false));
            stateDatabase.Clear();
            stateDatabase.Add(startingState);
            while (true)
            {
                if (stateDatabase.Count > BOUND)
                    stateDatabase.RemoveAt(stateDatabase.Count - 1);
                if(BackTrackRecursion())
                {
                    break;
                }
            }
        }

        public static bool BackTrackRecursion()//This only returns true if we found the answer
        {
            PuzzleState state = stateDatabase[stateDatabase.Count - 1];
            ValidMoves moves = CheckPossibleMoves(state.state);
            if (!state.moved.up && moves.up)
            {
                repetitions++;
                PuzzleState newState = MovePiece(state, 0);
                if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                {
                    stateDatabase.Add(newState);
                    return true;
                }
                else if (CheckIfStateExists(newState))//need to check next possible move
                {
                    state.moved.up = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                }
                else
                {
                    state.moved.up = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                    stateDatabase.Add(newState);
                    return false;
                }
            }
            if(!state.moved.left && moves.left)
            {
                repetitions++;
                PuzzleState newState = MovePiece(state, 2);
                if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                {
                    stateDatabase.Add(newState);
                    return true;
                }
                else if (CheckIfStateExists(newState))//need to check next possible move
                {
                    state.moved.left = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                }
                else
                {
                    state.moved.left = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                    stateDatabase.Add(newState);
                    return false;
                }
            }
            if (!state.moved.down && moves.down)
            {
                repetitions++;
                PuzzleState newState = MovePiece(state, 1);
                if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                {
                    stateDatabase.Add(newState);
                    return true;
                }
                else if (CheckIfStateExists(newState))//need to check next possible move
                {
                    state.moved.down = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                }
                else
                {
                    state.moved.down = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                    stateDatabase.Add(newState);
                    return false;
                }
            }
            if (!state.moved.right && moves.right)
            {
                repetitions++;
                PuzzleState newState = MovePiece(state, 3);
                if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                {
                    stateDatabase.Add(newState);
                    return true;
                }
                else if (CheckIfStateExists(newState))//need to check next possible move
                {
                    state.moved.right = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                }
                else
                {
                    state.moved.right = true;
                    stateDatabase[stateDatabase.Count - 1] = state;
                    stateDatabase.Add(newState);
                    return false;
                }
            }
            stateDatabase.RemoveAt(stateDatabase.Count - 1);
            return false;
        }

        public static void GraphSearch()
        {
            PuzzleState startingState = new PuzzleState(target.start, new ValidMoves(false, false, false, false));
            startingState.path.Add(-1);
            queue.Add(startingState);
            repetitions = 0;
            
            PuzzleState returnVal = GraphSearchRecursion();

            Console.WriteLine("Graph Search: Breadth First");
            Console.WriteLine("Puzzle Answer found after " + repetitions + " state checks.");
            Console.WriteLine("Number of moves to solve puzzle: " + (returnVal.path.Count-1));
            Console.WriteLine("");
            Console.WriteLine("Instructions to solve:");
            DrawInstructions(returnVal);
        }

        public static PuzzleState GraphSearchRecursion()
        {
            while (true)
            {
                int count = queue.Count;
                for (int i = 0; i < count; i++)
                {
                    PuzzleState state = queue[i];
                    ValidMoves moves = CheckPossibleMoves(state.state);
                    if (state.path[state.path.Count-1] != 1 && moves.up)
                    {
                        repetitions++;
                        PuzzleState newState = MovePiece(state, 0);
                        if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                        {
                            newState.path.Add(0);
                            return newState;//found the answer
                        }
                        else
                        {//update and add to queue
                            newState.path.Add(0);
                            queueTemp.Add(newState);
                        }
                    }
                    if (state.path[state.path.Count - 1] != 3 && moves.left)
                    {
                        repetitions++;
                        PuzzleState newState = MovePiece(state, 2);
                        if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                        {
                            newState.path.Add(2);
                            return newState;//found the answer
                        }
                        else
                        {//update and add to queue
                            newState.path.Add(2);
                            queueTemp.Add(newState);
                        }
                    }
                    if (state.path[state.path.Count - 1] != 0 && moves.down)
                    {
                        repetitions++;
                        PuzzleState newState = MovePiece(state, 1);
                        if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                        {
                            newState.path.Add(1);
                            return newState;//found the answer
                        }
                        else
                        {//update and add to queue
                            newState.path.Add(1);
                            queueTemp.Add(newState);
                        }
                    }
                    if (state.path[state.path.Count - 1] != 2 && moves.right)
                    {
                        repetitions++;
                        PuzzleState newState = MovePiece(state, 3);
                        if (CheckTwoStates(newState.state, target.goal, state.state.Count))
                        {
                            newState.path.Add(3);
                            return newState;//found the answer
                        }
                        else
                        {//update and add to queue
                            newState.path.Add(3);
                            queueTemp.Add(newState);
                        }
                    }
                }
                queue.Clear();
                queue = new List<PuzzleState>(queueTemp);
                queueTemp.Clear();
            }
        }

        #region CheckPreviousStateExistence
        private static bool CheckIfStateExists(PuzzleState state)
        {
            int n = state.state.Count;
            for (int x = stateDatabase.Count - 1; x >= 0; x--)
            {
                if (CheckTwoStates(state.state, stateDatabase[x].state, n))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckTwoStates(List<List<int>> state1, List<List<int>> state2, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < n; k++)
                {
                    if (state1[i][k] != state2[i][k])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion CheckPreviousStateExistence

        private static PuzzleState MovePiece(PuzzleState state, int move)
        {
            int x = -1, y = -1;
            //find zero
            x = FindZero(state.state, out y);
            PuzzleState newState = DeepCopy(state);

            if (move == 0)//up
            {
                int movePiece = state.state[y - 1][x];
                newState.state[y - 1][x] = 0;
                newState.state[y][x] = movePiece;
                return newState;
            }
            else if (move == 1)//down
            {
                int movePiece = state.state[y + 1][x];
                newState.state[y + 1][x] = 0;
                newState.state[y][x] = movePiece;
                return newState;
            }
            else if (move == 2)//left
            {
                int movePiece = state.state[y][x - 1];
                newState.state[y][x - 1] = 0;
                newState.state[y][x] = movePiece;
                return newState;
            }
            else//right
            {
                int movePiece = state.state[y][x + 1];
                newState.state[y][x + 1] = 0;
                newState.state[y][x] = movePiece;
                return newState;
            }
        }

        public static int FindZero(List<List<int>> board, out int y)
        {
            int n = board.Count;
            //find zero
            for (int i = 0; i < board.Count; i++)
            {
                for (int k = 0; k < board[i].Count; k++)
                {
                    if (board[i][k] == 0)
                    {
                        y = i;
                        return k;
                    }
                }
            }
            y = -1;
            return -1;//this bad tho
        }

        private static ValidMoves CheckPossibleMoves(List<List<int>> board)
        {
            int n = board.Count;
            int x = -1, y = -1;
            //find zero
            x = FindZero(board, out y);

            ValidMoves moves;

            if (y == 0)
                moves.up = false;
            else
                moves.up = true;
            if (y == n-1)
                moves.down = false;
            else
                moves.down = true;
            if (x == 0)
                moves.left = false;
            else
                moves.left = true;
            if (x == n-1)
                moves.right = false;
            else
                moves.right = true;

            return moves;
        }

        private static PuzzleStateTarget ReadJson(string read)
        {
            try
            {
                PuzzleStateTarget state = JsonConvert.DeserializeObject<PuzzleStateTarget>(read);
                return state;
            }
            catch
            {
                return new PuzzleStateTarget();
            }
        }

        private static void DrawBoard(List<List<int>> board)
        {
            for(int i = 0; i < board.Count; i ++)
            {
                for(int k = 0; k < board[i].Count; k++)
                {
                    Console.Write(board[i][k]);
                }
                Console.Write("\n");
            }
        }

        private static void DrawInstructions()
        {
            for(int i = 0; i < stateDatabase.Count; i ++)
            {//right//down//left//up
                if (stateDatabase[i].moved.right)
                    Console.Write("Right, ");
                else if (stateDatabase[i].moved.down)
                    Console.Write("Down, ");
                else if (stateDatabase[i].moved.left)
                    Console.Write("Left, ");
                else if (stateDatabase[i].moved.up)
                    Console.Write("Up, ");
            }
        }

        private static void DrawInstructions(PuzzleState state)
        {
            for (int i = 0; i < state.path.Count; i++)
            {//right//down//left//up
                if (state.path[i] == 3)
                    Console.Write("Right, ");
                else if (state.path[i] == 1)
                    Console.Write("Down, ");
                else if (state.path[i] == 2)
                    Console.Write("Left, ");
                else if (state.path[i] == 0)
                    Console.Write("Up, ");
            }
        }
    }
}
