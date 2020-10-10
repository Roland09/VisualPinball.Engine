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

using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VisualPinball.Engine.Common;
using VisualPinball.Engine.Game;
using VisualPinball.Engine.VPT.Spinner;

namespace VisualPinball.Unity
{
	[ExecuteAlways]
	[AddComponentMenu("Visual Pinball/Game Item/Spinner")]
	public class SpinnerAuthoring : ItemMainAuthoring<Spinner, SpinnerData>,
		IHittableAuthoring, ISwitchableAuthoring, IConvertGameObjectToEntity
	{
		protected override Spinner InstantiateItem(SpinnerData data) => new Spinner(data);

		protected override Type MeshAuthoringType { get; } = typeof(ItemMeshAuthoring<Spinner, SpinnerData, SpinnerAuthoring>);

		public IHittable Hittable => Item;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			Convert(entity, dstManager);

			dstManager.AddComponentData(entity, new SpinnerStaticData {
				AngleMax = math.radians(Data.AngleMax),
				AngleMin = math.radians(Data.AngleMin),
				Damping = math.pow(Data.Damping, (float)PhysicsConstants.PhysFactor),
				Elasticity = Data.Elasticity,
				Height = Data.Height
			});

			// register
			transform.GetComponentInParent<Player>().RegisterSpinner(Item, entity, gameObject);
		}

		private void OnDestroy()
		{
			if (!Application.isPlaying) {
				Table?.Remove<Spinner>(Name);
			}
		}

		public void RemoveHittableComponent()
		{
		}

		public override ItemDataTransformType EditorPositionType => ItemDataTransformType.ThreeD;
		public override Vector3 GetEditorPosition() => Data.Center.ToUnityVector3(Data.Height);
		public override void SetEditorPosition(Vector3 pos)
		{
			Data.Center = pos.ToVertex2Dxy();
			Data.Height = pos.z;
		}

		public override ItemDataTransformType EditorRotationType => ItemDataTransformType.OneD;
		public override Vector3 GetEditorRotation() => new Vector3(Data.Rotation, 0f, 0f);
		public override void SetEditorRotation(Vector3 rot) => Data.Rotation = rot.x;

		public override ItemDataTransformType EditorScaleType => ItemDataTransformType.OneD;
		public override Vector3 GetEditorScale() => new Vector3(Data.Length, 0f, 0f);
		public override void SetEditorScale(Vector3 scale) => Data.Length = scale.x;
	}
}
