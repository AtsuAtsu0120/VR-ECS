using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

public partial struct AttackEnemySystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		state.Dependency = new EnterPlayerArea
		{
			players = state.GetComponentLookup<PlayerTag>(),
			enemies = state.GetComponentLookup<EnemyTag>(),
			ecb = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer()
		}.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

	new DamageJob{ parallelWriter = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter() }.ScheduleParallel();
	}
}
[BurstCompile]
public struct EnterPlayerArea : ITriggerEventsJob
{
	public ComponentLookup<PlayerTag> players;
	public ComponentLookup<EnemyTag> enemies;
	public EntityCommandBuffer ecb;

	public void Execute(TriggerEvent triggerEvent)
	{
		var isEntityAPlayer = players.HasComponent(triggerEvent.EntityA);
		var isEntityBPlayer = players.HasComponent(triggerEvent.EntityB);

		var isEntityAEnemy = enemies.HasComponent(triggerEvent.EntityA);
		var isEntityBEnemy = enemies.HasComponent(triggerEvent.EntityB);

		if(isEntityAPlayer && isEntityBEnemy)
		{
			AddDamageComponent(triggerEvent.EntityB);
		}
		else if(isEntityBPlayer && isEntityAEnemy)
		{
			AddDamageComponent(triggerEvent.EntityA);
		}
	}
	public void AddDamageComponent(Entity entity)
	{
		ecb.AddComponent(entity, new DamageData(20));
	}
}
[BurstCompile]
public partial struct DamageJob : IJobEntity
{
	public EntityCommandBuffer.ParallelWriter parallelWriter;
	public void Execute([EntityIndexInQuery] int index, ref DamageData damageData, ref EnemyData enemyData, Entity entity)
	{
		enemyData.hp -= damageData.damage;

		if (enemyData.hp <= 0)
		{
			parallelWriter.AddComponent(index, entity, new DeadTag());
		}
	}
}

