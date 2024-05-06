/**
 * CustomMath 는 수학적 처리와 관련해서 사용할 다양한 함수들을 모아두는 클래스 파일이다.
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;
public enum _easingType { none, easeInSine, easeInOutCubic, length, easeOutQuint };

public class CustomMath : MonoBehaviour
{
    static public float GetEase(_easingType type, float x) {
        switch (type) {
            case _easingType.easeInSine:
                return easeInSine(x);
            case _easingType.easeInOutCubic:
                return easeInOutCubic(x);
            case _easingType.easeOutQuint:
                return easeOutQuint(x);
        }
        return x;
    }

    static float easeInSine (float x) {
        return 1 - Mathf.Cos((x* Mathf.PI) / 2);
    }
    static float easeInOutCubic(float x) {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    static float easeOutQuint(float x) {
        return 1 - Mathf.Pow(1 - x, 5);
    }

    static public Quaternion GetEulerToVector(Vector3 v3) {
        return Quaternion.Euler(new Vector3(0, 0, v3.y > 0 ? -math.atan(v3.x / v3.y) / math.PI* 180 : -math.atan(v3.x / v3.y) / math.PI* 180 + 180));
    }
}
