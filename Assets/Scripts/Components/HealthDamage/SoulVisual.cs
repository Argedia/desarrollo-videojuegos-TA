using UnityEngine;

public class SoulVisual : MonoBehaviour
{
    public float riseHeight = 2f;
    public float duration = 2f;
    public float startSize = 0.2f;
    public float endSize = 1f;
    public Color soulColor = new Color(1f, 1f, 1f, 0.7f); // blanco etéreo

    public void PlaySoulEffect()
    {   
        GameObject soulGO = new GameObject("SoulEffect");
        soulGO.transform.position = transform.position;

        var ps = soulGO.AddComponent<ParticleSystem>();

        // Detener completamente antes de modificar
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = duration;
        main.startLifetime = duration;
        main.startSpeed = 0;
        main.startSize = startSize;
        main.startColor = soulColor;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, startSize);
        sizeCurve.AddKey(1f, endSize);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(soulColor, 0.0f),
                new GradientColorKey(soulColor, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(soulColor.a, 0.0f),
                new GradientAlphaKey(0f, 1.0f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = riseHeight / duration;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateDefaultSoulMaterial();

        // Emitir después de configurar
        ps.Emit(1);
        Destroy(soulGO, duration + 0.5f);
    }

    private Material CreateDefaultSoulMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetColor("_Color", soulColor);
        mat.SetFloat("_Mode", 2); // Fade
        mat.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.EnableKeyword("_ALPHABLEND_ON");
        return mat;
    }
}
