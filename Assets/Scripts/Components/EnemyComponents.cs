using Unity.Entities;

public struct EnemyTag : IComponentData
{

}
public struct EnemyData : IComponentData
{
	public Entity prefab;
	public int hp;
	public int stlength;
}
public struct DamageData : IComponentData
{
	public int damage;

	public DamageData(int damage)
	{
		this.damage = damage;
	}
}
public struct DeadTag : IComponentData
{

}
public struct NeedInitTag : IComponentData
{

}