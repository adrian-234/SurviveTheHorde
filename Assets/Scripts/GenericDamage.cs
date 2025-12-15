using UnityEngine;

/*
    Ez az osztaly csak azert van hogy minden player es enemy egyseges modon tudja megszerezni azt hogy mennyit sebzodot
*/
public class GenericDamage : MonoBehaviour
{
    public float damage; //Mennyit sebez az adott lovedek/ellenseg/robbanas...
    public float knockback; //Mennyivel tud vissza lokni az adott dolog 
    public Targets targetType; //Kiket tud megsebezni az adott dolog

    public enum Targets {
        All,
        Enemy,
        Player
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Vector3 direction = collision.transform.position - transform.position;
            direction = direction.normalized;

            collision.transform.Translate(knockback * Time.deltaTime * direction);
        }
    }
}
