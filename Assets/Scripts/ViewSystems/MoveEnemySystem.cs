using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public partial struct MoveEnemySystem : ISystem
{
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		LocalTransform playerTransform = new();
		foreach(var (transform, _) in SystemAPI.Query<LocalTransform, PlayerTag>())
		{
			playerTransform = transform;
		}
		foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>())
		{
			var vector = playerTransform.Position - transform.ValueRW.Position;
			var direction = math.normalize(vector);
			direction.y = 0;

			transform.ValueRW.Position += 0.1f * direction;
		}
	}
}
