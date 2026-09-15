using UnityEngine;

public class fuego : MonoBehaviour
{
    public Light[] lights;

    public float minIntensity = 1f;
    public float maxIntensity = 3f;

    public float speed = 4f;

    void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            float noise = Mathf.PerlinNoise(
                Time.time * speed + i * 10f,
                0f
            );

            lights[i].intensity =
                Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}
