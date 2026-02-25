using System.Diagnostics;
using System.Numerics;

//エージェントのセルクラス
public class AgentCell
{
    public CellType Type { get; set; } = CellType.Empty;
    //セルがアクティブかどうか
    public bool IsActive { get; set; } = true;
    //命令物質を解読するための固有キー
    public byte Key { get; set; }
    //現在入力されている命令物質のコード（未解読）
    public byte CurrentProcessed { get; set; }
    //エージェントの構造の中での座標
    public Vector2 GridPosition { get; set; }
    public Vector2 Velocity { get; set; }
    //リソース量
    public float Resources
    {
        get
        {
            return resources;
        }
        set
        {
            resources = value;
            if (resources <= 0)
                Destroy();
        }
    }
    float resources;

    //現在接触（自身との距離が0.5未満）している命令物質を返す
    public (bool, Instructor) ContactedInstructor(List<Instructor> _instructors, Vector2 agentPosition, float agentRotation, Vector2 centerOfMass)
    {
        CurrentProcessed = 0;

        float sin = (float)Math.Sin(agentRotation);
        float cos = (float)Math.Cos(agentRotation);

        Vector2 position = new()
        {
            X = ((GridPosition.X - centerOfMass.X) * cos - (GridPosition.Y - centerOfMass.Y) * sin) + agentPosition.X,
            Y = ((GridPosition.X - centerOfMass.X) * sin + (GridPosition.Y - centerOfMass.Y) * cos) + agentPosition.Y
        };

        foreach (Instructor _instructor in _instructors)
        {
            if (_instructor.IsActive == false)
                continue;

            if (Vector2.DistanceSquared(_instructor.Position, position) < 0.25f)
            {
                _instructor.IsActive = false;
                return (true, _instructor);
            }
        }
        return (false, new(0));
    }

    //命令物質を解析し、実行する命令の種類を返す
    public (InstructionType, byte) ProcessCode()
    {
        if (CurrentProcessed == 0)
            return (InstructionType.Empty, 0);

        //XORで変換
        byte processCode = (byte)(Key ^ CurrentProcessed);

        //オペランド（引数）
        byte operand = (byte)(processCode >> 6);

        switch (operand)
        {
            case 0b00: //推進
                Debug.WriteLine("Thrust");
                return (InstructionType.Thrust, processCode);

            case 0b01: //資源
                Debug.WriteLine("Resource");
                return (InstructionType.Resource, processCode);

            case 0b10 or 0b11: //命令物質の複製
                Debug.WriteLine("Release");
                return (InstructionType.Release, processCode);

            default:
                return (InstructionType.Empty, processCode);
        }
    }

    public void Destroy()
    {
        IsActive = false;
    }
}

public enum InstructionType
{
    Empty,
    Thrust,
    Resource,
    Release
}