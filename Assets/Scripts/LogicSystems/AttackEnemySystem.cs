using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct AttackEnemySystem : ISystem
{
	public ComponentLookup<PlayerTag> players;
	public ComponentLookup<EnemyTag> enemies;
	public bool isInited;

	[BurstCompile]
	public void OnCreate(ref SystemBase state)
	{
		//TODO: なぜかここでGetComponentLookupをするとエラーになる。原因探る。
		isInited = false;
	}

	public void OnUpdate(ref SystemState state)
	{
		if(!isInited)
		{
			players = state.GetComponentLookup<PlayerTag>();
			enemies = state.GetComponentLookup<EnemyTag>();

			isInited = true;
		}
		else
		{
			players.Update(ref state);
			enemies.Update(ref state);
		}

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

		state.Dependency.Complete();
	}
}
[BurstCompile]
public partial struct EnterPlayerArea : ITriggerEventsJob
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

