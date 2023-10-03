using JetBrains.Annotations;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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
		var seed = (uint)(DateTime.Now.Millisecond * 1000);
		float rad;
		try
		{
			random.InitState(seed);
			rad = random.NextFloat(0, 360);
		}
		catch (ArgumentException) 
		{
			return;
		}

		var positionX = math.cos(rad) * radius;
		var positionZ = math.sin(rad) * radius;

		var position = new float3(positionX, 0, positionZ);

		var cmb = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

		var entity = SystemAPI.GetSingleton<PlayerData>().enemyPrefab;
		cmb.Instantiate(entity);
	}
}
