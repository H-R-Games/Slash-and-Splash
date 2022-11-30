using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    [Header("Stats")]
    [Header("Skin")]
    public string SkinName = "Base Skin";
    public Color SpriteGlowColor = new Color(0, 24, 255);
    [Range(0, 10)] public float GlowBrightness = 10;
    [Range(0, 10)] public int OutlineWidth = 10;
    [Range(0, 1)] public float AlphaThreshold = 0;

    public Sprite PlayerSprite;

    [Header("Trail")]
    public Color TrailColor = new Color(0, 24, 255);

    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float MinDashRange = 3f;
    public float MaxDashRange = 9f;
    public float DashICD = 0.75f;

    [Header("Dash")]
    public float DashSpeed = 0.1f;
    public int MaxDashes = 3;
    [Range(0, 1)] public float SlowMotionValue = 0.3f;
    public float SpecialSkillCooldown = 20f;

    [Header("Health")]
    public int Lifes = 1;

    /*
     * Conseguir mas dinero
     * Ser inmune a daño de caida
     * Por cada combo de 30, +1 de vida
     * Autoaim
     */

}

