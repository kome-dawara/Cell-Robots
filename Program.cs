namespace MySpace
{
    using System.Linq;

    static class Program
    {
        static void Main()
        {
            System.Diagnostics.Stopwatch sw = new();

            while (true)
            {
                sw.Reset();

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
    }
}