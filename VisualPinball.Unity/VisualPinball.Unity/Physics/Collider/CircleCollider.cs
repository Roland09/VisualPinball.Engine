﻿using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using VisualPinball.Engine.Physics;
using VisualPinball.Unity.Extensions;
using VisualPinball.Unity.Physics.Collision;
using VisualPinball.Unity.VPT.Ball;

namespace VisualPinball.Unity.Physics.Collider
{
	public struct CircleCollider : ICollider, ICollidable
	{
		private ColliderHeader _header;

		private float2 _center;
		private float _radius;

		public ColliderType Type => _header.Type;

		public static void Create(HitCircle src, ref BlobPtr<Collider> dest, BlobBuilder builder)
		{
			var collider = default(CircleCollider);
			collider.Init(src);
			ref var ptr = ref UnsafeUtilityEx.As<BlobPtr<Collider>, BlobPtr<CircleCollider>>(ref dest);
			builder.Allocate(ref ptr);
		}

		private void Init(HitCircle src)
		{
			_header.Type = ColliderType.Circle;
			_header.EntityIndex = src.ItemIndex;
			_header.HitBBox = src.HitBBox.ToAabb();

			_center = src.Center.ToUnityFloat2();
			_radius = src.Radius;
		}

		public float HitTest(BallData ball, float dTime, CollisionEvent coll)
		{
			return -1;
		}
	}
}