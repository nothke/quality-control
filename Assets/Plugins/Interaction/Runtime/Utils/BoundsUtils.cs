﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nothke.Utils
{
    public static class BoundsUtils
    {
        // static list for caching, never deallocates
        static List<Collider> colliderCache = new List<Collider>();

        public static Bounds GetObjectSpaceColliderBounds(GameObject go, bool includeInactive = false)
        {
            Transform t = go.transform;
            var rootW2L = t.worldToLocalMatrix;

            go.GetComponentsInChildren(includeInactive, colliderCache);

            if (colliderCache.Count == 0)
            {
                Debug.LogError("Attempting to get bounds of the object but it has no colliders");
                return default;
            }

            Bounds goBounds = GetBoundsInRootSpace(colliderCache[0]);

            for (int i = 1; i < colliderCache.Count; i++)
            {
                Bounds b = GetBoundsInRootSpace(colliderCache[i]);
                goBounds.Encapsulate(b);
            }

            return goBounds;

            Bounds GetBoundsInRootSpace(Collider col)
            {
                Bounds b = col.GetLocalBounds();
                Matrix4x4 l2w = col.transform.localToWorldMatrix;
                Matrix4x4 local = rootW2L * l2w;
                return TransformBounds(local, b);
            }
        }

        public static Bounds GetLocalBounds(this Collider collider)
        {
            if (collider is BoxCollider)
            {
                BoxCollider box = (BoxCollider)collider;
                return new Bounds(box.center, box.size);
            }
            else if (collider is SphereCollider)
            {
                var center = ((SphereCollider)collider).center;
                var radius = ((SphereCollider)collider).radius;
                Vector3 size = new Vector3(radius * 2, radius * 2, radius * 2);
                return new Bounds(center, size);
            }
            else if (collider is CapsuleCollider)
            {
                var capsule = (CapsuleCollider)collider;
                var r = capsule.radius;
                var h = capsule.height;

                Vector3 size;
                switch (capsule.direction)
                {
                    case 0: size = new Vector3(h, r * 2, r * 2); break;
                    case 1: size = new Vector3(r * 2, h, r * 2); break;
                    case 2: size = new Vector3(r * 2, r * 2, h); break;
                    default: size = default; break;
                }

                return new Bounds(capsule.center, size);
            }
            else if (collider is MeshCollider)
            {
                return ((MeshCollider)collider).sharedMesh.bounds;
            }

            Debug.LogError("Attempting to get bounds of an unknown collider type");
            return new Bounds();
        }

        public static Bounds TransformBounds(in Matrix4x4 mat, in Bounds bounds)
        {
            // Find 8 corners of the bounds
            Vector3 p0 = bounds.min;
            Vector3 p1 = bounds.max;
            Vector3 p2 = new Vector3(p0.x, p0.y, p1.z);
            Vector3 p3 = new Vector3(p0.x, p1.y, p0.z);
            Vector3 p4 = new Vector3(p1.x, p0.y, p0.z);
            Vector3 p5 = new Vector3(p0.x, p1.y, p1.z);
            Vector3 p6 = new Vector3(p1.x, p0.y, p1.z);
            Vector3 p7 = new Vector3(p1.x, p1.y, p0.z);

            Bounds b = new Bounds(mat * p0, Vector3.zero);
            b.Encapsulate(mat * p1);
            b.Encapsulate(mat * p2);
            b.Encapsulate(mat * p3);
            b.Encapsulate(mat * p4);
            b.Encapsulate(mat * p5);
            b.Encapsulate(mat * p6);
            b.Encapsulate(mat * p7);

            return b;
        }

        public static void DrawBoundsGizmos(in Bounds bounds)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}