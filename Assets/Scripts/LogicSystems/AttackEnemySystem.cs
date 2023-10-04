using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct AttackEnemySystem : ISystem
{
	public ComponentLookup<PlayerTag> players;
	public ComponentLookup<EnemyTag> enemies;

	[BurstCompile]
	public void OnCreate(ref SystemBase state)
	{
		players = state.GetComponentLookup<PlayerTag>();
		enemies = state.GetComponentLookup<EnemyTag>();
	}
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		players.Update(ref state);
		enemies.Update(ref state);

		state.Dependency = new EnterPlayerArea
		{
			players = players,
			enemies = enemies,
			ecb = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer()
		}.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

		new DamageJob
		{ 
			parallelWriter = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter() 
		}.ScheduleParallel();
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
		UnityEngine.Debug.Log("当たりました。");
		var isEntityAPlayer = players.HasComponent(triggerEvent.EntityA);
		var isEntityBPlayer = players.HasComponent(triggerEvent.EntityB);

		var isEntityAEnemy = enemies.HasComponent(triggerEvent.EntityA);
		var isEntityBEnemy = enemies.HasComponent(triggerEvent.EntityB);

		if(isEntityAPlayer && isEntityBEnemy)
		{
			UnityEngine.Debug.Log("ダメージを与えます。");
			AddDamageComponent(triggerEvent.EntityB);
		}
		else if(isEntityBPlayer && isEntityAEnemy)
		{
			UnityEngine.Debug.Log("ダメージを与えます。");
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

