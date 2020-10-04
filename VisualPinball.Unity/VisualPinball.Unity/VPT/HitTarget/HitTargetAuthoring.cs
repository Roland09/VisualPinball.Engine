// Visual Pinball Engine
// Copyright (C) 2020 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

#region ReSharper
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
#endregion

using Unity.Entities;
using UnityEngine;
using VisualPinball.Engine.Game;
using VisualPinball.Engine.VPT.HitTarget;

namespace VisualPinball.Unity
{
	[ExecuteAlways]
	[AddComponentMenu("Visual Pinball/Hit Target")]
	public class HitTargetAuthoring : ItemMainAuthoring<HitTarget, HitTargetData>, IConvertGameObjectToEntity, IHittableAuthoring, ISwitchableAuthoring
	{
		protected override HitTarget InstantiateItem(HitTargetData data) => new HitTarget(data);

		public IHittable Hittable => Item;

		private void OnDestroy()
		{
			if (!Application.isPlaying) {
				Table?.Remove<HitTarget>(Name);
			}
		}

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			Convert(entity, dstManager);
			var table = gameObject.GetComponentInParent<TableAuthoring>().Item;

			dstManager.AddComponentData(entity, new HitTargetStaticData {
				TargetType = Data.TargetType,
				DropSpeed = Data.DropSpeed,
				RaiseDelay = Data.RaiseDelay,
				UseHitEvent = Data.UseHitEvent,
				RotZ = Data.RotZ,
				TableScaleZ = table.GetScaleZ()
			});
			dstManager.AddComponentData(entity, new HitTargetAnimationData {
				IsDropped = Data.IsDropped
			});
			dstManager.AddComponentData(entity, new HitTargetMovementData());

			// register
			var hitTarget = transform.GetComponent<HitTargetAuthoring>().Item;
			transform.GetComponentInParent<Player>().RegisterHitTarget(hitTarget, entity, gameObject);
		}

		public void RemoveHittableComponent()
		{
		}

		public void LinkChild(IItemAuthoring item)
		{
		}

		public override ItemDataTransformType EditorPositionType => ItemDataTransformType.ThreeD;
		public override Vector3 GetEditorPosition() => Data.Position.ToUnityVector3();
		public override void SetEditorPosition(Vector3 pos) => Data.Position = pos.ToVertex3D();

		public override ItemDataTransformType EditorRotationType => ItemDataTransformType.OneD;
		public override Vector3 GetEditorRotation() => new Vector3(Data.RotZ, 0f, 0f);
		public override void SetEditorRotation(Vector3 rot) => Data.RotZ = rot.x;

		public override ItemDataTransformType EditorScaleType => ItemDataTransformType.ThreeD;
		public override Vector3 GetEditorScale() => Data.Size.ToUnityVector3();
		public override void SetEditorScale(Vector3 scale) => Data.Size = scale.ToVertex3D();
	}
}
