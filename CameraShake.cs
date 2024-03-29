﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles what background music.
/// 
/// </summary>
public class CameraShake : MonoBehaviour
{

    /// <summary>
    /// Shake the camera.
    /// 
    /// Currently deprecated.
    /// </summary>
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originialPosition = transform.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originialPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originialPosition;
    }

}
