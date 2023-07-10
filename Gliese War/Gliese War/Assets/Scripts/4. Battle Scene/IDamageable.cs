using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);
}
