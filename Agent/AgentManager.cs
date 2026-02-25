using System.Collections.Generic;
using System.Diagnostics;


public class AgentManager
{
    int agentNumber;

    public List<Agent> Agents { get; private set; }
    public List<Agent> AgentsQueue { get; private set; } = new();
    public List<Instructor> Instructors { get; private set; }
    public List<Instructor> InstructorsQueue { get; private set; } = new();

    public AgentManager(List<Agent> agents, List<Instructor> instructors)
    {
        Agents = agents;
        Instructors = instructors;
    }

    public AgentManager() : this([], [])
    {

    }

    public void Awake(int number)
    {
        agentNumber = number;
        Debug.WriteLine(agentNumber);

        for (int i = 0; i < updateInstructorNumber; i++)
            InstructorsQueue.Add(new(Settings.instructorCode) { Position = new System.Numerics.Vector2(System.Random.Shared.Next(-Settings.fieldSize, Settings.fieldSize), System.Random.Shared.Next(-Settings.fieldSize, Settings.fieldSize)) });

        for (int i = 0; i < agentNumber; i++)
        {
            Agent _agent = new Agent();
            _agent.AgentCreated += (_) => AgentsQueue.Add(_);
            _agent.InstructorReleased += (_) => InstructorsQueue.Add(_);
            Agents.Add(_agent);
        }
    }

    public void Start()
    {
        foreach (Agent agent in Agents)
            agent.Start();
    }
  
    private readonly int updateInstructorNumber = 100;

    public void Update()
    {
        for (int i = 0; i < updateInstructorNumber; i++)
            InstructorsQueue.Add(new(Settings.instructorCode) { Position = new System.Numerics.Vector2(System.Random.Shared.Next(-Settings.fieldSize * 100, Settings.fieldSize * 100) / 100f, System.Random.Shared.Next(-Settings.fieldSize * 100, Settings.fieldSize * 100) / 100f) });

        foreach (Agent agent in Agents)
        {
            if(agent.IsActive)
                agent.Update(Instructors);
        }

        foreach(Instructor instructor in Instructors)
        {
            instructor.Age++;
            if (instructor.Age + 1 >= 100)
                instructor.IsActive = false;
        }

        //V‹K¶¬Agent&Instructor‚ðƒŠƒXƒg‚É’Ç‰Á
        foreach (Agent agent in AgentsQueue)
            agent.Start();
        Agents.AddRange(AgentsQueue);
        AgentsQueue.Clear();
        Instructors.AddRange(InstructorsQueue);
        Instructors.RemoveAll(_ => _.IsActive == false);
        InstructorsQueue.Clear();
    }
}
