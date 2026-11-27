using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace DeftHands
{
    internal sealed class RotateController
    {
        private const float InputDeadzone = 0.001f;
        private const float DepthStepMeters = 0.15f;
        private const float DepthEaseRate = 18f;
        private const float DepthSnapEpsilon = 0.001f;
        private const float MinHoldDepth = 0.5f;
        private const float MaxHoldDepth = 3.0f;

        private static readonly FieldInfo IsInGridModeField = AccessTools.Field(typeof(PlayerInteractable), "isInGridMode");
        private static readonly FieldInfo InitialLocalPositionField = AccessTools.Field(typeof(PlayerInteractable), "InitialLocalPosition");

        private PlayerInteractable player;
        private InteractableItems activeItem;
        private bool hasTargetDepth;
        private float targetDepthZ;
        private bool suppressNextRightClickUp;

        public void BindPlayer(PlayerInteractable instance)
        {
            player = instance;
            activeItem = null;
            hasTargetDepth = false;
            suppressNextRightClickUp = false;
        }

        public bool IsInGridMode(PlayerInteractable instance)
        {
            return instance != null && IsInGridModeField != null && (bool)IsInGridModeField.GetValue(instance);
        }

        public bool CanControlHeldItem(PlayerInteractable instance)
        {
            if (instance == null || !instance.isActiveAndEnabled)
            {
                return false;
            }

            InteractableItems item = instance.Get_HoldingItem;
            return RotateMath.CanManipulateItem(
                item != null,
                item != null && item.CanRotate(),
                IsInGridMode(instance),
                item != null && item.transform.parent == instance.transform,
                Cursor.lockState == CursorLockMode.Locked);
        }

        public bool IsRotating
        {
            get
            {
                return RotateConfig.EnableMouseRotation.Value
                    && CanControlHeldItem(player)
                    && RotateConfig.IsHeld(RotateConfig.RotateKey.Value);
            }
        }

        public bool ShouldBlockRightClickDown()
        {
            if (!IsRotating)
            {
                return false;
            }

            suppressNextRightClickUp = true;
            return true;
        }

        public bool ShouldBlockRightClickUp()
        {
            if (!suppressNextRightClickUp)
            {
                return false;
            }

            suppressNextRightClickUp = false;
            return true;
        }

        public void AdjustHoldDepth(float scrollDirection)
        {
            if (!CanControlHeldItem(player))
            {
                return;
            }

            InteractableItems item = player.Get_HoldingItem;
            SyncActiveItem(item);

            float currentZ = item.transform.localPosition.z;
            float baseZ = hasTargetDepth ? targetDepthZ : currentZ;
            targetDepthZ = RotateMath.ClampDepthTarget(
                currentZ,
                baseZ + scrollDirection * DepthStepMeters,
                MinHoldDepth,
                MaxHoldDepth);
            hasTargetDepth = true;
        }

        public void Update()
        {
            if (player == null)
            {
                activeItem = null;
                hasTargetDepth = false;
                suppressNextRightClickUp = false;
                return;
            }

            InteractableItems item = player.Get_HoldingItem;
            SyncActiveItem(item);

            if (!CanControlHeldItem(player))
            {
                hasTargetDepth = false;
                return;
            }

            StepHoldDepth(item);
            StepMouseRotation(item);
        }

        private void SyncActiveItem(InteractableItems item)
        {
            if (activeItem != item)
            {
                activeItem = item;
                hasTargetDepth = false;
            }
        }

        private void StepHoldDepth(InteractableItems item)
        {
            if (!hasTargetDepth)
            {
                return;
            }

            Vector3 localPos = item.transform.localPosition;
            float nextZ = RotateMath.EaseTowardDepth(
                localPos.z,
                targetDepthZ,
                DepthEaseRate,
                Time.deltaTime,
                DepthSnapEpsilon);

            float appliedDelta = nextZ - localPos.z;
            if (appliedDelta != 0f)
            {
                localPos.z = nextZ;
                item.transform.localPosition = localPos;

                if (InitialLocalPositionField != null)
                {
                    Vector3 initialPos = (Vector3)InitialLocalPositionField.GetValue(player);
                    initialPos.z += appliedDelta;
                    InitialLocalPositionField.SetValue(player, initialPos);
                }
            }

            if (nextZ == targetDepthZ)
            {
                hasTargetDepth = false;
            }
        }

        private void StepMouseRotation(InteractableItems item)
        {
            if (!IsRotating)
            {
                return;
            }

            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");
            if (Mathf.Abs(deltaX) < InputDeadzone && Mathf.Abs(deltaY) < InputDeadzone)
            {
                return;
            }

            Camera cam = player.mainCamera != null ? player.mainCamera : Camera.main;
            if (cam == null)
            {
                return;
            }

            RotationStep step = RotateMath.CalculateRotationStep(
                deltaX,
                deltaY,
                RotateConfig.RotationSpeed.Value,
                RotateConfig.DefaultHorizontalAxis.Value == HorizontalAxis.Yaw,
                RotateConfig.IsHeld(RotateConfig.SwapAxisKey.Value),
                RotateConfig.InvertPitch.Value,
                RotateConfig.InvertRoll.Value,
                RotateConfig.InvertYaw.Value);

            Vector3 pivot = item.meshRenderer != null
                ? item.meshRenderer.bounds.center
                : item.transform.position;

            Transform camTransform = cam.transform;
            if (step.HorizontalDegrees != 0f)
            {
                Vector3 axis = step.UseYaw ? camTransform.up : camTransform.forward;
                item.transform.RotateAround(pivot, axis, step.HorizontalDegrees);
            }

            if (step.PitchDegrees != 0f)
            {
                item.transform.RotateAround(pivot, camTransform.right, step.PitchDegrees);
            }
        }
    }
}
