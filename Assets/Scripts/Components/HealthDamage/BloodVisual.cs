using UnityEngine;

public class BloodVisual : MonoBehaviour
{
    private ParticleSystem bloodDrip;

    /// <summary>
    /// Crea el sistema de partículas de goteo de sangre.
    /// </summary>
    public void CreateDefaultBloodEffect()
    {
        GameObject dripGO = new GameObject("BloodDrip");
        dripGO.transform.SetParent(transform);
        dripGO.transform.localPosition = new Vector3(0, -0.3f, 0);

        bloodDrip = dripGO.AddComponent<ParticleSystem>();

        // Configurar el sistema de partículas
        var main = bloodDrip.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1f); 
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.startColor = Color.red;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 1f;
        main.loop = true;

        var emission = bloodDrip.emission;
        emission.rateOverTime = 0;

        var shape = bloodDrip.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15;
        shape.radius = 0.1f;

        var renderer = bloodDrip.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateDefaultBloodMaterial();
        renderer.sortingOrder = 100; // Muy alto, encima de todo
        bloodDrip.Stop();
    }

    /// <summary>
    /// Ajusta la cantidad de sangre que gotea.
    /// </summary>
    public void SetBloodLevel(float amount)
    {
        if (bloodDrip == null) return;

        var emission = bloodDrip.emission;
        emission.rateOverTime = amount;

        if (amount <= 0 && bloodDrip.isPlaying)
            bloodDrip.Stop();
        else if (amount > 0 && !bloodDrip.isPlaying)
            bloodDrip.Play();
    }

    private Material CreateDefaultBloodMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.color = Color.red;
        mat.SetFloat("_Mode", 2); // Fade
        return mat;
    }
}
