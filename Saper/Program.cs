using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper
{
    class Program
    {
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        static int cnt_mine;
        static string[,] GenerateField(int N, int M)
        {
            cnt_mine = 0;
            Random rand = new Random();
            string[,] Field = new string[N, M];
            for (int i = 0; i<N; i++)
            {
                for (int j = 0; j<M; j++)
                {
                    int k = rand.Next(0, 3);
                    if (k == 0) //0 == мина
                    {
                        Field[i, j] = "*"; //0,33 - вероятность того, что мина
                        cnt_mine++;
                    }
                    else
                    {
                        Field[i, j] = "x"; //0,67 - вероятность того, что не мина
                    }
                }
            }
            return Field;
        }

        const string UNDERLINE = "\x1B[4m";
        const string RESET = "\x1B[0m";
        static string[,] GenerateOutPut(int N, int M)
        {
            string[,] OutPut = new string[N + 1, M + 1];
            for (int i = 0; i <= N; i++)
            {
                for (int j = 0; j <= M; j++)
                {
                    if (i == 0 && j != 0)
                    {
                        if (j == 1)
                            OutPut[i, j] += "   " + (j - 1).ToString();
                        else
                            OutPut[i, j] += "  " + (j - 1).ToString();

                    }
                    else if (i != 0 && j != 0)
                    {
                        OutPut[i, j] += UNDERLINE + "  " + RESET;
                    }

                    if (j == 0 && i != 0)
                    {
                        OutPut[i, j] += (i - 1).ToString() + " |";
                    }
                    else if (i != 0 && j != 0)
                    {
                        OutPut[i, j] += "|";
                    }
                }
            }
            //первоначальный вывод
            for (int i = 0; i <= N; i++)
            {
                for (int j = 0; j <= M; j++)
                {
                    Console.Write(OutPut[i, j]);
                }
                Console.WriteLine();
            }
            return OutPut;
        }

        static void Main(string[] args)
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);

            Console.WriteLine("Введите размер поля: ");
            Console.Write("(N, M) = ");
            int[] ints0 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToArray<int>();
            int N = ints0[0];
            int M = ints0[1];
            Console.Clear();
            string[,] OutPut = GenerateOutPut(N, M);
            string[,] Field = GenerateField(N, M);


            bool Game = true; //пока True игра продолжается
            string NG = "";
            int cnt = 0;
            int cnt_pairs = 0;
            while (Game)
            {
                int[] ints = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToArray<int>();
                int I = ints[0];
                int J = ints[1];
                cnt_pairs++;
                Console.Clear();
                if (Field[I, J] == "*") //игра окончена
                {
                    Game = false;
                    for (int i = 0; i<N; i++)
                    {
                        for (int j = 0; j<M; j++)
                        {
                            if (Field[i, j] == "*")
                            {
                                OutPut[i + 1, j + 1] = UNDERLINE + "* |" + RESET;
                            }
                        }
                    }
                }
                else //подсчитаем количество мин по соседству
                {
                    cnt = 0;
                    if (I > 0)
                    {
                        if (Field[I-1,J] == "*")
                        {
                            cnt++;
                        }
                    }
                    if (J > 0)
                    {
                        if (Field[I, J-1] == "*")
                        {
                            cnt++;
                        }
                    }
                    if (I < N - 1)
                    {
                        if (Field[I+1,J] == "*")
                        {
                            cnt++;
                        }
                    }
                    if (J < M - 1)
                    {
                        if (Field[I,J+1] == "*")
                        {
                            cnt++;
                        }

                    }
                    if (I > 0 && J > 0)
                    {
                        if (Field[I-1, J-1] == "*")
                        {
                            cnt++;
                        }

                    }
                    if (I > 0 && J < M - 1)
                    {
                        if (Field[I-1, J+1] == "*")
                        {
                            cnt++;
                        }

                    }
                    if (I < N-1 && J > 0)
                    {
                        if (Field[I+1, J-1] == "*")
                        {
                            cnt++;
                        }

                    }
                    if (I < N-1 && J < M-1)
                    {
                        if (Field[I+1, J+1] == "*")
                        {
                            cnt++;
                        }

                    }
                    OutPut[I + 1, J + 1] = UNDERLINE + cnt.ToString()+" |" + RESET;
                }

                for (int i = 0; i <= N; i++)
                {
                    for (int j = 0; j <= M; j++)
                    {
                        Console.Write(OutPut[i, j]);
                    }
                    Console.WriteLine();
                }
                if (!Game)
                {
                    Console.WriteLine("Game Over. Do you want Restart? Y/N ");
                    NG = Console.ReadLine();
                    cnt_pairs = 0;
                }
                if (NG == "Y")
                {
                    Game = true;
                    Console.Clear();
                    Console.WriteLine("Введите размер поля: ");
                    Console.Write("(N, M) = ");
                    ints0 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToArray<int>();
                    N = ints0[0];
                    M = ints0[1];
                    OutPut = GenerateOutPut(N, M);
                    Field = GenerateField(N, M);
                }
                NG = "";
                if (cnt_pairs+cnt_mine == N*M)
                {
                    Game = false;
                    Console.WriteLine("You Won!");
                }
            }
        }
    }
}
