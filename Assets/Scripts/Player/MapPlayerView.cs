using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents player on map
/// </summary>
public class MapPlayerView : MonoBehaviour
{
    public float cubeSideSize = 1f;
    public float stepDuration = 0.15f;

    [SerializeField]
    private bool smoothRotation = false;

    //move queue
    private Queue<Vector3> targetPosQueue = new Queue<Vector3>();

    //called when move queue is empty
    public Action onMoveEnd;

    public void ResetAtPosition(Vector3 pos)
    {
        targetPosQueue.Clear();
        StopAllCoroutines();

        transform.position = pos + Vector3.up * cubeSideSize / 2;
        transform.rotation = Quaternion.identity;
    }
    public void QueueMoveToPosition(Vector3 targetPos)
    {
        targetPosQueue.Enqueue(targetPos);

        if (targetPosQueue.Count == 1)
            StartCoroutine(MoveCoroutine(targetPos));
    }
    private void MoveQueue()
    {
        targetPosQueue.Dequeue();
        if (targetPosQueue.Count != 0)
        {
            StartCoroutine(MoveCoroutine(targetPosQueue.Peek()));
        }
        else
        {
            //queue is empty!
            onMoveEnd?.Invoke();
        }
    }



    private IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        //move target pos to match desired cube offset
        targetPos += Vector3.up * cubeSideSize / 2;
        Vector3 fromPos = transform.position;
        Vector3 moveDirection = (targetPos - fromPos).normalized;

        int currentSteps = 0;
        float cubeSteps = Vector3.Distance(fromPos, targetPos) / cubeSideSize;
        float fullCubeSteps = Mathf.Floor(cubeSteps);

        float stepTravelDistance = cubeSideSize;
        float additionalTravelDistance = (cubeSteps - fullCubeSteps) * cubeSideSize;
        float additionalTravelDistancePerStep = additionalTravelDistance / fullCubeSteps;



        if (smoothRotation)
        {
            //rotate to target direction
            float rotDuration = 0.2f;
            float rotT = 0;

            Vector3 startRight = transform.right;
            Vector3 endRight = Vector3.Cross(Vector3.up, moveDirection);
            while (rotT < 1)
            {
                rotT += Time.deltaTime / rotDuration;
                transform.right = Vector3.Lerp(startRight, endRight, rotT);
                yield return null;
            }
        }
        else
        {
            transform.right = Vector3.Cross(Vector3.up, moveDirection);
        }

        //for each step
        while (currentSteps < fullCubeSteps)
        {
            float t = 0;

            Vector3 stepOriginPos = transform.position;
            Vector3 stepTargetPos = moveDirection * (stepTravelDistance) + stepOriginPos;

            //add some more distance to fit total travel distance to desired move steps
            Vector3 finalTargetPos = stepTargetPos + moveDirection * additionalTravelDistancePerStep;

            Quaternion stepOriginRotation = transform.rotation;
            Quaternion stepTargetRotation = Quaternion.AngleAxis(90, transform.right) * transform.rotation;


            //perform step
            while (t < 1)
            {
                t += Time.deltaTime / stepDuration;
                Vector3 currentPos = Vector3.Lerp(stepOriginPos, finalTargetPos, t);
                Quaternion currentRot = Quaternion.Slerp(stepOriginRotation, stepTargetRotation, t);

                transform.position = currentPos + GetVerticalOffset(t);
                transform.rotation = currentRot;


                yield return null;
            }
            transform.position = finalTargetPos;
            transform.rotation = stepTargetRotation;
            currentSteps++;
        }

        //move queue
        MoveQueue();
    }

    /// <summary>
    /// Returns additional height from floor to geometrical center when cube is rotating 90 degrees
    /// </summary>
    /// <param name="t">rotation time [0;1]</param>
    /// <returns></returns>
    private Vector3 GetVerticalOffset(float t)
    {
        float diagonal = Mathf.Sqrt(cubeSideSize * cubeSideSize / 2);
        float diagonalDiff = diagonal - cubeSideSize / 2;
        return t < 0.5f ? Vector3.up * Mathf.Lerp(0, diagonalDiff, t * 2) : Vector3.up * Mathf.Lerp(diagonalDiff, 0, (t - 0.5f) * 2);
    }
}