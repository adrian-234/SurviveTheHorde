using UnityEngine;

/*
    Ez az osztaly csak azert van hogy minden player es enemy egyseges modon tudja megszerezni azt hogy mennyit sebzodot
*/
public class GenericDamage : MonoBehaviour
{
    public float damage; //Mennyit sebez az adott lovedek/ellenseg/robbanas...
    public Targets targetType; //Kiket tud megsebezni az adott dolog

    public enum Targets {
        All,
        Enemy,
        Player
    }
}
