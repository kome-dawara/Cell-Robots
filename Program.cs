namespace MySpace
{
    using ScottPlot;
    using ScottPlot.AxisPanels;
    using ScottPlot.Plottables;
    using System.Linq;

    static class Program
    {
        static Plot mainPlot = new();
        static Plot graphPlot = new();

        static void Main()
        {
            System.Diagnostics.Stopwatch sw = new();

            while (true)
            {
                sw.Reset();

                graphPlot.Clear();
                List<double> aCounts = new();
                Signal aSig = graphPlot.Add.Signal(aCounts);
                aSig.Axes.YAxis = graphPlot.Axes.Left;
                aSig.LineWidth = 5;
                List<double> iCounts = new();
                Signal iSig = graphPlot.Add.Signal(iCounts);
                iSig.Axes.YAxis = graphPlot.Axes.Right;
                iSig.LineWidth = 5;

                AgentManager manager = new AgentManager();

                Console.WriteLine("初期配置数");
                int _number = 0;
                int.TryParse(Console.ReadLine(), out _number);

                Console.WriteLine("ステップ数");
                int step = 0;
                int.TryParse(Console.ReadLine(), out step);

                Console.WriteLine("ログ出力間隔");
                int interval = 0;
                int.TryParse(Console.ReadLine(), out interval);

                graphPlot.Axes.SetLimitsX(0, step);
                graphPlot.Axes.SetLimitsY(0, _number * 1.5f, graphPlot.Axes.Left);
                graphPlot.Axes.SetLimitsY(0, 10000, graphPlot.Axes.Right);

                manager.Awake(_number);
                sw.Start();

                Console.WriteLine();
                Console.WriteLine("実行");
                Console.WriteLine();

                for (int i = 0; i < step; i++)
                {
                    manager.Update();

                    aCounts.Add(manager.Agents.Where(_ => _.IsActive).ToList().Count);
                    iCounts.Add(manager.Instructors.Count);

                    Visualize(manager.Agents, manager.Instructors, i);

                    if (i % interval == 0)
                    {
                        Console.WriteLine($"現在{i}ステップ目");
                        Console.WriteLine($"仮想ロボットは {manager.Agents.Where(_ => _.IsActive).ToList().Count}体存在");
                        Console.WriteLine($"命令物質の数は{manager.Instructors.Count}");
                        Console.WriteLine();
                    }
                }

                sw.Stop();
                Console.WriteLine("終了");
                Console.WriteLine($"所要時間 : {sw.ElapsedMilliseconds}");
                Console.WriteLine($"仮想ロボットの個体数 : {manager.Agents.Where(_ => _.IsActive).ToList().Count}");
                Console.WriteLine($"命令物質の数 : {manager.Instructors.Count}");
                Console.WriteLine();
            }
        }

        static void Visualize(List<Agent> _agents, List<Instructor> _instructors, int stepNumber)
        {
            mainPlot.Clear();

            mainPlot.Axes.SetLimits(-Settings.fieldSize, Settings.fieldSize, -Settings.fieldSize, Settings.fieldSize);

            //インストラクタの描画
            Instructor[] iActive = _instructors.Where(_ => _.IsActive).ToArray();
            float[] ix = iActive.Select(_ => _.Position.X).ToArray();
            float[] iy = iActive.Select(_ => _.Position.Y).ToArray();

            Scatter iScatter = mainPlot.Add.Scatter(ix, iy);
            iScatter.LineWidth = 0;
            iScatter.MarkerSize = 3;
            iScatter.Color = Colors.Blue.WithAlpha(0.5f);
            iScatter.MarkerShape = MarkerShape.FilledCircle;

            //エージェントの描画
            Agent[] aActive = _agents.Where(_ => _.IsActive).ToArray();
            float[] ax = aActive.Select(_ => _.Position.X).ToArray();
            float[] ay = aActive.Select(_ => _.Position.Y).ToArray();
           
            Scatter aScatter = mainPlot.Add.Scatter(ax, ay);
            aScatter.MarkerShape = MarkerShape.FilledSquare;
            aScatter.MarkerSize = 10;
            aScatter.LineWidth = 0;

            graphPlot.SavePng($"C:/Users/shinm/Videos/Result/graph{stepNumber:D5}.png", 1920, 1080);
            mainPlot.SavePng($"C:/Users/shinm/Videos/Result/result{stepNumber:D5}.png", 1920, 1080);
        }
    }
}