﻿using System.Collections.Generic;
using NLog;
using Unity.Entities;
using UnityEngine;
using VisualPinball.Engine.Physics;
using VisualPinball.Unity.VPT.Table;
using Logger = NLog.Logger;

namespace VisualPinball.Unity.Physics.Collision
{
	[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
	public class QuadTreeConversionSystem : GameObjectConversionSystem
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		protected override void OnUpdate()
		{
			// fixme
			if (DstEntityManager.CreateEntityQuery(typeof(CollisionData)).CalculateEntityCount() > 0) {
				return;
			}

			var table = Object.FindObjectOfType<TableBehavior>().Table;

			foreach (var playable in table.Playables) {
				playable.SetupPlayer(null, table);
			}

			// index hittables
			var hitObjects = new List<HitObject>();
			foreach (var hittable in table.Hittables) {
				foreach (var hitObject in hittable.GetHitShapes()) {
					hitObject.ItemIndex = hittable.Index;
					hitObjects.Add(hitObject);
					hitObject.CalcHitBBox();
				}
			}

			// construct quad tree
			var quadTree = new HitQuadTree(hitObjects, table.Data.BoundingBox);
			var quadTreeBlobAssetRef = QuadTree.CreateBlobAssetReference(quadTree);

			// save it to entity
			var collEntity = DstEntityManager.CreateEntity(ComponentType.ReadOnly<CollisionData>());
			DstEntityManager.SetName(collEntity, "Collision Holder");
			DstEntityManager.SetComponentData(collEntity, new CollisionData { QuadTree = quadTreeBlobAssetRef });

			Logger.Info("Static QuadTree initialized.");
		}
	}
}