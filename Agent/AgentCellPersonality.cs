using System.Collections;

//セルの固定される特徴（キーなど）
public class AgentCellPersonality
{
    //タイプとあるが、存在する（NonEmpty）か存在しない（Empty）の二択で、実質boolean
    public CellType Type { get; set; } = CellType.Empty;
    //生成時にAgentCellにコピーされる変換キー
    public byte Key { get; set; }

    public AgentCellPersonality(CellType _type)
    {
        Type = _type;
        Key = (byte)Random.Shared.Next(0, 255);
    }
}

public enum CellType
{
    Empty, //セルが存在しない
    NonEmpty //存在する
}