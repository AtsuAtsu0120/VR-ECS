using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnerSystem : ISystem
{
	private float radius;
	private Unity.Mathematics.Random random;
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		radius = 10f;
		random = new Unity.Mathematics.Random();
	}
	public void OnUpdate(ref SystemState state)
	{
		var cmb = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

		var entity = SystemAPI.GetSingleton<PlayerData>().enemyPrefab;
		cmb.Instantiate(entity);

		var seed = (uint)(DateTime.Now.Millisecond * 1000);
		if(seed != 0)
		{
			new InitObjectPosition
			{
				parallelWriter = cmb.AsParallelWriter(),
				random = random,
				seed = seed,
				radius = radius,
			}.ScheduleParallel();
		}
	}
	[BurstCompile]
	public partial struct InitObjectPosition : IJobEntity
	{
		public EntityCommandBuffer.ParallelWriter parallelWriter;
		public Unity.Mathematics.Random random;

		public uint seed;
		public float radius;
		public void Execute([EntityIndexInChunk] int index, in NeedInitTag _, Entity entity, ref LocalTransform transform)
		{
			parallelWriter.RemoveComponent<NeedInitTag>(index, entity);

			// PositionÇê›íË
			random.InitState(seed);
			var rad = random.NextFloat(0, 360);

			var positionX = math.cos(rad) * radius;
			var positionZ = math.sin(rad) * radius;
			var positionY = random.NextFloat(0, 0.5f);

			var position = new float3(positionX, positionY, positionZ);

			transform.Position = position;
		}
	}
}
