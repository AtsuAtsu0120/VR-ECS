using Unity.Entities;

public partial struct DestroyEnemySystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		new DestroyEnemyJob
		{
			parallelWriter = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter()
		}.ScheduleParallel();
	}
}
public partial struct DestroyEnemyJob : IJobEntity
{
	public EntityCommandBuffer.ParallelWriter parallelWriter;
	public void Execute([EntityIndexInQuery] int index, in DeadTag _, Entity entity)
	{
		UnityEngine.Debug.Log("çÌèúÅI");
		parallelWriter.DestroyEntity(index, entity);
	}
}

