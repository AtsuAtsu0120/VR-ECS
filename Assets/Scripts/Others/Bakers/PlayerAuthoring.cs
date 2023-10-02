using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
	class PlayerBaker : Baker<PlayerAuthoring>
	{
		public override void Bake(PlayerAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, typeof(PlayerTag));
		}
	}
}
