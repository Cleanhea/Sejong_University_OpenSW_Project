/// <summary>
/// 데미지를 받을 수 있는 오브젝트가 구현하는 인터페이스입니다.
/// </summary>
public interface IDamageable
{
    /// <summary>지정한 양만큼 데미지를 입힙니다.</summary>
    /// <param name="amount">받을 데미지 값입니다.</param>
    void TakeDamage(float amount);
}
