using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NeedInitTagAuthoring : MonoBehaviour
{
	class NeedInitTagBaker : Baker<NeedInitTagAuthoring>
	{
		public override void Bake(NeedInitTagAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, typeof(NeedInitTag));
		}
	}
}
